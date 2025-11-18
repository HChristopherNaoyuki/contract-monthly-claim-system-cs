using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using contract_monthly_claim_system_cs.Services;
using contract_monthly_claim_system_cs.Models.DataModels;

namespace contract_monthly_claim_system_cs
{
    /// <summary>
    /// Main program class for the Contract Monthly Claim System
    /// Handles application startup, configuration, and web server setup
    /// Uses text file storage instead of database for Part 3 POE requirements
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main entry point for the application
        /// Part 3 POE requirement: Application startup and configuration
        /// </summary>
        /// <param name="args">Command line arguments</param>
        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Starting Contract Monthly Claim System: Part 3 POE");

                var builder = CreateWebHostBuilder(args);
                var app = builder.Build();

                ConfigureMiddlewarePipeline(app);
                InitializeApplication(app);
                StartApplication(app);
            }
            catch (Exception ex)
            {
                HandleStartupException(ex);
            }
        }

        /// <summary>
        /// Creates and configures the web host builder
        /// Part 3 POE requirement: Proper application configuration
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>Configured WebApplicationBuilder instance</returns>
        private static WebApplicationBuilder CreateWebHostBuilder(string[] args)
        {
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                Args = args,
                WebRootPath = "wwwroot",
                ContentRootPath = Directory.GetCurrentDirectory()
            });

            ConfigureLogging(builder);
            ConfigureConfiguration(builder, args);
            ConfigureWebServer(builder);
            ConfigureApplicationServices(builder);

            return builder;
        }

        /// <summary>
        /// Configures logging for the application
        /// Part 3 POE requirement: Comprehensive logging for debugging and monitoring
        /// </summary>
        /// <param name="builder">Web application builder instance</param>
        private static void ConfigureLogging(WebApplicationBuilder builder)
        {
            builder.Logging.ClearProviders();
            builder.Logging.AddSimpleConsole(options =>
            {
                options.IncludeScopes = true;
                options.SingleLine = true;
                options.TimestampFormat = "HH:mm:ss ";
            });

            if (builder.Environment.IsDevelopment())
            {
                builder.Logging.AddDebug();
            }

            // Filter out noisy framework logs
            builder.Logging.AddFilter("Microsoft", LogLevel.Warning);
            builder.Logging.AddFilter("System", LogLevel.Warning);
            builder.Logging.AddFilter("contract_monthly_claim_system_cs", LogLevel.Information);
        }

        /// <summary>
        /// Configures application configuration sources
        /// Part 3 POE requirement: Flexible configuration management
        /// </summary>
        /// <param name="builder">Web application builder instance</param>
        /// <param name="args">Command line arguments</param>
        private static void ConfigureConfiguration(WebApplicationBuilder builder, string[] args)
        {
            builder.Configuration
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            if (args.Length > 0)
            {
                builder.Configuration.AddCommandLine(args);
            }
        }

        /// <summary>
        /// Configures web server settings
        /// Part 3 POE requirement: Optimized web server configuration
        /// </summary>
        /// <param name="builder">Web application builder instance</param>
        private static void ConfigureWebServer(WebApplicationBuilder builder)
        {
            builder.WebHost.ConfigureKestrel((context, serverOptions) =>
            {
                // Configure Kestrel server limits
                serverOptions.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10MB file upload limit
                serverOptions.Limits.MaxConcurrentConnections = 100;
                serverOptions.Limits.MaxConcurrentUpgradedConnections = 100;
                serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
                serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(1);
            });

            // Use HTTP only for simplicity in Part 3 POE
            builder.WebHost.UseUrls("http://localhost:5000", "http://0.0.0.0:5000");
        }

        /// <summary>
        /// Configures application services and dependency injection
        /// Part 3 POE requirement: Proper dependency injection setup
        /// </summary>
        /// <param name="builder">Web application builder instance</param>
        private static void ConfigureApplicationServices(WebApplicationBuilder builder)
        {
            // Add MVC controllers with views and runtime compilation for development
            builder.Services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();

            // Add distributed memory cache for session storage
            builder.Services.AddDistributedMemoryCache();

            // Configure session options for user authentication
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.Name = "CMCS.Session";
                options.Cookie.SecurePolicy = CookieSecurePolicy.None; // HTTP only for Part 3 POE
            });

            // Add text file data service as singleton - Part 3 POE requirement: No database
            builder.Services.AddSingleton<TextFileDataService>();

            // Add HTTP client for potential external service integration
            builder.Services.AddHttpClient("DefaultClient", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            // Add response compression for performance optimization
            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
            });

            // Configure anti-forgery token options for security
            builder.Services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-CSRF-TOKEN";
                options.SuppressXFrameOptionsHeader = false;
            });

            // Add custom services for Part 3 POE automation features
            builder.Services.AddScoped<DatabaseService>();
        }

        /// <summary>
        /// Configures the middleware pipeline for the application
        /// Part 3 POE requirement: Proper request processing pipeline
        /// </summary>
        /// <param name="app">Web application instance</param>
        private static void ConfigureMiddlewarePipeline(WebApplication app)
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Configuring middleware pipeline for {Environment} environment", app.Environment.EnvironmentName);

            // Configure error handling based on environment
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                logger.LogInformation("Development exception page enabled");
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
                logger.LogInformation("Production error handling configured");
            }

            // Enable response compression for better performance
            app.UseResponseCompression();

            // Configure static file serving with appropriate caching
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    var cachePeriod = app.Environment.IsDevelopment() ? "60" : "3600";
                    ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age={cachePeriod}");
                }
            });

            // Configure routing and session middleware
            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();

            // Configure MVC routes for the application
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "claims",
                pattern: "Claims/{action=Index}/{id?}",
                defaults: new { controller = "Claims" });

            app.MapControllerRoute(
                name: "auth",
                pattern: "Auth/{action=Index}/{id?}",
                defaults: new { controller = "Auth" });

            logger.LogInformation("Middleware pipeline configuration completed");
        }

        /// <summary>
        /// Initializes the application data and services
        /// Part 3 POE requirement: Automated data initialization
        /// </summary>
        /// <param name="app">Web application instance</param>
        private static void InitializeApplication(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();
            var dataService = services.GetRequiredService<TextFileDataService>();

            try
            {
                logger.LogInformation("Initializing text file storage for Part 3 POE...");

                // Initialize sample data if no data exists
                InitializeSampleData(dataService, logger);

                logger.LogInformation("Text file storage initialized successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while initializing text file storage");
                throw;
            }
        }

        /// <summary>
        /// Initializes sample data for demonstration purposes
        /// Part 3 POE requirement: Pre-populated sample data for testing
        /// </summary>
        /// <param name="dataService">Text file data service instance</param>
        /// <param name="logger">Logger instance for tracking initialization</param>
        private static void InitializeSampleData(TextFileDataService dataService, ILogger<Program> logger)
        {
            try
            {
                // Check if users already exist
                var existingUsers = dataService.GetAllUsers();
                if (existingUsers.Count == 0)
                {
                    logger.LogInformation("Creating sample data for Part 3 POE demonstration...");

                    // Create comprehensive sample data for all roles
                    var sampleUsers = new[]
                    {
                        new User
                        {
                            UserId = 1,
                            Name = "System",
                            Surname = "Administrator",
                            Username = "admin",
                            Password = "admin123",
                            Role = UserRole.AcademicManager,
                            Email = "admin@cmcs.com",
                            IsActive = true,
                            CreatedDate = DateTime.UtcNow
                        },
                        new User
                        {
                            UserId = 2,
                            Name = "John",
                            Surname = "Smith",
                            Username = "lecturer",
                            Password = "lecturer123",
                            Role = UserRole.Lecturer,
                            Email = "john.smith@university.com",
                            IsActive = true,
                            CreatedDate = DateTime.UtcNow
                        },
                        new User
                        {
                            UserId = 3,
                            Name = "Sarah",
                            Surname = "Johnson",
                            Username = "coordinator",
                            Password = "coordinator123",
                            Role = UserRole.ProgrammeCoordinator,
                            Email = "sarah.johnson@university.com",
                            IsActive = true,
                            CreatedDate = DateTime.UtcNow
                        },
                        new User
                        {
                            UserId = 4,
                            Name = "Michael",
                            Surname = "Brown",
                            Username = "hr",
                            Password = "hr123",
                            Role = UserRole.HumanResource,
                            Email = "michael.brown@university.com",
                            IsActive = true,
                            CreatedDate = DateTime.UtcNow
                        }
                    };

                    foreach (var user in sampleUsers)
                    {
                        dataService.SaveUser(user);
                        logger.LogDebug("Created sample user: {Username}", user.Username);
                    }

                    // Create sample lecturer details
                    var lecturer = new Lecturer
                    {
                        LecturerId = 2,
                        EmployeeNumber = "EMP001",
                        Department = "Computer Science",
                        HourlyRate = 150.00m,
                        ContractStartDate = DateTime.Now.AddYears(-1),
                        ContractEndDate = DateTime.Now.AddYears(1),
                        BankAccountNumber = "123456789",
                        BankName = "Sample Bank",
                        TaxNumber = "TAX001"
                    };

                    dataService.SaveLecturer(lecturer);

                    // Create sample claims for Part 3 POE demonstration
                    var sampleClaim = new Claim
                    {
                        ClaimId = 1,
                        LecturerId = 2,
                        MonthYear = DateTime.Now.ToString("yyyy-MM"),
                        HoursWorked = 40,
                        HourlyRate = 150.00m,
                        Amount = 6000.00m,
                        Status = ClaimStatus.Submitted,
                        SubmissionComments = "Sample claim for Part 3 POE demonstration",
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now
                    };

                    dataService.SaveClaim(sampleClaim);

                    // Create additional sample claims for HR analytics
                    var additionalClaims = new[]
                    {
                        new Claim
                        {
                            ClaimId = 2,
                            LecturerId = 2,
                            MonthYear = DateTime.Now.AddMonths(-1).ToString("yyyy-MM"),
                            HoursWorked = 35,
                            HourlyRate = 150.00m,
                            Amount = 5250.00m,
                            Status = ClaimStatus.Approved,
                            SubmissionComments = "Previous month claim - approved",
                            CreatedDate = DateTime.Now.AddMonths(-1),
                            ModifiedDate = DateTime.Now.AddMonths(-1).AddDays(2)
                        },
                        new Claim
                        {
                            ClaimId = 3,
                            LecturerId = 2,
                            MonthYear = DateTime.Now.AddMonths(-2).ToString("yyyy-MM"),
                            HoursWorked = 45,
                            HourlyRate = 150.00m,
                            Amount = 6750.00m,
                            Status = ClaimStatus.Paid,
                            SubmissionComments = "Two months ago claim - paid",
                            CreatedDate = DateTime.Now.AddMonths(-2),
                            ModifiedDate = DateTime.Now.AddMonths(-2).AddDays(5)
                        }
                    };

                    foreach (var claim in additionalClaims)
                    {
                        dataService.SaveClaim(claim);
                    }

                    logger.LogInformation("Sample data initialization completed for Part 3 POE");
                }
                else
                {
                    logger.LogInformation("Existing data found, skipping sample data initialization");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to initialize sample data");
                throw new InvalidOperationException("Failed to initialize sample data for Part 3 POE", ex);
            }
        }

        /// <summary>
        /// Starts the application with proper initialization
        /// Part 3 POE requirement: Robust application startup
        /// </summary>
        /// <param name="app">Web application instance</param>
        private static void StartApplication(WebApplication app)
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();

            try
            {
                // Ensure required directories exist
                EnsureRequiredDirectories();

                // Log startup information
                LogStartupInformation(app, logger);

                // Display startup message
                DisplayStartupMessage(app);

                // Start the application
                app.Run();
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Application failed to start");
                throw;
            }
        }

        /// <summary>
        /// Ensures required directories exist for the application
        /// Part 3 POE requirement: Proper file system setup
        /// </summary>
        private static void EnsureRequiredDirectories()
        {
            var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            if (!Directory.Exists(webRootPath))
            {
                Directory.CreateDirectory(webRootPath);
                Console.WriteLine("Created wwwroot directory");
            }

            // Create necessary subdirectories
            var subdirectories = new[] { "css", "js", "lib", "images", "uploads" };
            foreach (var subdir in subdirectories)
            {
                var subdirPath = Path.Combine(webRootPath, subdir);
                if (!Directory.Exists(subdirPath))
                {
                    Directory.CreateDirectory(subdirPath);
                    Console.WriteLine("Created wwwroot/{0} directory", subdir);
                }
            }

            // Ensure data directory exists
            var dataPath = Path.Combine(Directory.GetCurrentDirectory(), "Data");
            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
                Console.WriteLine("Created Data directory for text file storage");
            }
        }

        /// <summary>
        /// Logs startup information for debugging and monitoring
        /// Part 3 POE requirement: Comprehensive startup logging
        /// </summary>
        /// <param name="app">Web application instance</param>
        /// <param name="logger">Logger instance</param>
        private static void LogStartupInformation(WebApplication app, ILogger<Program> logger)
        {
            logger.LogInformation("Contract Monthly Claim System: Part 3 POE Starting Up");
            logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);
            logger.LogInformation("Content Root: {ContentRoot}", app.Environment.ContentRootPath);
            logger.LogInformation("Web Root: {WebRoot}", app.Environment.WebRootPath);
            logger.LogInformation("Framework: {Framework}", Environment.Version);
            logger.LogInformation("OS: {OS}", Environment.OSVersion);
            logger.LogInformation("Machine: {MachineName}", Environment.MachineName);

            foreach (var url in app.Urls)
            {
                logger.LogInformation("Server listening on: {Url}", url);
            }

            logger.LogInformation("Part 3 POE Features: Text File Storage, HR Analytics, Automated Processing");
        }

        /// <summary>
        /// Displays startup message in console
        /// Part 3 POE requirement: User-friendly startup information
        /// </summary>
        /// <param name="app">Web application instance</param>
        private static void DisplayStartupMessage(WebApplication app)
        {
            Console.WriteLine();
            Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║               Contract Monthly Claim System                    ║");
            Console.WriteLine("║                     Part 3 POE Automation                      ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine("Application Started Successfully!");
            Console.WriteLine("======================================");
            Console.WriteLine("Environment: {0}", app.Environment.EnvironmentName);
            Console.WriteLine("Framework: {0}", Environment.Version);
            Console.WriteLine("OS: {0}", Environment.OSVersion);
            Console.WriteLine("Storage: Text Files (No Database)");
            Console.WriteLine();
            Console.WriteLine("> Available URLs:");
            foreach (var url in app.Urls)
            {
                Console.WriteLine("   • {0}", url);
            }
            Console.WriteLine();
            Console.WriteLine("> Demo Accounts:");
            Console.WriteLine("   • Lecturer:     lecturer / lecturer123");
            Console.WriteLine("   • Coordinator:  coordinator / coordinator123");
            Console.WriteLine("   • Manager:      admin / admin123");
            Console.WriteLine("   • HR:           hr / hr123");
            Console.WriteLine();
            Console.WriteLine("> Part 3 POE Features:");
            Console.WriteLine("   • Automated Claim Calculations");
            Console.WriteLine("   • HR Analytics Dashboard");
            Console.WriteLine("   • Multi-level Approval Workflow");
            Console.WriteLine("   • Performance Analytics");
            Console.WriteLine("   • PDF Report Generation");
            Console.WriteLine();
            Console.WriteLine("Press Ctrl+C to stop the application");
            Console.WriteLine("======================================");
            Console.WriteLine();
        }

        /// <summary>
        /// Handles startup exceptions and provides helpful information
        /// Part 3 POE requirement: Robust error handling and user guidance
        /// </summary>
        /// <param name="ex">Exception that occurred during startup</param>
        private static void HandleStartupException(Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine("> Application failed to start!");
            Console.WriteLine("======================================");
            Console.WriteLine("Error: {0}", ex.Message);
            Console.WriteLine("Type: {0}", ex.GetType().Name);

            // Provide specific guidance for common issues
            if (ex is System.Net.Sockets.SocketException)
            {
                Console.WriteLine();
                Console.WriteLine("> Port Conflict Detected!");
                Console.WriteLine("Possible solutions:");
                Console.WriteLine("1. Change ports in launchSettings.json");
                Console.WriteLine("2. Run: netstat -ano | findstr :5000 (check port usage)");
                Console.WriteLine("3. Use different ports like 5001, 5002");
                Console.WriteLine("4. Restart Visual Studio as Administrator");
            }
            else if (ex is UnauthorizedAccessException)
            {
                Console.WriteLine();
                Console.WriteLine("> Permission Issue Detected!");
                Console.WriteLine("Possible solutions:");
                Console.WriteLine("1. Run Visual Studio as Administrator");
                Console.WriteLine("2. Check file permissions in project directory");
                Console.WriteLine("3. Ensure write access to Data and wwwroot folders");
            }

            Console.WriteLine();
            Console.WriteLine("> Full error details:");
            Console.WriteLine(ex.ToString());
            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Environment.Exit(1);
        }
    }
}