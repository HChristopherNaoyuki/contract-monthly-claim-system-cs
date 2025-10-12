using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using contract_monthly_claim_system_cs.Services;
using System.Threading.Tasks;

namespace contract_monthly_claim_system_cs.Controllers
{
    /// <summary>
    /// Controller for testing database connection and health
    /// </summary>
    public class TestController : Controller
    {
        private readonly DatabaseService _databaseService;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the TestController class
        /// </summary>
        /// <param name="databaseService">The database service</param>
        /// <param name="configuration">The configuration</param>
        public TestController(DatabaseService databaseService, IConfiguration configuration)
        {
            _databaseService = databaseService;
            _configuration = configuration;
        }

        /// <summary>
        /// Tests the database connection
        /// </summary>
        /// <returns>JSON result with connection status</returns>
        [HttpGet]
        public async Task<IActionResult> TestConnection()
        {
            var connectionString = _configuration.GetConnectionString("CMCSDatabase");
            var healthResult = await _databaseService.HealthCheckAsync();

            var result = new
            {
                ConnectionString = connectionString?.Length > 50 ?
                    connectionString.Substring(0, 50) + "..." : connectionString,
                HealthCheck = healthResult,
                Timestamp = System.DateTime.UtcNow
            };

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
            ViewBag.ConnectionString = _configuration.GetConnectionString("CMCSDatabase");
            ViewBag.DatabaseName = "CMCS_Database";

            return View();
        }
    }
}