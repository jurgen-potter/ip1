using AspNetCoreLiveMonitoring.Extensions;
using CitizenPanel.BL;
using CitizenPanel.BL.Domain.Panel;
using CitizenPanel.BL.Domain.Tenancy;
using CitizenPanel.BL.Domain.User;
using CitizenPanel.BL.QuestionnaireModule;
using CitizenPanel.BL.Registration;
using CitizenPanel.DAL;
using CitizenPanel.DAL.Data;
using CitizenPanel.DAL.QuestionnaireModule;
using CitizenPanel.DAL.Registration;
using CitizenPanel.UI.MVC;
using CitizenPanel.UI.MVC.Areas.Identity.DutchLocalization;
using CitizenPanel.UI.MVC.Areas.Identity.Managers;
using CitizenPanel.UI.MVC.Areas.Identity.Services;
using CitizenPanel.UI.MVC.Middleware;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext with PostgreSQL
builder.Services.AddDbContext<PanelDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Repositories and Managers
builder.Services.AddScoped<IDrawRepository, DrawRepository>();
builder.Services.AddScoped<IPanelRepository, PanelRepository>();
builder.Services.AddScoped<IRegistrationRepository, RegistrationRepository>();
builder.Services.AddScoped<IQuestionnaireModuleRepository, QuestionnaireModuleRepository>();
builder.Services.AddScoped<IDrawManager, DrawManager>();
builder.Services.AddScoped<IPanelManager, PanelManager>();
builder.Services.AddScoped<IQuestionnaireModuleManager, QuestionnaireModuleManager>();
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
// builder.Services.AddScoped<IRegistrationManager, RegistrationManager>();
builder.Services.AddScoped<IMeetingManager, MeetingManager>();
builder.Services.AddScoped<IMeetingRepository, MeetingRepository>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<IMemberManager, MemberManager>();
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

builder.Services.Configure<AvailableTenants>(
    builder.Configuration.GetSection(AvailableTenants.SectionName)
    );

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
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAndMapLiveMonitoring();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<TenantMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();