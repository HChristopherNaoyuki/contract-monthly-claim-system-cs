using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace contract_monthly_claim_system_cs.Controllers
{
    /// <summary>
    /// Controller for web server troubleshooting and diagnostics
    /// Helps diagnose and fix web server connection issues
    /// </summary>
    public class ServerController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ServerController> _logger;

        /// <summary>
        /// Initializes a new instance of ServerController
        /// </summary>
        /// <param name="configuration">Configuration instance</param>
        /// <param name="logger">Logger instance</param>
        public ServerController(IConfiguration configuration, ILogger<ServerController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Displays server status and diagnostics information
        /// </summary>
        /// <returns>View with server diagnostics</returns>
        [HttpGet]
        public IActionResult Status()
        {
            _logger.LogInformation("Server status page requested");

            var model = new ServerStatusViewModel
            {
                MachineName = Environment.MachineName,
                OSVersion = Environment.OSVersion.ToString(),
                ProcessId = Process.GetCurrentProcess().Id,
                ProcessName = Process.GetCurrentProcess().ProcessName,
                WorkingSet = Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024, // MB
                StartTime = Process.GetCurrentProcess().StartTime,
                EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                BaseDirectory = AppContext.BaseDirectory,
                PortCheckResults = CheckPortAvailability(),
                Urls = GetConfiguredUrls()
            };

            return View(model);
        }

        /// <summary>
        /// Tests specific port availability
        /// </summary>
        /// <param name="port">Port number to test</param>
        /// <returns>JSON result with port status</returns>
        [HttpGet]
        public IActionResult TestPort(int port = 5000)
        {
            _logger.LogInformation("Testing port {Port} availability", port);

            var result = new
            {
                Port = port,
                IsAvailable = IsPortAvailable(port),
                Timestamp = DateTime.UtcNow
            };

            return Json(result);
        }

        /// <summary>
        /// Provides web server troubleshooting guide
        /// </summary>
        /// <returns>View with troubleshooting information</returns>
        [HttpGet]
        public IActionResult Troubleshooting()
        {
            return View();
        }

        /// <summary>
        /// Checks availability of common web server ports
        /// </summary>
        /// <returns>Dictionary of port availability results</returns>
        private Dictionary<int, bool> CheckPortAvailability()
        {
            var ports = new[] { 5000, 5001, 7000, 7001, 80, 443 };
            var results = new Dictionary<int, bool>();

            foreach (var port in ports)
            {
                results[port] = IsPortAvailable(port);
            }

            return results;
        }

        /// <summary>
        /// Checks if a specific port is available
        /// </summary>
        /// <param name="port">Port number to check</param>
        /// <returns>True if port is available</returns>
        private bool IsPortAvailable(int port)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    var result = client.BeginConnect("localhost", port, null, null);
                    var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(2));
                    client.EndConnect(result);

                    // If connection succeeded, port is in use
                    return !success;
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Port {Port} check failed", port);
                // If connection failed, port is likely available
                return true;
            }
        }

        /// <summary>
        /// Gets configured URLs from application configuration
        /// </summary>
        /// <returns>List of configured URLs</returns>
        private List<string> GetConfiguredUrls()
        {
            var urls = new List<string>();

            // Get URLs from Kestrel configuration
            var kestrelEndpoints = _configuration.GetSection("Kestrel:Endpoints");
            if (kestrelEndpoints.Exists())
            {
                foreach (var endpoint in kestrelEndpoints.GetChildren())
                {
                    var url = endpoint.GetValue<string>("Url");
                    if (!string.IsNullOrEmpty(url))
                    {
                        urls.Add(url);
                    }
                }
            }

            // Get URLs from launch settings (for development)
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                urls.AddRange(new[] { "https://localhost:7000", "http://localhost:5000" });
            }

            return urls;
        }
    }

    /// <summary>
    /// View model for server status information
    /// </summary>
    public class ServerStatusViewModel
    {
        /// <summary>
        /// Gets or sets the machine name
        /// </summary>
        public string MachineName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the operating system version
        /// </summary>
        public string OSVersion { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the process ID
        /// </summary>
        public int ProcessId { get; set; }

        /// <summary>
        /// Gets or sets the process name
        /// </summary>
        public string ProcessName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the working set memory in MB
        /// </summary>
        public long WorkingSet { get; set; }

        /// <summary>
        /// Gets or sets the process start time
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the environment name
        /// </summary>
        public string EnvironmentName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the application base directory
        /// </summary>
        public string BaseDirectory { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the port availability check results
        /// </summary>
        public Dictionary<int, bool> PortCheckResults { get; set; } = new Dictionary<int, bool>();

        /// <summary>
        /// Gets or sets the configured URLs
        /// </summary>
        public List<string> Urls { get; set; } = new List<string>();
    }
}