using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using SAAS_Projectplanningtool.Models.Budgetplanning;


namespace SAAS_Projectplanningtool.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<SAAS_Projectplanningtool.Models.Company> Company { get; set; } = default!;
        public DbSet<SAAS_Projectplanningtool.Models.Address> Address { get; set; } = default!;
        public DbSet<SAAS_Projectplanningtool.Models.Customer> Customer { get; set; } = default!;
        public DbSet<SAAS_Projectplanningtool.Models.Employee> Employee { get; set; } = default!;
        public DbSet<SAAS_Projectplanningtool.Models.HourlyRateGroup> HourlyRateGroup { get; set; } = default!;
        public DbSet<SAAS_Projectplanningtool.Models.Budgetplanning.Project> Project { get; set; } = default!;
        public DbSet<SAAS_Projectplanningtool.Models.Budgetplanning.ProjectBudget> ProjectBudget { get; set; } = default!;
        public DbSet<SAAS_Projectplanningtool.Models.Budgetplanning.ProjectSection> ProjectSection { get; set; } = default!;
        public DbSet<SAAS_Projectplanningtool.Models.Budgetplanning.ProjectTask> ProjectTask { get; set; } = default!;
        public DbSet<SAAS_Projectplanningtool.Models.IndependentTables.IndustrySector> IndustrySector { get; set; } = default!;
        public DbSet<SAAS_Projectplanningtool.Models.IndependentTables.LicenseModel> LicenseModel { get; set; } = default!;
        public DbSet<SAAS_Projectplanningtool.Models.IndependentTables.State> State { get; set; } = default!;
        public DbSet<SAAS_Projectplanningtool.Models.Ressourceplanning.ProjectTaskRessource> ProjectTaskRessource { get; set; } = default!;
        public DbSet<SAAS_Projectplanningtool.Models.Ressourceplanning.Ressource> Ressource { get; set; } = default!;
        public DbSet<SAAS_Projectplanningtool.Models.Ressourceplanning.RessourceType> RessourceType { get; set; } = default!;
        public DbSet<SAAS_Projectplanningtool.Models.Ressourceplanning.Unit> Unit { get; set; } = default!;
        public DbSet<SAAS_Projectplanningtool.Models.Logfile> Logfile { get; set; } = default!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Global configuration: disable cascade delete for all relationships.
            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.NoAction;
            }

            // Configure the self-referencing relationship for ProjectSection:
            modelBuilder.Entity<ProjectSection>()
                .HasOne(ps => ps.ParentSection)
                .WithMany(ps => ps.SubSections)
                .HasForeignKey(ps => ps.ParentSectionId)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
