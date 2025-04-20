using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.Panel;
using CitizenPanel.BL.Domain.QuestionnaireModule;
using CitizenPanel.BL.Domain.User;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CitizenPanel.DAL.Data;

public class PanelDbContext : IdentityDbContext
{
    private readonly IConfiguration _configuration;
    public DbSet<Member> Members { get; set; }
    public DbSet<Panel> Panels { get; set; }
    public DbSet<Recommendation> Recommendations { get; set; }
    public DbSet<UserVote> UserVotes { get; set; } 
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<ExtraCriteria> ExtraCriteria { get; set; }
    public DbSet<SubCriteria> SubCriteria { get; set; }
    public DbSet<Invitation> Invitations { get; set; }
    public DbSet<DrawResult> DrawResults { get; set; }
    public DbSet<Questionnaire> Questionnaires { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Answer> Answers { get; set; }
        
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
        
        modelBuilder.Entity<Member>().ToTable("Members");
        modelBuilder.Entity<Organization>().ToTable("Organizations");

        modelBuilder.Entity<DrawResult>()
            .HasMany(m => m.SelectedMembers);
        modelBuilder.Entity<DrawResult>()
            .HasMany(m => m.ReserveMembers);
        modelBuilder.Entity<Panel>()
            .OwnsMany(p => p.RecruitmentBuckets);
        modelBuilder.Entity<Panel>()
            .HasMany(m => m.Members);
        modelBuilder.Entity<Member>()
            .HasOne(m => m.Panel); //momenteel gewoon 1 ik weet dat het meerdere kan bevatten
        
        modelBuilder.Entity<Member>()
            .HasMany(m => m.SelectedCriteria)
            .WithMany()
            .UsingEntity(j => j.ToTable("MemberSelectedCriteria"));
        
        modelBuilder.Entity<ExtraCriteria>()
            .HasMany(e => e.SubCriteria)
            .WithMany()
            .UsingEntity(j => j.ToTable("ExtraSubCriteria"));

        modelBuilder.Entity<Panel>()
            .HasMany(p => p.ExtraCriteria)
            .WithOne(e => e.Panel);

        modelBuilder.Entity<UserVote>()
            .HasOne(uv => uv.Recommendation)
            .WithMany(r => r.UserVotes)
            .HasForeignKey(uv => uv.RecommendationId);

        modelBuilder.Entity<UserVote>()
            .HasOne(iu => iu.Voter);

        modelBuilder.Entity<Questionnaire>()
            .HasMany(q => q.Questions)
            .WithOne(q => q.Questionnaire);
        modelBuilder.Entity<Question>()
            .HasMany(q => q.Answers)
            .WithOne(a => a.Question);

    }
    
    public bool CreateDatabase(bool delete) {
        if (delete) {
            Database.EnsureDeleted();
        }

        return Database.EnsureCreated();
    }
}