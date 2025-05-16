using AspNetCoreLiveMonitoring.Extensions;
using CitizenPanel.BL.Domain.Tenancy;
using CitizenPanel.BL.Domain.Users;
using CitizenPanel.BL.Draws;
using CitizenPanel.BL.Panels;
using CitizenPanel.BL.Questionnaires;
using CitizenPanel.BL.Registrations;
using CitizenPanel.BL.Users;
using CitizenPanel.BL.Utilities;
using CitizenPanel.DAL.Data;
using CitizenPanel.DAL.Draws;
using CitizenPanel.DAL.Panels;
using CitizenPanel.DAL.Questionnaires;
using CitizenPanel.DAL.Users;
using CitizenPanel.UI.MVC;
using CitizenPanel.UI.MVC.Areas.Identity.DutchLocalization;
using CitizenPanel.UI.MVC.Areas.Identity.Managers;
using CitizenPanel.UI.MVC.Areas.Identity.Services;
using CitizenPanel.UI.MVC.Middleware;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext with PostgreSQL
builder.Services.AddDbContext<PanelDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Repositories and Managers
builder.Services.AddScoped<IDrawRepository, DrawRepository>();
builder.Services.AddScoped<IPanelRepository, PanelRepository>();
builder.Services.AddScoped<IQuestionnaireRepository, QuestionnaireRepository>();
builder.Services.AddScoped<IDrawManager, DrawManager>();
builder.Services.AddScoped<IPanelManager, PanelManager>();
builder.Services.AddScoped<IQuestionnaireManager, QuestionnaireManager>();
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
builder.Services.AddScoped<IRegistrationManager, RegistrationManager>();
builder.Services.AddScoped<IMeetingManager, MeetingManager>();
builder.Services.AddScoped<IMeetingRepository, MeetingRepository>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<IUserProfileManager, UserProfileManager>();
builder.Services.AddScoped<IUtilityManager, UtilityManager>();
builder.Services.AddScoped<UserManager<ApplicationUser>, ApplicationUserManager>();
builder.Services.AddLiveMonitoring();
builder.Services.AddRazorPages();

// Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedAccount = true;
    })
    .AddUserManager<ApplicationUserManager>()
    .AddEntityFrameworkStores<PanelDbContext>()
    .AddErrorDescriber<DutchIdentityErrorDescriber>()
    .AddDefaultTokenProviders();

builder.Services.AddLiveMonitoring();

// Configure Redis for session and data protection
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = Environment.GetEnvironmentVariable("REDIS_IP") + ":6379";
    options.InstanceName = "redisdb";
});

var redis = ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("REDIS_IP") + ":6379");
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);

builder.Services.AddDataProtection()
    .PersistKeysToStackExchangeRedis(redis, "BurgerPanel-DataProtection-Keys");

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

builder.Services
    .AddTenantContext()
    .AddScoped<TenantMiddleware>();

var app = builder.Build();

app.MapRazorPages();

using (IServiceScope scope = app.Services.CreateScope()) {
    PanelDbContext context = scope.ServiceProvider.GetRequiredService<PanelDbContext>();
    if (context.CreateDatabase(true)) {
        var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
        IdentitySeeder identitySeeder = new IdentitySeeder(userManager, roleManager);
        await identitySeeder.SeedAsync();
        DataSeeder dataSeeder = new DataSeeder(context);
        dataSeeder.Seed();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAndMapLiveMonitoring();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<TenantMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
