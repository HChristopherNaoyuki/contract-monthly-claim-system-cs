using Microsoft.EntityFrameworkCore;
using contract_monthly_claim_system_cs.Models.DataModels;
using System;
using System.Threading.Tasks;

namespace contract_monthly_claim_system_cs.Services
{
    /// <summary>
    /// Service for managing database connections and operations
    /// </summary>
    public class DatabaseService
    {
        private readonly CMCSDbContext _context;

        /// <summary>
        /// Initializes a new instance of the DatabaseService class
        /// </summary>
        /// <param name="context">The database context</param>
        public DatabaseService(CMCSDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Tests the database connection
        /// </summary>
        /// <returns>True if connection is successful, false otherwise</returns>
        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                return await _context.TestConnectionAsync();
            }
            catch (Exception ex)
            {
                // Log the exception (in production, use proper logging)
                System.Diagnostics.Debug.WriteLine($"Database connection test failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Ensures the database is created and migrations are applied
        /// </summary>
        /// <returns>True if successful, false otherwise</returns>
        public async Task<bool> EnsureDatabaseCreatedAsync()
        {
            try
            {
                await _context.Database.EnsureCreatedAsync();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database creation failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Executes a database health check
        /// </summary>
        /// <returns>Health check result with status and message</returns>
        public async Task<DatabaseHealthResult> HealthCheckAsync()
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync();

                if (canConnect)
                {
                    // Test basic query execution
                    var userCount = await _context.Users.CountAsync();

                    return new DatabaseHealthResult
                    {
                        IsHealthy = true,
                        Message = "Database connection is healthy",
                        UserCount = userCount
                    };
                }
                else
                {
                    return new DatabaseHealthResult
                    {
                        IsHealthy = false,
                        Message = "Cannot connect to database",
                        UserCount = 0
                    };
                }
            }
            catch (Exception ex)
            {
                return new DatabaseHealthResult
                {
                    IsHealthy = false,
                    Message = $"Database health check failed: {ex.Message}",
                    UserCount = 0
                };
            }
        }
    }

    /// <summary>
    /// Represents the result of a database health check
    /// </summary>
    public class DatabaseHealthResult
    {
        /// <summary>
        /// Gets or sets whether the database is healthy
        /// </summary>
        public bool IsHealthy { get; set; }

        /// <summary>
        /// Gets or sets the health check message
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the number of users in the database
        /// </summary>
        public int UserCount { get; set; }
    }
}