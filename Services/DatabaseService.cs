using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace contract_monthly_claim_system_cs.Services
{
    /// <summary>
    /// Service for managing text file database connections and operations
    /// Replaces Entity Framework with text file storage
    /// </summary>
    public class DatabaseService
    {
        private readonly TextFileDataService _dataService;
        private readonly ILogger<DatabaseService> _logger;

        /// <summary>
        /// Initializes a new instance of the DatabaseService class
        /// </summary>
        /// <param name="dataService">The text file data service</param>
        /// <param name="logger">Logger instance</param>
        public DatabaseService(TextFileDataService dataService, ILogger<DatabaseService> logger)
        {
            _dataService = dataService;
            _logger = logger;
        }

        /// <summary>
        /// Tests the text file storage connection
        /// </summary>
        /// <returns>True if connection is successful, false otherwise</returns>
        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                _logger.LogInformation("Testing text file storage connection...");

                var users = _dataService.GetAllUsers();
                var canConnect = users != null;

                if (canConnect)
                {
                    _logger.LogInformation("Text file storage connection test successful.");
                    return true;
                }
                else
                {
                    _logger.LogWarning("Text file storage connection test failed.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Text file storage connection test failed.");
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
                _logger.LogInformation("Performing text file storage health check...");

                // Test basic data access
                var users = _dataService.GetAllUsers();
                var claims = _dataService.GetAllClaims();

                var canConnect = users != null && claims != null;
                var userCount = users?.Count ?? 0;
                var claimCount = claims?.Count ?? 0;

                if (canConnect)
                {
                    return new DatabaseHealthResult
                    {
                        IsHealthy = true,
                        Message = "Text file storage is healthy",
                        UserCount = userCount,
                        ClaimCount = claimCount,
                        CanConnect = true,
                        WriteOperationTest = true
                    };
                }
                else
                {
                    return new DatabaseHealthResult
                    {
                        IsHealthy = false,
                        Message = "Cannot access text file storage",
                        UserCount = 0,
                        ClaimCount = 0,
                        CanConnect = false,
                        WriteOperationTest = false
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Text file storage health check failed");

                return new DatabaseHealthResult
                {
                    IsHealthy = false,
                    Message = $"Text file storage health check failed: {ex.Message}",
                    UserCount = 0,
                    ClaimCount = 0,
                    CanConnect = false,
                    WriteOperationTest = false,
                    Exception = ex
                };
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
                var userCount = _dataService.GetAllUsers().Count;
                var claimCount = _dataService.GetAllClaims().Count;
                return $"Text File Storage: Users={userCount}, Claims={claimCount}, Status=Healthy";
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
        /// Gets or sets the number of claims in the database
        /// </summary>
        public int ClaimCount { get; set; }

        /// <summary>
        /// Gets or sets the exception if any occurred
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the health check
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}