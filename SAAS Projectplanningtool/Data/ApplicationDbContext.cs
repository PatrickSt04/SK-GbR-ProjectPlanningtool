using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using SAAS_Projectplanningtool.Models.Budgetplanning;
using SAAS_Projectplanningtool.Models;


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
        public DbSet<SAAS_Projectplanningtool.Models.Budgetplanning.ProjectAdditionalCosts> ProjectAdditionalCosts { get; set; } = default!;
        public DbSet<SAAS_Projectplanningtool.Models.Budgetplanning.ProjectTaskHourlyRateGroup> ProjectTaskHourlyRateGroup { get; set; } = default!;
        public DbSet<SAAS_Projectplanningtool.Models.Budgetplanning.BudgetRecalculation> BudgetRecalculation { get; set; } = default!;
        public DbSet<SAAS_Projectplanningtool.Models.Budgetplanning.ProjectTaskFixCosts> ProjectTaskFixCosts { get; set; } = default!;

        public DbSet<SAAS_Projectplanningtool.Models.Budgetplanning.ProjectSectionMilestone> ProjectSectionMilestone { get; set; } = default!;
        public DbSet<SAAS_Projectplanningtool.Models.Budgetplanning.ProjectTaskCatalogTask> ProjectTaskCatalogTask { get; set; } = default!;


        public DbSet<SAAS_Projectplanningtool.Models.Logfile> Logfile { get; set; } = default!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Global configuration: disable cascade delete for all relationships.
            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.NoAction;

            }

            //NOTWENDIG:
            modelBuilder.Entity<ProjectBudget>(entity =>
            {
                entity.OwnsMany(pb => pb.InitialHRGPlannings, hrg =>
                {
                    hrg.ToJson(); // Speichert als JSON in einer Spalte
                });

                entity.OwnsMany(pb => pb.InitialAdditionalCosts, cost =>
                {
                    cost.ToJson(); // Speichert als JSON in einer Spalte
                });
            });

            //NOTWENDIG:
            modelBuilder.Entity<ProjectTaskFixCosts>(entity =>
            {
                entity.OwnsMany(ptfc => ptfc.FixCosts, fc =>
                {
                    fc.ToJson(); // Speichert als JSON in einer Spalte
                });
            });


            // Configure the self-referencing relationship for ProjectSection:
            modelBuilder.Entity<ProjectSection>()
                .HasOne(ps => ps.ParentSection)
                .WithMany(ps => ps.SubSections)
                .HasForeignKey(ps => ps.ParentSectionId)
                .OnDelete(DeleteBehavior.NoAction);

            // Every Table has a Employee ( LatestModifier ) and a Employee ( CreatedBy )
            // For the Employee instance itself we have to handle this differently
            modelBuilder.Entity<Employee>()
            .HasOne(e => e.CreatedByEmployee)
            .WithMany()
            .HasForeignKey(e => e.CreatedById)
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.LatestModifier)
                .WithMany()
                .HasForeignKey(e => e.LatestModifierId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete


            // Manuelles Mapping für jede der Entitäten, die auf Employee verweisen
            modelBuilder.Entity<Project>()
                .HasOne(p => p.CreatedByEmployee)
                .WithMany()
                .HasForeignKey(p => p.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);  // Verhindern der Kaskadenlöschung

            modelBuilder.Entity<Project>()
                .HasOne(p => p.LatestModifier)
                .WithMany()
                .HasForeignKey(p => p.LatestModifierId)
                .OnDelete(DeleteBehavior.Restrict);  // Verhindern der Kaskadenlöschung

            modelBuilder.Entity<ProjectBudget>()
                .HasOne(pb => pb.CreatedByEmployee)
                .WithMany()
                .HasForeignKey(pb => pb.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectBudget>()
                .HasOne(pb => pb.LatestModifier)
                .WithMany()
                .HasForeignKey(pb => pb.LatestModifierId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectSection>()
                .HasOne(ps => ps.CreatedByEmployee)
                .WithMany()
                .HasForeignKey(ps => ps.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectSection>()
                .HasOne(ps => ps.LatestModifier)
                .WithMany()
                .HasForeignKey(ps => ps.LatestModifierId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectTask>()
                .HasOne(pt => pt.CreatedByEmployee)
                .WithMany()
                .HasForeignKey(pt => pt.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectTask>()
                .HasOne(pt => pt.LatestModifier)
                .WithMany()
                .HasForeignKey(pt => pt.LatestModifierId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Address>()
                .HasOne(a => a.CreatedByEmployee)
                .WithMany()
                .HasForeignKey(a => a.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Address>()
                .HasOne(a => a.LatestModifier)
                .WithMany()
                .HasForeignKey(a => a.LatestModifierId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Company>()
                .HasOne(c => c.CreatedByEmployee)
                .WithMany()
                .HasForeignKey(c => c.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Company>()
                .HasOne(c => c.LatestModifier)
                .WithMany()
                .HasForeignKey(c => c.LatestModifierId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Customer>()
                .HasOne(cu => cu.CreatedByEmployee)
                .WithMany()
                .HasForeignKey(cu => cu.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Customer>()
                .HasOne(cu => cu.LatestModifier)
                .WithMany()
                .HasForeignKey(cu => cu.LatestModifierId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HourlyRateGroup>()
                .HasOne(hrg => hrg.CreatedByEmployee)
                .WithMany()
                .HasForeignKey(hrg => hrg.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HourlyRateGroup>()
                .HasOne(hrg => hrg.LatestModifier)
                .WithMany()
                .HasForeignKey(hrg => hrg.LatestModifierId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
