using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CitizenPanel.DAL;

using BL.Domain.User;

public class PanelDbContext : IdentityDbContext
{
    private readonly IConfiguration _configuration;
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Citizen> Citizens { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<Panelmember> Panelmembers { get; set; }
    
    public PanelDbContext(DbContextOptions<PanelDbContext> options, IConfiguration configuration) : base(options)
    {
        _configuration = configuration;
    }
    
    override protected void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        if (!optionsBuilder.IsConfigured) {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseNpgsql(connectionString);
        }
        optionsBuilder.LogTo(message => System.Diagnostics.Debug.WriteLine(message), LogLevel.Information);
    }
    
    override protected void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
    
    public bool CreateDatabase(bool delete) {
        if (delete) {
            Database.EnsureDeleted();
        }

        return Database.EnsureCreated();
    }
}