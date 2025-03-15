using CitizenPanel.BL.Domain.PanelManagement;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CitizenPanel.DAL;

using BL.Domain.User;

public class PanelDbContext : IdentityDbContext
{
    private readonly IConfiguration _configuration;
    public DbSet<Member> Members { get; set; }
    public DbSet<Panel> Panels { get; set; }
        
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
        
        modelBuilder.Entity<Panel>()
            .HasMany(m => m.Members);
        modelBuilder.Entity<Member>()
            .HasOne(m => m.Panel); //momenteel gwn 1 ik weet dat het meerdere kan bevatten
    }
    
    public bool CreateDatabase(bool delete) {
        if (delete) {
            Database.EnsureDeleted();
        }

        return Database.EnsureCreated();
    }
}