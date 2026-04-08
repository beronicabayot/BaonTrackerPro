using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using BaonTrackerPro.Models;

namespace BaonTrackerPro.Data
{
    public class ApplicationDbContext : DbContext
    {
        private static readonly ValueConverter<DateTime, DateTime> UtcDateTimeConverter =
            new(
                toDb => toDb.Kind == DateTimeKind.Utc ? toDb : DateTime.SpecifyKind(toDb, DateTimeKind.Utc),
                fromDb => DateTime.SpecifyKind(fromDb, DateTimeKind.Utc));

        private static readonly ValueConverter<DateTime?, DateTime?> NullableUtcDateTimeConverter =
            new(
                toDb => toDb.HasValue
                    ? (toDb.Value.Kind == DateTimeKind.Utc
                        ? toDb
                        : DateTime.SpecifyKind(toDb.Value, DateTimeKind.Utc))
                    : toDb,
                fromDb => fromDb.HasValue
                    ? DateTime.SpecifyKind(fromDb.Value, DateTimeKind.Utc)
                    : fromDb);

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<BudgetItem> BudgetItems { get; set; }
        public DbSet<SavingsGoal> SavingsGoals { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<AppUser>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.AppUser)
                .WithMany()
                .HasForeignKey(t => t.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SavingsGoal>()
                .HasOne(g => g.AppUser)
                .WithMany()
                .HasForeignKey(g => g.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BudgetItem>()
                .HasOne(b => b.AppUser)
                .WithMany()
                .HasForeignKey(b => b.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(UtcDateTimeConverter);
                    }
                    else if (property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(NullableUtcDateTimeConverter);
                    }
                }
            }
        }
    }
}