using Microsoft.EntityFrameworkCore;

namespace contract_monthly_claim_system_cs.Models.DataModels
{
    public class CMCSDbContext : DbContext
    {
        public CMCSDbContext(DbContextOptions<CMCSDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Lecturer> Lecturers { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Approval> Approvals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Surname).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Role).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(255);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
            });

            // Lecturer configuration
            modelBuilder.Entity<Lecturer>(entity =>
            {
                entity.HasKey(e => e.LecturerId);
                entity.Property(e => e.EmployeeNumber).HasMaxLength(20);
                entity.Property(e => e.Department).HasMaxLength(100);
                entity.Property(e => e.HourlyRate).HasColumnType("decimal(10,2)");
                entity.Property(e => e.BankAccountNumber).HasMaxLength(50);
                entity.Property(e => e.BankName).HasMaxLength(100);
                entity.Property(e => e.TaxNumber).HasMaxLength(50);

                entity.HasOne(e => e.User)
                      .WithOne()
                      .HasForeignKey<Lecturer>(e => e.LecturerId);
            });

            // Claim configuration
            modelBuilder.Entity<Claim>(entity =>
            {
                entity.HasKey(e => e.ClaimId);
                entity.Property(e => e.MonthYear).IsRequired().HasMaxLength(7);
                entity.Property(e => e.HoursWorked).HasColumnType("decimal(6,2)");
                entity.Property(e => e.HourlyRate).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Amount).HasColumnType("decimal(12,2)");
                entity.Property(e => e.Status).HasDefaultValue(ClaimStatus.Submitted);
                entity.Property(e => e.SubmissionComments).HasMaxLength(500);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.ModifiedDate).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(e => e.Lecturer)
                      .WithMany(l => l.Claims)
                      .HasForeignKey(e => e.LecturerId);
            });

            // Document configuration
            modelBuilder.Entity<Document>(entity =>
            {
                entity.HasKey(e => e.DocumentId);
                entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
                entity.Property(e => e.FileType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.UploadDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                entity.HasOne(e => e.Claim)
                      .WithMany(c => c.Documents)
                      .HasForeignKey(e => e.ClaimId);
            });

            // Approval configuration
            modelBuilder.Entity<Approval>(entity =>
            {
                entity.HasKey(e => e.ApprovalId);
                entity.Property(e => e.Comments).HasMaxLength(500);
                entity.Property(e => e.ApprovalDate).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(e => e.Claim)
                      .WithMany(c => c.Approvals)
                      .HasForeignKey(e => e.ClaimId);

                entity.HasOne(e => e.ApproverUser)
                      .WithMany()
                      .HasForeignKey(e => e.ApproverUserId);
            });
        }
    }
}