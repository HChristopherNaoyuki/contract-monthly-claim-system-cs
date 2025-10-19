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

namespace contract_monthly_claim_system_cs
{
    /// <summary>
    /// Main program class for the Contract Monthly Claim System
    /// Handles application startup, configuration, and web server setup
    /// Uses text file storage instead of database
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main entry point for the application
        /// </summary>
        /// <param name="args">Command line arguments</param>
        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Starting Contract Monthly Claim System...");

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
        /// </summary>
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
        /// </summary>
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

            builder.Logging.AddFilter("Microsoft", LogLevel.Warning);
            builder.Logging.AddFilter("System", LogLevel.Warning);
            builder.Logging.AddFilter("contract_monthly_claim_system_cs", LogLevel.Information);
        }

        /// <summary>
        /// Configures application configuration sources
        /// </summary>
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
        /// </summary>
        private static void ConfigureWebServer(WebApplicationBuilder builder)
        {
            builder.WebHost.ConfigureKestrel((context, serverOptions) =>
            {
                serverOptions.Limits.MaxRequestBodySize = 10 * 1024 * 1024;
                serverOptions.Limits.MaxConcurrentConnections = 100;
                serverOptions.Limits.MaxConcurrentUpgradedConnections = 100;
                serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
                serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(1);
            });

            builder.WebHost.UseUrls("http://localhost:5000");
        }

        /// <summary>
        /// Configures application services and dependency injection
        /// </summary>
        private static void ConfigureApplicationServices(WebApplicationBuilder builder)
        {
            // Add MVC controllers with views and runtime compilation
            builder.Services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();

            // Add distributed memory cache for session storage
            builder.Services.AddDistributedMemoryCache();

            // Configure session options
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.Name = "CMCS.Session";
                options.Cookie.SecurePolicy = CookieSecurePolicy.None;
            });

            // Add text file data service as singleton
            builder.Services.AddSingleton<TextFileDataService>();

            // Add HTTP client for external services
            builder.Services.AddHttpClient("DefaultClient", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            // Add response compression for performance
            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
            });

            // Configure anti-forgery token options
            builder.Services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-CSRF-TOKEN";
                options.SuppressXFrameOptionsHeader = false;
            });
        }

        /// <summary>
        /// Configures the middleware pipeline for the application
        /// </summary>
        private static void ConfigureMiddlewarePipeline(WebApplication app)
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Configuring middleware pipeline for {Environment} environment", app.Environment.EnvironmentName);

            // Configure error handling based on environment
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Enable response compression
            app.UseResponseCompression();

            // Configure static file serving with caching
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    var cachePeriod = app.Environment.IsDevelopment() ? "60" : "3600";
                    ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age={cachePeriod}");
                }
            });

            // Configure routing and session
            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();

            // Configure MVC routes
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "claims",
                pattern: "Claims/{action=Index}/{id?}",
                defaults: new { controller = "Claims" });

            app.MapControllerRoute(
                name: "api",
                pattern: "api/{controller=Home}/{action=Index}/{id?}");
        }

        /// <summary>
        /// Initializes the application data and services
        /// </summary>
        private static void InitializeApplication(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();
            var dataService = services.GetRequiredService<TextFileDataService>();

            try
            {
                logger.LogInformation("Initializing text file storage...");

                // Initialize sample data if no data exists
                InitializeSampleData(dataService);

                logger.LogInformation("Text file storage initialized successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while initializing text file storage");
            }
        }

        /// <summary>
        /// Initializes sample data for demonstration purposes
        /// </summary>
        private static void InitializeSampleData(TextFileDataService dataService)
        {
            try
            {
                // Check if users already exist
                var existingUsers = dataService.GetAllUsers();
                if (existingUsers.Count == 0)
                {
                    // Create sample users
                    var sampleUsers = new[]
                    {
                        new Models.DataModels.User
                        {
                            UserId = 1,
                            Name = "System",
                            Surname = "Administrator",
                            Username = "admin",
                            Password = "admin123",
                            Role = Models.DataModels.UserRole.AcademicManager,
                            Email = "admin@cmcs.com",
                            IsActive = true,
                            CreatedDate = DateTime.UtcNow
                        },
                        new Models.DataModels.User
                        {
                            UserId = 2,
                            Name = "John",
                            Surname = "Smith",
                            Username = "lecturer",
                            Password = "lecturer123",
                            Role = Models.DataModels.UserRole.Lecturer,
                            Email = "john.smith@university.com",
                            IsActive = true,
                            CreatedDate = DateTime.UtcNow
                        },
                        new Models.DataModels.User
                        {
                            UserId = 3,
                            Name = "Sarah",
                            Surname = "Johnson",
                            Username = "coordinator",
                            Password = "coordinator123",
                            Role = Models.DataModels.UserRole.ProgrammeCoordinator,
                            Email = "sarah.johnson@university.com",
                            IsActive = true,
                            CreatedDate = DateTime.UtcNow
                        }
                    };

                    foreach (var user in sampleUsers)
                    {
                        dataService.SaveUser(user);
                    }

                    // Create sample lecturer
                    var lecturer = new Models.DataModels.Lecturer
                    {
                        LecturerId = 2,
                        EmployeeNumber = "EMP001",
                        Department = "Computer Science",
                        HourlyRate = 150.00m,
                        ContractStartDate = DateTime.Now.AddYears(-1),
                        ContractEndDate = DateTime.Now.AddYears(1)
                    };

                    dataService.SaveLecturer(lecturer);

                    // Create sample claim
                    var sampleClaim = new Models.DataModels.Claim
                    {
                        ClaimId = 1,
                        LecturerId = 2,
                        MonthYear = DateTime.Now.ToString("yyyy-MM"),
                        HoursWorked = 40,
                        HourlyRate = 150.00m,
                        Amount = 6000.00m,
                        Status = Models.DataModels.ClaimStatus.Submitted,
                        SubmissionComments = "Sample claim for testing purposes",
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now
                    };

                    dataService.SaveClaim(sampleClaim);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to initialize sample data", ex);
            }
        }

        /// <summary>
        /// Starts the application with proper initialization
        /// </summary>
        private static void StartApplication(WebApplication app)
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();

            try
            {
                // Ensure required directories exist
                EnsureWwwRootDirectory();

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
        /// Ensures wwwroot directory and necessary subdirectories exist
        /// </summary>
        private static void EnsureWwwRootDirectory()
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
        }

        /// <summary>
        /// Logs startup information for debugging and monitoring
        /// </summary>
        private static void LogStartupInformation(WebApplication app, ILogger<Program> logger)
        {
            logger.LogInformation("Contract Monthly Claim System Starting Up");
            logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);
            logger.LogInformation("Content Root: {ContentRoot}", app.Environment.ContentRootPath);
            logger.LogInformation("Web Root: {WebRoot}", app.Environment.WebRootPath);
            logger.LogInformation("Framework: {Framework}", Environment.Version);
            logger.LogInformation("OS: {OS}", Environment.OSVersion);

            foreach (var url in app.Urls)
            {
                logger.LogInformation("Server listening on: {Url}", url);
            }
        }

        /// <summary>
        /// Displays startup message in console
        /// </summary>
        private static void DisplayStartupMessage(WebApplication app)
        {
            Console.WriteLine();
            Console.WriteLine("Contract Monthly Claim System Started Successfully!");
            Console.WriteLine("======================================================");
            Console.WriteLine("Environment: {0}", app.Environment.EnvironmentName);
            Console.WriteLine("Framework: {0}", Environment.Version);
            Console.WriteLine("OS: {0}", Environment.OSVersion);
            Console.WriteLine("Storage: Text Files");
            Console.WriteLine();
            Console.WriteLine("Available URLs:");
            foreach (var url in app.Urls)
            {
                Console.WriteLine("   {0}", url);
            }
            Console.WriteLine();
            Console.WriteLine("Quick Access:");
            Console.WriteLine("   Main Application: http://localhost:5000");
            Console.WriteLine("   Default Login:    admin / admin123");
            Console.WriteLine();
            Console.WriteLine("Press Ctrl+C to stop the application");
            Console.WriteLine("======================================================");
            Console.WriteLine();
        }

        /// <summary>
        /// Handles startup exceptions and provides helpful information
        /// </summary>
        private static void HandleStartupException(Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine("Application failed to start!");
            Console.WriteLine("======================================================");
            Console.WriteLine("Error: {0}", ex.Message);
            Console.WriteLine("Type: {0}", ex.GetType().Name);

            // Provide specific guidance for common issues
            if (ex is System.Net.Sockets.SocketException)
            {
                Console.WriteLine();
                Console.WriteLine("Port Conflict Detected!");
                Console.WriteLine("Possible solutions:");
                Console.WriteLine("1. Change ports in launchSettings.json");
                Console.WriteLine("2. Run: netstat -ano | findstr :5000 (check port usage)");
                Console.WriteLine("3. Use different ports like 5001, 5002");
                Console.WriteLine("4. Restart Visual Studio as Administrator");
            }

            Console.WriteLine();
            Console.WriteLine("Full error details:");
            Console.WriteLine(ex.ToString());
            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Environment.Exit(1);
        }
    }
}