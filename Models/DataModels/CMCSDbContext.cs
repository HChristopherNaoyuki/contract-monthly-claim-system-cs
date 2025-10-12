using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace contract_monthly_claim_system_cs.Models.DataModels
{
    /// <summary>
    /// Database context for the Contract Monthly Claim System
    /// Uses Entity Framework Core for database operations
    /// </summary>
    public class CMCSDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the database context
        /// </summary>
        /// <param name="options">Database context options</param>
        public CMCSDbContext(DbContextOptions<CMCSDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the Users table for system users
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Gets or sets the Lecturers table for lecturer-specific information
        /// </summary>
        public DbSet<Lecturer> Lecturers { get; set; }

        /// <summary>
        /// Gets or sets the Claims table for monthly claim submissions
        /// </summary>
        public DbSet<Claim> Claims { get; set; }

        /// <summary>
        /// Gets or sets the Documents table for supporting documents
        /// </summary>
        public DbSet<Document> Documents { get; set; }

        /// <summary>
        /// Gets or sets the Approvals table for claim approval workflow
        /// </summary>
        public DbSet<Approval> Approvals { get; set; }

        /// <summary>
        /// Configures the database model and relationships
        /// </summary>
        /// <param name="modelBuilder">Model builder instance</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure User entity
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
                entity.HasIndex(e => e.Username)
                    .IsUnique();
            });

            // Configure Lecturer entity
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
                entity.HasOne(e => e.User)
                      .WithOne()
                      .HasForeignKey<Lecturer>(e => e.LecturerId);
                entity.HasIndex(e => e.EmployeeNumber)
                    .IsUnique();
            });

            // Configure Claim entity
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
                entity.Property(e => e.ClaimDate)
                    .HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.ModifiedDate)
                    .HasDefaultValueSql("GETDATE()");
                entity.HasOne(e => e.Lecturer)
                      .WithMany(l => l.Claims)
                      .HasForeignKey(e => e.LecturerId);
                entity.HasIndex(e => e.LecturerId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.MonthYear);
            });

            // Configure Document entity
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
                    .HasDefaultValueSql("GETDATE()");
                entity.HasOne(e => e.Claim)
                      .WithMany(c => c.Documents)
                      .HasForeignKey(e => e.ClaimId);
                entity.HasIndex(e => e.ClaimId);
            });

            // Configure Approval entity
            modelBuilder.Entity<Approval>(entity =>
            {
                entity.HasKey(e => e.ApprovalId);
                entity.Property(e => e.ApproverRole)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.Comments)
                    .HasMaxLength(500);
                entity.Property(e => e.ApprovalDate)
                    .HasDefaultValueSql("GETDATE()");
                entity.HasOne(e => e.Claim)
                      .WithMany(c => c.Approvals)
                      .HasForeignKey(e => e.ClaimId);
                entity.HasOne(e => e.ApproverUser)
                      .WithMany()
                      .HasForeignKey(e => e.ApproverUserId);
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
            var currentTime = DateTime.Now;

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