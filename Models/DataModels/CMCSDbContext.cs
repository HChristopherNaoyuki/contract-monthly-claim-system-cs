using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace contract_monthly_claim_system_cs.Models.DataModels
{
    /// <summary>
    /// Database context for the Contract Monthly Claim System
    /// Manages database connections and entity configurations
    /// </summary>
    public class CMCSDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the CMCSDbContext class
        /// </summary>
        /// <param name="options">The options to be used by the DbContext</param>
        public CMCSDbContext(DbContextOptions<CMCSDbContext> options) : base(options)
        {
        }

        // DbSet properties for each entity
        public DbSet<User> Users { get; set; }
        public DbSet<Lecturer> Lecturers { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Approval> Approvals { get; set; }

        /// <summary>
        /// Configures the model that was discovered by convention from the entity types
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User entity configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.Surname)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(20);
                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(255);
                entity.Property(e => e.Role)
                    .IsRequired();
                entity.Property(e => e.Email)
                    .HasMaxLength(255);
                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(20);
                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Index for performance
                entity.HasIndex(e => e.Username)
                    .IsUnique();
                entity.HasIndex(e => e.Role);
                entity.HasIndex(e => e.IsActive);
            });

            // Lecturer entity configuration
            modelBuilder.Entity<Lecturer>(entity =>
            {
                entity.HasKey(e => e.LecturerId);
                entity.Property(e => e.EmployeeNumber)
                    .HasMaxLength(20);
                entity.Property(e => e.Department)
                    .HasMaxLength(100);
                entity.Property(e => e.HourlyRate)
                    .HasColumnType("decimal(10,2)");
                entity.Property(e => e.BankAccountNumber)
                    .HasMaxLength(50);
                entity.Property(e => e.BankName)
                    .HasMaxLength(100);
                entity.Property(e => e.TaxNumber)
                    .HasMaxLength(50);

                // One-to-one relationship with User
                entity.HasOne(e => e.User)
                      .WithOne()
                      .HasForeignKey<Lecturer>(e => e.LecturerId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Index for performance
                entity.HasIndex(e => e.EmployeeNumber)
                    .IsUnique();
            });

            // Claim entity configuration
            modelBuilder.Entity<Claim>(entity =>
            {
                entity.HasKey(e => e.ClaimId);
                entity.Property(e => e.MonthYear)
                    .IsRequired()
                    .HasMaxLength(7);
                entity.Property(e => e.HoursWorked)
                    .HasColumnType("decimal(6,2)");
                entity.Property(e => e.HourlyRate)
                    .HasColumnType("decimal(10,2)");
                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(12,2)");
                entity.Property(e => e.Status)
                    .HasDefaultValue(ClaimStatus.Submitted);
                entity.Property(e => e.SubmissionComments)
                    .HasMaxLength(500);
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.ModifiedDate)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Many-to-one relationship with Lecturer
                entity.HasOne(e => e.Lecturer)
                      .WithMany(l => l.Claims)
                      .HasForeignKey(e => e.LecturerId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Indexes for performance
                entity.HasIndex(e => e.LecturerId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.MonthYear);
                entity.HasIndex(e => e.CreatedDate);
            });

            // Document entity configuration
            modelBuilder.Entity<Document>(entity =>
            {
                entity.HasKey(e => e.DocumentId);
                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasMaxLength(255);
                entity.Property(e => e.FilePath)
                    .IsRequired()
                    .HasMaxLength(500);
                entity.Property(e => e.FileType)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.UploadDate)
                    .HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                // Many-to-one relationship with Claim
                entity.HasOne(e => e.Claim)
                      .WithMany(c => c.Documents)
                      .HasForeignKey(e => e.ClaimId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Index for performance
                entity.HasIndex(e => e.ClaimId);
            });

            // Approval entity configuration
            modelBuilder.Entity<Approval>(entity =>
            {
                entity.HasKey(e => e.ApprovalId);
                entity.Property(e => e.Comments)
                    .HasMaxLength(500);
                entity.Property(e => e.ApprovalDate)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Many-to-one relationship with Claim
                entity.HasOne(e => e.Claim)
                      .WithMany(c => c.Approvals)
                      .HasForeignKey(e => e.ClaimId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Many-to-one relationship with User (Approver)
                entity.HasOne(e => e.ApproverUser)
                      .WithMany()
                      .HasForeignKey(e => e.ApproverUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Indexes for performance
                entity.HasIndex(e => e.ClaimId);
                entity.HasIndex(e => e.ApproverUserId);
            });

            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Saves all changes made in this context to the database
        /// </summary>
        /// <returns>The number of state entries written to the database</returns>
        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        /// <summary>
        /// Saves all changes made in this context to the database asynchronously
        /// </summary>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete</param>
        /// <returns>A task that represents the asynchronous save operation</returns>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Updates created and modified timestamps for entities
        /// </summary>
        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries();
            var currentTime = DateTime.UtcNow;

            foreach (var entry in entries)
            {
                if (entry.Entity is Claim claim)
                {
                    if (entry.State == EntityState.Added)
                    {
                        claim.CreatedDate = currentTime;
                        claim.ModifiedDate = currentTime;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        claim.ModifiedDate = currentTime;
                    }
                }
            }
        }

        /// <summary>
        /// Tests the database connection
        /// </summary>
        /// <returns>True if connection is successful, false otherwise</returns>
        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                return await Database.CanConnectAsync();
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}