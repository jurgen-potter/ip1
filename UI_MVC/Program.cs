using AspNetCoreLiveMonitoring.Extensions;
using CitizenPanel.BL;
using CitizenPanel.BL.Registration;
using CitizenPanel.DAL;
using CitizenPanel.DAL.Data;
using CitizenPanel.DAL.Registration;
using CitizenPanel.UI.MVC;
using Microsoft.AspNetCore.Identity;
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
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
builder.Services.AddScoped<IDrawManager, DrawManager>();
builder.Services.AddScoped<IPanelManager, PanelManager>();
builder.Services.AddScoped<IQuestionManager, QuestionManager>();
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<IRegistrationManager, RegistrationManager>();
builder.Services.AddScoped<IMailSender, MailSender>();
builder.Services.AddScoped<IMemberManager, MemberManager>();
builder.Services.AddLiveMonitoring();
builder.Services.AddRazorPages();

// Add Identity
builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<PanelDbContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.User.RequireUniqueEmail = true;
});

var app = builder.Build();

app.MapRazorPages();

using (IServiceScope scope = app.Services.CreateScope()) {
    PanelDbContext context = scope.ServiceProvider.GetRequiredService<PanelDbContext>();
    if (context.CreateDatabase(true)) {
        
        var userManager = scope.ServiceProvider.GetService<UserManager<IdentityUser>>();
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();