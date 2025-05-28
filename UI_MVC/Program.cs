using AspNetCoreLiveMonitoring.Extensions;
using CitizenPanel.BL.Content;
using CitizenPanel.BL.Domain.Tenancy;
using CitizenPanel.BL.Domain.Users;
using CitizenPanel.BL.Draws;
using CitizenPanel.BL.Panels;
using CitizenPanel.BL.Questionnaires;
using CitizenPanel.BL.Registrations;
using CitizenPanel.BL.Tenancy;
using CitizenPanel.BL.Users;
using CitizenPanel.BL.Utilities;
using CitizenPanel.DAL.Content;
using CitizenPanel.DAL.Data;
using CitizenPanel.DAL.Draws;
using CitizenPanel.DAL.Panels;
using CitizenPanel.DAL.Questionnaires;
using CitizenPanel.DAL.ServiceInterfaces;
using CitizenPanel.DAL.Tenancy;
using CitizenPanel.DAL.Users;
using CitizenPanel.UI.MVC;
using CitizenPanel.UI.MVC.Areas.Identity.DutchLocalization;
using CitizenPanel.UI.MVC.Areas.Identity.Managers;
using CitizenPanel.UI.MVC.Areas.Identity.Services;
using CitizenPanel.UI.MVC.Middleware;
using CitizenPanel.UI.MVC.Services;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
//using StackExchange.Redis;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    ContentRootPath = AppContext.BaseDirectory
});


// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext with PostgreSQL
builder.Services.AddDbContext<PanelDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Repositories and Managers
builder.Services.AddScoped<IDrawRepository, DrawRepository>();
builder.Services.AddScoped<IPanelRepository, PanelRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IPostManager, PostManager>();
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
builder.Services.AddScoped<IContentManager, ContentManager>();
builder.Services.AddScoped<IContentRepository, ContentRepository>();
builder.Services.AddScoped<ITenantManager, TenantManager>();
builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<ITenantResolver, TenantResolver>();
builder.Services.AddScoped<UserManager<ApplicationUser>, ApplicationUserManager>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<ITenantAccessService, TenantAccessService>();
builder.Services.AddLiveMonitoring();
builder.Services.AddRazorPages();

/*// Redis setup
var redisIp = Environment.GetEnvironmentVariable("REDIS_IP");
var redisConnection = $"{redisIp}:6379,abortConnect=false";

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnection;
    options.InstanceName = "redisdb";
});

var redis = ConnectionMultiplexer.Connect(redisConnection);
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);

builder.Services.AddDataProtection()
    .PersistKeysToStackExchangeRedis(redis, "BurgerPanel-DataProtection-Keys");

// Session via Redis
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});*/

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = true;
})
    .AddUserManager<ApplicationUserManager>()
    .AddEntityFrameworkStores<PanelDbContext>()
    .AddErrorDescriber<DutchIdentityErrorDescriber>()
    .AddDefaultTokenProviders();

// Existing StorageClient registratio
builder.Services.AddSingleton<StorageClient>(provider =>
{
    var credential = GoogleCredential.GetApplicationDefault()
        .CreateScoped("https://www.googleapis.com/auth/devstorage.read_write");
    return StorageClient.Create(credential);
});

builder.Services.AddLiveMonitoring();

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

// Tenant + Route config
builder.Services.Configure<RouteOptions>(options =>
{
    options.ConstraintMap.Add("validTenant", typeof(ValidTenantConstraint));
});

var tenantStore = new TenantStore();
builder.Services.AddSingleton(tenantStore);


builder.Services
    .AddTenantContext()
    .AddScoped<TenantMiddleware>();

var app = builder.Build();

app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PanelDbContext>();
    tenantStore.TenantIds.UnionWith(db.Tenants.Select(t => t.Id));
}

using (IServiceScope scope = app.Services.CreateScope()) {
    PanelDbContext context = scope.ServiceProvider.GetRequiredService<PanelDbContext>();
    if (context.CreateDatabase(true)) {
        var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
        IdentitySeeder identitySeeder = new IdentitySeeder(userManager, roleManager);
        DataSeeder dataSeeder = new DataSeeder(context);
        dataSeeder.SeedTenants();
        await identitySeeder.SeedAsync();
        dataSeeder.Seed();
    }
}

// Configure HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAndMapLiveMonitoring();

// Order is important!
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<TenantMiddleware>();

app.MapControllerRoute(
    name: "tenant",
    pattern: "{tenantId:validTenant}/{controller=Home}/{action=Index}/{id?}",
    constraints: new { tenantId = @"^[a-zA-Z0-9_-]+$" });

app.MapControllerRoute(
    name: "tenant-api",
    pattern: "{tenantId:validTenant}/api/{controller=Home}/{action=Index}/{id?}",
    constraints: new { tenantId = @"^[a-zA-Z0-9_-]+$" });



app.MapControllerRoute(
    name: "public",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();