﻿// This file is no longer needed for the prototype phase
// Database functionality has been removed for prototype demonstration

/*
using Microsoft.EntityFrameworkCore;

namespace contract_monthly_claim_system_cs.Models.DataModels
{
    /// <summary>
    /// Database context for the Claim System
    /// </summary>
    public class ClaimSystemContext : DbContext
    {
        public ClaimSystemContext(DbContextOptions<ClaimSystemContext> options)
            : base(options)
        {
        }

        public DbSet<Lecturer> Lecturers { get; set; } = null!;
        public DbSet<Claim> Claims { get; set; } = null!;
        public DbSet<Document> Documents { get; set; } = null!;
        public DbSet<Approval> Approvals { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationships
            modelBuilder.Entity<Claim>()
                .HasOne(c => c.Lecturer)
                .WithMany(l => l.Claims)
                .HasForeignKey(c => c.LecturerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Document>()
                .HasOne(d => d.Claim)
                .WithMany(c => c.Documents)
                .HasForeignKey(d => d.ClaimId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Approval>()
                .HasOne(a => a.Claim)
                .WithMany(c => c.Approvals)
                .HasForeignKey(a => a.ClaimId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed initial data for prototype
            modelBuilder.Entity<Lecturer>().HasData(
                new Lecturer
                {
                    LecturerId = 1,
                    FirstName = "John",
                    LastName = "Smith",
                    Email = "john.smith@example.com",
                    HourlyRate = 150.00m
                });
        }
    }
}
*/