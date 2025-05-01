using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.Panel;
using CitizenPanel.BL.Domain.QuestionnaireModule;
using CitizenPanel.BL.Domain.Tenancy;
using CitizenPanel.BL.Domain.User;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace CitizenPanel.DAL.Data;

public class PanelDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly IConfiguration _configuration;
    private TenantContext _tenantContext;
    public DbSet<Panel> Panels { get; set; }
    public DbSet<Recommendation> Recommendations { get; set; }

    public DbSet<UserVote> UserVotes { get; set; } 
    public DbSet<Criteria> Criteria { get; set; }
    public DbSet<SubCriteria> SubCriteria { get; set; }
    public DbSet<Invitation> Invitations { get; set; }
    public DbSet<DrawResult> DrawResults { get; set; }
    public DbSet<Questionnaire> Questionnaires { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<MemberProfile> MemberProfiles { get; set; }
    public DbSet<OrganizationProfile> OrganizationProfiles { get; set; }
    
    public string TenantId => _tenantContext.GetCurrentTenantId();
    
    public PanelDbContext(DbContextOptions<PanelDbContext> options, IConfiguration configuration, TenantContext tenantContext) : base(options)
    {
        _configuration = configuration;
        _tenantContext = tenantContext;
        
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
        var tenantedModels = modelBuilder.Model.GetEntityTypes()
                .Where(entity => typeof(ITenanted).IsAssignableFrom(entity.ClrType))
            ;

        foreach (var tenantedModel in tenantedModels)
        {
            modelBuilder.Entity(tenantedModel.ClrType)
                .HasQueryFilter<ITenanted>(e => e.TenantId == TenantId)
                .HasIndex(nameof(ITenanted.TenantId))
                ;
            modelBuilder.Entity(tenantedModel.ClrType)
                .Property(nameof(ITenanted.TenantId))
                .IsRequired()
                .HasValueGenerator<TenantIdValueGenerator>()
                ;
        }
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PanelDbContext).Assembly);
        
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>()
            .HasOne(u => u.MemberProfile)
            .WithOne(m => m.ApplicationUser)
            .HasForeignKey<MemberProfile>(m => m.ApplicationUserId);
    
        modelBuilder.Entity<ApplicationUser>()
            .HasOne(u => u.OrganizationProfile)
            .WithOne(o => o.ApplicationUser)
            .HasForeignKey<OrganizationProfile>(o => o.ApplicationUserId);

        modelBuilder.Entity<DrawResult>()
            .HasMany(dr => dr.SelectedMembers)
            .WithOne()
            .HasForeignKey("SelectedDrawResultId");

        modelBuilder.Entity<DrawResult>()
            .HasMany(dr => dr.ReserveMembers)
            .WithOne()
            .HasForeignKey("ReserveDrawResultId");

        modelBuilder.Entity<DrawResult>()
            .HasMany(dr => dr.NotSelectedMembers)
            .WithOne()
            .HasForeignKey("NotSelectedDrawResultId");
        
        
        modelBuilder.Entity<Panel>()
            .OwnsMany(p => p.RecruitmentBuckets);
        modelBuilder.Entity<Panel>()
            .HasMany(m => m.Members)
            .WithMany(m => m.Panels)
            .UsingEntity(p => p.ToTable("PanelMembers"));
        
        modelBuilder.Entity<MemberProfile>()
            .HasMany(m => m.SelectedCriteria)
            .WithMany()
            .UsingEntity(j => j.ToTable("MemberSelectedCriteria"));
        
        modelBuilder.Entity<Criteria>()
            .HasMany(e => e.SubCriteria)
            .WithMany()
            .UsingEntity(j => j.ToTable("ExtraSubCriteria"));

        modelBuilder.Entity<Panel>()
            .HasMany(p => p.Criteria)
            .WithOne(e => e.Panel);
        
            modelBuilder.Entity<Recommendation>()
                .HasMany<UserVote>(r => r.UserVotes)
                .WithOne(uv => uv.Recommendation)
                .HasForeignKey("RecommendationId");
            
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

public static class QueryFilterExtensions
{
    public static EntityTypeBuilder HasQueryFilter<TInterface>(this EntityTypeBuilder entityTypeBuilder,
        Expression<Func<TInterface, bool>> filterExpression)
    {
        var param = Expression.Parameter(entityTypeBuilder.Metadata.ClrType);
        var body = ReplacingExpressionVisitor.Replace(filterExpression.Parameters.Single(), param,
            filterExpression.Body);

        var lambdaExpression = Expression.Lambda(body, param);

        return entityTypeBuilder.HasQueryFilter(lambdaExpression);
    }
}

public class TenantIdValueGenerator : ValueGenerator<string>
{
    public override string Next(EntityEntry entry)
    {
        if (entry is { Entity: ITenanted, Context: PanelDbContext dbContext })
        {
            return dbContext.TenantId;
        }

        throw new InvalidOperationException("Could not generate a new TenantId");
    }

    public override bool GeneratesTemporaryValues { get; }
        = false;
}