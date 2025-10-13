using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using contract_monthly_claim_system_cs.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace contract_monthly_claim_system_cs.Controllers
{
    /// <summary>
    /// Controller for testing database connection and health
    /// Enhanced with comprehensive diagnostics
    /// </summary>
    public class TestController : Controller
    {
        private readonly DatabaseService _databaseService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TestController> _logger;

        /// <summary>
        /// Initializes a new instance of the TestController class
        /// </summary>
        /// <param name="databaseService">The database service</param>
        /// <param name="configuration">The configuration</param>
        /// <param name="logger">Logger instance</param>
        public TestController(DatabaseService databaseService, IConfiguration configuration, ILogger<TestController> logger)
        {
            _databaseService = databaseService;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Tests the database connection with comprehensive diagnostics
        /// </summary>
        /// <returns>JSON result with connection status and diagnostics</returns>
        [HttpGet]
        public async Task<IActionResult> TestConnection()
        {
            _logger.LogInformation("Database connection test requested.");

            var connectionString = _configuration.GetConnectionString("CMCSDatabase");
            var healthResult = await _databaseService.HealthCheckAsync();
            var connectionInfo = _databaseService.GetConnectionInfo();

            var result = new
            {
                ConnectionString = GetMaskedConnectionString(connectionString),
                ConnectionInfo = connectionInfo,
                HealthCheck = new
                {
                    healthResult.IsHealthy,
                    healthResult.CanConnect,
                    healthResult.WriteOperationTest,
                    healthResult.Message,
                    healthResult.UserCount,
                    healthResult.Timestamp
                },
                AvailableConnections = new
                {
                    HasCMCSDatabase = !string.IsNullOrEmpty(_configuration.GetConnectionString("CMCSDatabase")),
                    HasLocalDB = !string.IsNullOrEmpty(_configuration.GetConnectionString("LocalDB")),
                    HasSQLite = !string.IsNullOrEmpty(_configuration.GetConnectionString("SQLite"))
                },
                Timestamp = System.DateTime.UtcNow
            };

            _logger.LogInformation("Database connection test completed: {IsHealthy}", healthResult.IsHealthy);

            return Json(result);
        }

        /// <summary>
        /// Displays database connection information
        /// </summary>
        /// <returns>View with connection details</returns>
        [HttpGet]
        public async Task<IActionResult> DatabaseInfo()
        {
            var healthResult = await _databaseService.HealthCheckAsync();

            ViewBag.HealthResult = healthResult;
            ViewBag.ConnectionString = GetMaskedConnectionString(_configuration.GetConnectionString("CMCSDatabase"));
            ViewBag.ConnectionInfo = _databaseService.GetConnectionInfo();
            ViewBag.DatabaseName = "CMCS_Database";
            ViewBag.AvailableConnections = new
            {
                CMCSDatabase = !string.IsNullOrEmpty(_configuration.GetConnectionString("CMCSDatabase")),
                LocalDB = !string.IsNullOrEmpty(_configuration.GetConnectionString("LocalDB")),
                SQLite = !string.IsNullOrEmpty(_configuration.GetConnectionString("SQLite"))
            };

            return View();
        }

        /// <summary>
        /// Provides database setup instructions
        /// </summary>
        /// <returns>View with setup instructions</returns>
        [HttpGet]
        public IActionResult DatabaseSetup()
        {
            return View();
        }

        /// <summary>
        /// Masks sensitive information in connection string for security
        /// </summary>
        /// <param name="connectionString">Original connection string</param>
        /// <returns>Masked connection string</returns>
        private string GetMaskedConnectionString(string? connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                return "Not configured";
            }

            // Mask password and sensitive data
            var masked = connectionString
                .Replace("Password=.*;", "Password=******;")
                .Replace("User Id=.*;", "User Id=******;")
                .Replace("UID=.*;", "UID=******;")
                .Replace("PWD=.*;", "PWD=******;");

            return masked.Length > 100 ? masked.Substring(0, 100) + "..." : masked;
        }
    }
}