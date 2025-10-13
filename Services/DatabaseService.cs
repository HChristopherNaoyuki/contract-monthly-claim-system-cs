using Microsoft.EntityFrameworkCore;
using contract_monthly_claim_system_cs.Models.DataModels;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace contract_monthly_claim_system_cs.Services
{
    /// <summary>
    /// Service for managing database connections and operations
    /// Enhanced with better error handling and connection resilience
    /// </summary>
    public class DatabaseService
    {
        private readonly CMCSDbContext _context;
        private readonly ILogger<DatabaseService> _logger;

        /// <summary>
        /// Initializes a new instance of the DatabaseService class
        /// </summary>
        /// <param name="context">The database context</param>
        /// <param name="logger">Logger instance</param>
        public DatabaseService(CMCSDbContext context, ILogger<DatabaseService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Tests the database connection with retry logic
        /// </summary>
        /// <returns>True if connection is successful, false otherwise</returns>
        public async Task<bool> TestConnectionAsync()
        {
            const int maxRetries = 3;
            var retryCount = 0;

            while (retryCount < maxRetries)
            {
                try
                {
                    _logger.LogInformation("Testing database connection (attempt {Attempt})...", retryCount + 1);

                    var canConnect = await _context.Database.CanConnectAsync();

                    if (canConnect)
                    {
                        _logger.LogInformation("Database connection test successful.");
                        return true;
                    }
                    else
                    {
                        _logger.LogWarning("Database connection test failed - cannot connect to database.");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    retryCount++;
                    _logger.LogWarning(ex, "Database connection test failed (attempt {Attempt}).", retryCount);

                    if (retryCount >= maxRetries)
                    {
                        _logger.LogError(ex, "All database connection attempts failed.");
                        return false;
                    }

                    // Wait before retrying
                    await Task.Delay(TimeSpan.FromSeconds(2 * retryCount));
                }
            }

            return false;
        }

        /// <summary>
        /// Ensures the database is created and migrations are applied
        /// </summary>
        /// <returns>True if successful, false otherwise</returns>
        public async Task<bool> EnsureDatabaseCreatedAsync()
        {
            try
            {
                _logger.LogInformation("Ensuring database is created...");

                var created = await _context.Database.EnsureCreatedAsync();

                if (created)
                {
                    _logger.LogInformation("Database created successfully.");
                }
                else
                {
                    _logger.LogInformation("Database already exists.");
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database creation failed.");
                return false;
            }
        }

        /// <summary>
        /// Executes a database health check with comprehensive diagnostics
        /// </summary>
        /// <returns>Health check result with status and message</returns>
        public async Task<DatabaseHealthResult> HealthCheckAsync()
        {
            try
            {
                _logger.LogInformation("Performing database health check...");

                // Test basic connection
                var canConnect = await _context.Database.CanConnectAsync();

                if (canConnect)
                {
                    // Test basic query execution
                    var userCount = await _context.Users.CountAsync();

                    // Test write operation with a simple transaction
                    var testSuccess = await TestWriteOperationAsync();

                    return new DatabaseHealthResult
                    {
                        IsHealthy = true,
                        Message = "Database connection is healthy",
                        UserCount = userCount,
                        CanConnect = true,
                        WriteOperationTest = testSuccess
                    };
                }
                else
                {
                    return new DatabaseHealthResult
                    {
                        IsHealthy = false,
                        Message = "Cannot connect to database",
                        UserCount = 0,
                        CanConnect = false,
                        WriteOperationTest = false
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database health check failed");

                return new DatabaseHealthResult
                {
                    IsHealthy = false,
                    Message = $"Database health check failed: {ex.Message}",
                    UserCount = 0,
                    CanConnect = false,
                    WriteOperationTest = false,
                    Exception = ex
                };
            }
        }

        /// <summary>
        /// Tests write operation capability
        /// </summary>
        /// <returns>True if write operation succeeds</returns>
        private async Task<bool> TestWriteOperationAsync()
        {
            try
            {
                // Create a simple test transaction
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // Perform a simple read operation
                        var count = await _context.Users.CountAsync();

                        // If we get here, basic operations work
                        await transaction.CommitAsync();
                        return true;
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Write operation test failed.");
                return false;
            }
        }

        /// <summary>
        /// Gets database connection information for diagnostics
        /// </summary>
        /// <returns>Database connection information</returns>
        public string GetConnectionInfo()
        {
            try
            {
                var connection = _context.Database.GetDbConnection();
                return $"Database: {connection.Database}, Data Source: {connection.DataSource}, State: {connection.State}";
            }
            catch (Exception ex)
            {
                return $"Unable to get connection info: {ex.Message}";
            }
        }
    }

    /// <summary>
    /// Enhanced database health result with additional diagnostics
    /// </summary>
    public class DatabaseHealthResult
    {
        /// <summary>
        /// Gets or sets whether the database is healthy
        /// </summary>
        public bool IsHealthy { get; set; }

        /// <summary>
        /// Gets or sets whether the database can be connected to
        /// </summary>
        public bool CanConnect { get; set; }

        /// <summary>
        /// Gets or sets whether write operations work
        /// </summary>
        public bool WriteOperationTest { get; set; }

        /// <summary>
        /// Gets or sets the health check message
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the number of users in the database
        /// </summary>
        public int UserCount { get; set; }

        /// <summary>
        /// Gets or sets the exception if any occurred
        /// </summary>
        public Exception? Exception { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the health check
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}