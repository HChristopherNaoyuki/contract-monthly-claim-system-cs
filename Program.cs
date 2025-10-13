using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using contract_monthly_claim_system_cs.Models.DataModels;
using contract_monthly_claim_system_cs.Services;
using System.IO;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Net;

namespace contract_monthly_claim_system_cs
{
    /// <summary>
    /// Main program class for the Contract Monthly Claim System
    /// Handles application startup, configuration, and web server setup
    /// Enhanced with robust error handling and multiple hosting options
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main entry point for the application
        /// </summary>
        /// <param name="args">Command line arguments</param>
        public static void Main(string[] args)
        {
            // Enhanced main method with comprehensive error handling
            try
            {
                Console.WriteLine("?? Starting Contract Monthly Claim System...");

                // Create and configure the web host builder
                var builder = CreateWebHostBuilder(args);

                // Build the application
                var app = builder.Build();

                // Configure middleware pipeline
                ConfigureMiddlewarePipeline(app);

                // Start the application
                StartApplication(app);
            }
            catch (Exception ex)
            {
                // Global exception handling for startup failures
                HandleStartupException(ex);
            }
        }

        /// <summary>
        /// Creates and configures the web host builder with enhanced settings
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>Configured WebApplicationBuilder</returns>
        private static WebApplicationBuilder CreateWebHostBuilder(string[] args)
        {
            // Create web application builder with explicit configuration
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                Args = args,
                WebRootPath = "wwwroot",
                ContentRootPath = Directory.GetCurrentDirectory()
            });

            // Configure application services in sequence
            ConfigureLogging(builder);
            ConfigureConfiguration(builder);
            ConfigureWebServer(builder);
            ConfigureApplicationServices(builder);
            ConfigureDatabaseServices(builder);

            return builder;
        }

        /// <summary>
        /// Configures logging for the application
        /// </summary>
        /// <param name="builder">WebApplication builder</param>
        private static void ConfigureLogging(WebApplicationBuilder builder)
        {
            // Clear default logging providers
            builder.Logging.ClearProviders();

            // Add console logging with enhanced configuration
            builder.Logging.AddConsole(options =>
            {
                options.IncludeScopes = true;
            });

            // Add debug logging for development
            if (builder.Environment.IsDevelopment())
            {
                builder.Logging.AddDebug();
            }

            // Configure log levels
            builder.Logging.AddFilter("Microsoft", LogLevel.Warning);
            builder.Logging.AddFilter("System", LogLevel.Warning);
            builder.Logging.AddFilter("contract_monthly_claim_system_cs", LogLevel.Information);
        }

        /// <summary>
        /// Configures application configuration sources
        /// </summary>
        /// <param name="builder">WebApplication builder</param>
        private static void ConfigureConfiguration(WebApplicationBuilder builder)
        {
            // Add configuration sources in order of precedence
            builder.Configuration
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(builder.Configuration.GetCommandLineArgs());
        }

        /// <summary>
        /// Configures web server settings including Kestrel configuration
        /// </summary>
        /// <param name="builder">WebApplication builder</param>
        private static void ConfigureWebServer(WebApplicationBuilder builder)
        {
            // Configure Kestrel server options for better performance and reliability
            builder.WebHost.ConfigureKestrel((context, serverOptions) =>
            {
                // Get configuration
                var kestrelSection = context.Configuration.GetSection("Kestrel");

                // Configure limits
                serverOptions.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10MB
                serverOptions.Limits.MaxConcurrentConnections = 100;
                serverOptions.Limits.MaxConcurrentUpgradedConnections = 100;
                serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
                serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(1);

                // Configure HTTPS in development
                if (context.HostingEnvironment.IsDevelopment())
                {
                    serverOptions.ConfigureHttpsDefaults(httpsOptions =>
                    {
                        // Allow self-signed certificates in development
                        httpsOptions.ClientCertificateMode = Microsoft.AspNetCore.Server.Kestrel.Https.ClientCertificateMode.AllowCertificate;
                    });
                }
            });

            // Configure server URLs based on environment
            if (builder.Environment.IsDevelopment())
            {
                // Use development URLs from launchSettings
                builder.WebHost.UseUrls(
                    "https://localhost:7278",
                    "http://localhost:5226",
                    "https://localhost:7000",
                    "http://localhost:5000"
                );
            }
            else
            {
                // Use production URLs
                builder.WebHost.UseUrls("http://*:80", "https://*:443");
            }
        }

        /// <summary>
        /// Configures application services and dependency injection
        /// </summary>
        /// <param name="builder">WebApplication builder</param>
        private static void ConfigureApplicationServices(WebApplicationBuilder builder)
        {
            // Add controllers with views and Razor runtime compilation
            builder.Services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();

            // Add session services with configuration
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.Name = "CMCS.Session";
                options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
                    ? Microsoft.AspNetCore.Http.CookieSecurePolicy.None
                    : Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
            });

            // Add custom services
            builder.Services.AddScoped<DatabaseService>();

            // Add health checks
            builder.Services.AddHealthChecks()
                .AddCheck<DatabaseHealthCheck>("database");

            // Register database health check service
            builder.Services.AddSingleton<IDatabaseHealthService, DatabaseHealthService>();

            // Add HTTP client factory for external services
            builder.Services.AddHttpClient("DefaultClient", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            // Add response compression for performance
            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
            });

            // Add anti-forgery services
            builder.Services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-CSRF-TOKEN";
                options.SuppressXFrameOptionsHeader = false;
            });
        }

        /// <summary>
        /// Configures database services with connection resilience
        /// </summary>
        /// <param name="builder">WebApplication builder</param>
        private static void ConfigureDatabaseServices(WebApplicationBuilder builder)
        {
            // Get connection string with fallback options
            var connectionString = GetConnectionString(builder.Configuration, builder.Environment);

            // Configure Entity Framework context
            builder.Services.AddDbContext<CMCSDbContext>(options =>
            {
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    // Configure connection resilience
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);

                    // Configure command timeout
                    sqlOptions.CommandTimeout(30);
                });

                // Enable sensitive data logging only in development
                if (builder.Environment.IsDevelopment())
                {
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                    options.LogTo(Console.WriteLine, LogLevel.Information);
                }
            });
        }

        /// <summary>
        /// Gets the appropriate connection string based on environment and configuration
        /// </summary>
        /// <param name="configuration">Configuration instance</param>
        /// <param name="environment">Hosting environment</param>
        /// <returns>Connection string</returns>
        private static string GetConnectionString(IConfiguration configuration, IWebHostEnvironment environment)
        {
            // Try primary connection string first
            var connectionString = configuration.GetConnectionString("CMCSDatabase");

            if (!string.IsNullOrEmpty(connectionString))
            {
                return connectionString;
            }

            // Fallback to LocalDB for development
            if (environment.IsDevelopment())
            {
                return @"Server=(localdb)\mssqllocaldb;Database=CMCS_Database;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true;Connection Timeout=30;";
            }

            // Final fallback
            return @"Server=localhost;Database=CMCS_Database;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true;Connection Timeout=30;";
        }

        /// <summary>
        /// Configures the middleware pipeline for the application
        /// </summary>
        /// <param name="app">Web application instance</param>
        private static void ConfigureMiddlewarePipeline(WebApplication app)
        {
            // Get logger for pipeline configuration
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Configuring middleware pipeline for {Environment} environment", app.Environment.EnvironmentName);

            // Configure error handling based on environment
            if (app.Environment.IsDevelopment())
            {
                // Detailed error pages in development
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                // Production error handling
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts(); // HTTP Strict Transport Security Protocol
            }

            // Enable response compression
            app.UseResponseCompression();

            // HTTPS Redirection Middleware with safe configuration
            app.UseHttpsRedirection();

            // Static Files Middleware with cache configuration
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    // Cache static files for 1 hour in production
                    var cachePeriod = app.Environment.IsDevelopment() ? "60" : "3600";
                    ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age={cachePeriod}");
                }
            });

            // Routing Middleware
            app.UseRouting();

            // Session Middleware (before authorization)
            app.UseSession();

            // Authentication & Authorization Middleware
            app.UseAuthentication();
            app.UseAuthorization();

            // Health check endpoint
            app.MapHealthChecks("/health");
            app.MapHealthChecks("/ready");

            // Configure endpoints
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Auth}/{action=Index}/{id?}");

            // Additional routes for specific functionality
            app.MapControllerRoute(
                name: "claims",
                pattern: "Claims/{action=Index}/{id?}",
                defaults: new { controller = "Claims" });

            app.MapControllerRoute(
                name: "api",
                pattern: "api/{controller=Home}/{action=Index}/{id?}");
        }

        /// <summary>
        /// Starts the application with proper initialization
        /// </summary>
        /// <param name="app">Web application instance</param>
        private static void StartApplication(WebApplication app)
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();

            try
            {
                // Ensure wwwroot directory exists
                EnsureWwwRootDirectory();

                // Initialize database
                InitializeDatabase(app);

                // Log startup information
                LogStartupInformation(app, logger);

                // Display startup message
                DisplayStartupMessage(app);

                // Run the application
                app.Run();
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Application failed to start");
                throw;
            }
        }

        /// <summary>
        /// Ensures wwwroot directory and necessary subdirectories exist
        /// </summary>
        private static void EnsureWwwRootDirectory()
        {
            var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            if (!Directory.Exists(webRootPath))
            {
                Directory.CreateDirectory(webRootPath);
                Console.WriteLine("?? Created wwwroot directory");
            }

            // Create necessary subdirectories
            var subdirectories = new[] { "css", "js", "lib", "images", "uploads" };
            foreach (var subdir in subdirectories)
            {
                var subdirPath = Path.Combine(webRootPath, subdir);
                if (!Directory.Exists(subdirPath))
                {
                    Directory.CreateDirectory(subdirPath);
                    Console.WriteLine($"?? Created wwwroot/{subdir} directory");
                }
            }
        }

        /// <summary>
        /// Initializes database with proper error handling
        /// </summary>
        /// <param name="app">Web application instance</param>
        private static void InitializeDatabase(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();

                try
                {
                    var context = services.GetRequiredService<CMCSDbContext>();
                    var healthService = services.GetRequiredService<IDatabaseHealthService>();

                    // Test database connection first
                    logger.LogInformation("Testing database connection...");
                    var healthResult = healthService.CheckDatabaseHealthAsync().GetAwaiter().GetResult();

                    if (healthResult.IsHealthy)
                    {
                        logger.LogInformation("Database connection successful");

                        // Ensure database is created
                        var created = context.Database.EnsureCreated();
                        if (created)
                        {
                            logger.LogInformation("Database created successfully");
                        }
                        else
                        {
                            logger.LogInformation("Database already exists");
                        }

                        // Seed initial data
                        DatabaseInitializer.Initialize(context);
                        logger.LogInformation("Database initialization completed");
                    }
                    else
                    {
                        logger.LogWarning("Database connection issues: {Message}", healthResult.Message);
                        logger.LogWarning("Application will run in limited mode without database access");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while initializing the database");
                    logger.LogWarning("Application will run in limited mode without database access");
                }
            }
        }

        /// <summary>
        /// Logs startup information for debugging and monitoring
        /// </summary>
        /// <param name="app">Web application instance</param>
        /// <param name="logger">Logger instance</param>
        private static void LogStartupInformation(WebApplication app, ILogger<Program> logger)
        {
            logger.LogInformation("?? Contract Monthly Claim System Starting Up");
            logger.LogInformation("?? Environment: {Environment}", app.Environment.EnvironmentName);
            logger.LogInformation("?? Content Root: {ContentRoot}", app.Environment.ContentRootPath);
            logger.LogInformation("?? Web Root: {WebRoot}", app.Environment.WebRootPath);
            logger.LogInformation("?? Framework: {Framework}", Environment.Version);
            logger.LogInformation("?? OS: {OS}", Environment.OSVersion);

            // Log available URLs
            var urls = app.Urls;
            foreach (var url in urls)
            {
                logger.LogInformation("?? Server listening on: {Url}", url);
            }
        }

        /// <summary>
        /// Displays startup message in console
        /// </summary>
        /// <param name="app">Web application instance</param>
        private static void DisplayStartupMessage(WebApplication app)
        {
            Console.WriteLine();
            Console.WriteLine("? Contract Monthly Claim System Started Successfully!");
            Console.WriteLine("======================================================");
            Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
            Console.WriteLine($"Framework: {Environment.Version}");
            Console.WriteLine($"OS: {Environment.OSVersion}");
            Console.WriteLine();
            Console.WriteLine("?? Available URLs:");
            foreach (var url in app.Urls)
            {
                Console.WriteLine($"   {url}");
            }
            Console.WriteLine();
            Console.WriteLine("?? Quick Access:");
            Console.WriteLine("   Main Application: https://localhost:7278");
            Console.WriteLine("   HTTP Fallback:    http://localhost:5226");
            Console.WriteLine("   Health Check:     https://localhost:7278/health");
            Console.WriteLine();
            Console.WriteLine("Press Ctrl+C to stop the application");
            Console.WriteLine("======================================================");
            Console.WriteLine();
        }

        /// <summary>
        /// Handles startup exceptions and provides helpful information
        /// </summary>
        /// <param name="ex">The exception that occurred during startup</param>
        private static void HandleStartupException(Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine("? Application failed to start!");
            Console.WriteLine("======================================================");
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Type: {ex.GetType().Name}");

            // Provide specific troubleshooting based on exception type
            if (ex is System.Net.Sockets.SocketException socketEx)
            {
                Console.WriteLine();
                Console.WriteLine("?? Port Conflict Detected!");
                Console.WriteLine("Possible solutions:");
                Console.WriteLine("1. Change ports in launchSettings.json");
                Console.WriteLine("2. Run: netstat -ano | findstr :7278 (check port usage)");
                Console.WriteLine("3. Use different ports like 5000, 7000, 5001, 7001");
                Console.WriteLine("4. Restart Visual Studio as Administrator");
            }
            else if (ex.InnerException is System.Security.Cryptography.CryptographicException)
            {
                Console.WriteLine();
                Console.WriteLine("?? SSL Certificate Issue!");
                Console.WriteLine("Run these commands in Command Prompt:");
                Console.WriteLine("   dotnet dev-certs https --clean");
                Console.WriteLine("   dotnet dev-certs https --trust");
                Console.WriteLine("Then restart Visual Studio");
            }
            else if (ex is Microsoft.Data.SqlClient.SqlException)
            {
                Console.WriteLine();
                Console.WriteLine("??? Database Connection Issue!");
                Console.WriteLine("1. Verify SQL Server is running");
                Console.WriteLine("2. Check connection string in appsettings.json");
                Console.WriteLine("3. Try using LocalDB for development");
            }

            Console.WriteLine();
            Console.WriteLine("?? Full error details:");
            Console.WriteLine(ex.ToString());
            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Environment.Exit(1);
        }
    }

    // Database Health Check and Service implementations remain the same as previous
    // but ensure they are included in the same file

    /// <summary>
    /// Database health check for ASP.NET Core Health Checks
    /// </summary>
    public class DatabaseHealthCheck : Microsoft.Extensions.Diagnostics.HealthChecks.IHealthCheck
    {
        private readonly CMCSDbContext _context;
        private readonly ILogger<DatabaseHealthCheck> _logger;

        public DatabaseHealthCheck(CMCSDbContext context, ILogger<DatabaseHealthCheck> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult> CheckHealthAsync(
            Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync(cancellationToken);
                return canConnect
                    ? Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Database is connected")
                    : Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy("Cannot connect to database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database health check failed");
                return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy("Database health check failed", ex);
            }
        }
    }

    /// <summary>
    /// Database health service interface
    /// </summary>
    public interface IDatabaseHealthService
    {
        Task<DatabaseHealthResult> CheckDatabaseHealthAsync();
    }

    /// <summary>
    /// Database health service implementation
    /// </summary>
    public class DatabaseHealthService : IDatabaseHealthService
    {
        private readonly CMCSDbContext _context;
        private readonly ILogger<DatabaseHealthService> _logger;

        public DatabaseHealthService(CMCSDbContext context, ILogger<DatabaseHealthService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DatabaseHealthResult> CheckDatabaseHealthAsync()
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync();
                if (canConnect)
                {
                    var userCount = await _context.Users.CountAsync();
                    return new DatabaseHealthResult
                    {
                        IsHealthy = true,
                        Message = "Database connection is healthy",
                        UserCount = userCount,
                        CanConnect = true
                    };
                }
                else
                {
                    return new DatabaseHealthResult
                    {
                        IsHealthy = false,
                        Message = "Cannot connect to database",
                        UserCount = 0,
                        CanConnect = false
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
                    Exception = ex
                };
            }
        }
    }

    /// <summary>
    /// Database health result
    /// </summary>
    public class DatabaseHealthResult
    {
        public bool IsHealthy { get; set; }
        public bool CanConnect { get; set; }
        public string Message { get; set; } = string.Empty;
        public int UserCount { get; set; }
        public Exception? Exception { get; set; }
    }

    /// <summary>
    /// Database initializer
    /// </summary>
    public static class DatabaseInitializer
    {
        public static void Initialize(CMCSDbContext context)
        {
            if (!context.Users.Any())
            {
                // Add default users and data
                // ... (same implementation as before)
            }
        }
    }
}