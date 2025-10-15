using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;

namespace contract_monthly_claim_system_cs
{
    /// <summary>
    /// Main program class for the Contract Monthly Claim System
    /// Handles application startup, configuration, and web server setup
    /// Enhanced with robust error handling and text file data storage
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

                // Initialize text file storage
                InitializeTextFileStorage();

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
            ConfigureConfiguration(builder, args);
            ConfigureWebServer(builder);
            ConfigureApplicationServices(builder);
            ConfigureTextFileServices(builder);

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

            // Add console logging with updated configuration
            builder.Logging.AddSimpleConsole(options =>
            {
                options.IncludeScopes = true;
                options.SingleLine = true;
                options.TimestampFormat = "HH:mm:ss ";
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
        /// <param name="args">Command line arguments</param>
        private static void ConfigureConfiguration(WebApplicationBuilder builder, string[] args)
        {
            // Add configuration sources in order of precedence
            builder.Configuration
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            // Add command line arguments if any are provided
            if (args.Length > 0)
            {
                builder.Configuration.AddCommandLine(args);
            }
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
                    ? CookieSecurePolicy.None
                    : CookieSecurePolicy.Always;
            });

            // Add text file data service
            builder.Services.AddSingleton<TextFileDataService>();

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
        /// Configures text file services for data storage
        /// </summary>
        /// <param name="builder">WebApplication builder</param>
        private static void ConfigureTextFileServices(WebApplicationBuilder builder)
        {
            // No database configuration needed - using text files
            // This method is kept for consistency with the original structure
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
                app.UseExceptionHandler("/Home/Error");
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

                // Initialize sample data
                InitializeSampleData(app);

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
        /// Initializes text file storage system
        /// </summary>
        private static void InitializeTextFileStorage()
        {
            try
            {
                // Ensure data directory exists
                var dataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Data");
                if (!Directory.Exists(dataDirectory))
                {
                    Directory.CreateDirectory(dataDirectory);
                    Console.WriteLine("?? Created Data directory for text file storage");
                }

                // Define required data files
                var dataFiles = new[]
                {
                    "users.txt",
                    "claims.txt",
                    "documents.txt",
                    "approvals.txt"
                };

                // Create empty data files if they don't exist
                foreach (var file in dataFiles)
                {
                    var filePath = Path.Combine(dataDirectory, file);
                    if (!File.Exists(filePath))
                    {
                        File.WriteAllText(filePath, "[]"); // Initialize with empty JSON array
                        Console.WriteLine($"?? Created {file} data file");
                    }
                }

                Console.WriteLine("? Text file storage initialized successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Error initializing text file storage: {ex.Message}");
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
        /// Initializes sample data for the application
        /// </summary>
        /// <param name="app">Web application instance</param>
        private static void InitializeSampleData(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();
                var dataService = services.GetRequiredService<TextFileDataService>();

                try
                {
                    // Check if we need to create sample data
                    var existingUsers = dataService.GetAllUsers();
                    if (!existingUsers.Any())
                    {
                        logger.LogInformation("Creating sample data...");

                        // Create sample users
                        var sampleUsers = new List<User>
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
                            }
                        };

                        // Save sample users
                        foreach (var user in sampleUsers)
                        {
                            dataService.SaveUser(user);
                        }

                        // Create sample lecturer details
                        var lecturer = new Lecturer
                        {
                            LecturerId = 2, // Matches John Smith's UserId
                            EmployeeNumber = "EMP001",
                            Department = "Computer Science",
                            HourlyRate = 150.00m,
                            ContractStartDate = DateTime.Now.AddYears(-1),
                            ContractEndDate = DateTime.Now.AddYears(1)
                        };

                        dataService.SaveLecturer(lecturer);

                        // Create sample claim
                        var sampleClaim = new Claim
                        {
                            ClaimId = 1,
                            LecturerId = 2,
                            MonthYear = DateTime.Now.ToString("yyyy-MM"),
                            HoursWorked = 40,
                            HourlyRate = 150.00m,
                            Amount = 6000.00m,
                            Status = ClaimStatus.Submitted,
                            SubmissionComments = "Sample claim for testing purposes",
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now
                        };

                        dataService.SaveClaim(sampleClaim);

                        logger.LogInformation("Sample data created successfully");
                    }
                    else
                    {
                        logger.LogInformation("Sample data already exists");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while initializing sample data");
                    logger.LogWarning("Application will run with existing data only");
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
            Console.WriteLine("?? Contract Monthly Claim System Started Successfully!");
            Console.WriteLine("======================================================");
            Console.WriteLine($"?? Environment: {app.Environment.EnvironmentName}");
            Console.WriteLine($"?? Framework: {Environment.Version}");
            Console.WriteLine($"?? OS: {Environment.OSVersion}");
            Console.WriteLine($"?? Storage: Text Files");
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
            Console.WriteLine("   Default Login:    admin / admin123");
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

            Console.WriteLine();
            Console.WriteLine("?? Full error details:");
            Console.WriteLine(ex.ToString());
            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Environment.Exit(1);
        }
    }

    /// <summary>
    /// Service for managing data storage using text files
    /// Replaces database functionality for prototype phase
    /// </summary>
    public class TextFileDataService
    {
        private readonly string _dataDirectory;
        private readonly ILogger<TextFileDataService> _logger;

        /// <summary>
        /// Initializes a new instance of TextFileDataService
        /// </summary>
        /// <param name="logger">Logger instance</param>
        public TextFileDataService(ILogger<TextFileDataService> logger)
        {
            _dataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Data");
            _logger = logger;

            // Ensure data directory exists
            if (!Directory.Exists(_dataDirectory))
            {
                Directory.CreateDirectory(_dataDirectory);
            }
        }

        /// <summary>
        /// Gets the file path for a specific data type
        /// </summary>
        /// <param name="dataType">Type of data (users, claims, etc.)</param>
        /// <returns>Full file path</returns>
        private string GetFilePath(string dataType)
        {
            return Path.Combine(_dataDirectory, $"{dataType}.txt");
        }

        /// <summary>
        /// Reads all data from a file
        /// </summary>
        /// <typeparam name="T">Type of data to read</typeparam>
        /// <param name="dataType">Type of data file</param>
        /// <returns>List of data objects</returns>
        private List<T> ReadData<T>(string dataType)
        {
            var filePath = GetFilePath(dataType);
            try
            {
                if (File.Exists(filePath))
                {
                    var json = File.ReadAllText(filePath);
                    if (!string.IsNullOrEmpty(json))
                    {
                        return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading data from {FilePath}", filePath);
            }

            return new List<T>();
        }

        /// <summary>
        /// Writes data to a file
        /// </summary>
        /// <typeparam name="T">Type of data to write</typeparam>
        /// <param name="dataType">Type of data file</param>
        /// <param name="data">Data to write</param>
        private void WriteData<T>(string dataType, List<T> data)
        {
            var filePath = GetFilePath(dataType);
            try
            {
                var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error writing data to {FilePath}", filePath);
                throw;
            }
        }

        // User operations
        public List<User> GetAllUsers()
        {
            return ReadData<User>("users");
        }

        public User GetUserById(int userId)
        {
            var users = GetAllUsers();
            return users.FirstOrDefault(u => u.UserId == userId);
        }

        public User GetUserByUsername(string username)
        {
            var users = GetAllUsers();
            return users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        public void SaveUser(User user)
        {
            var users = GetAllUsers();
            var existingUser = users.FirstOrDefault(u => u.UserId == user.UserId);

            if (existingUser != null)
            {
                users.Remove(existingUser);
            }

            users.Add(user);
            WriteData("users", users);
        }

        // Lecturer operations
        public List<Lecturer> GetAllLecturers()
        {
            return ReadData<Lecturer>("lecturers");
        }

        public Lecturer GetLecturerById(int lecturerId)
        {
            var lecturers = GetAllLecturers();
            return lecturers.FirstOrDefault(l => l.LecturerId == lecturerId);
        }

        public void SaveLecturer(Lecturer lecturer)
        {
            var lecturers = GetAllLecturers();
            var existingLecturer = lecturers.FirstOrDefault(l => l.LecturerId == lecturer.LecturerId);

            if (existingLecturer != null)
            {
                lecturers.Remove(existingLecturer);
            }

            lecturers.Add(lecturer);
            WriteData("lecturers", lecturers);
        }

        // Claim operations
        public List<Claim> GetAllClaims()
        {
            return ReadData<Claim>("claims");
        }

        public Claim GetClaimById(int claimId)
        {
            var claims = GetAllClaims();
            return claims.FirstOrDefault(c => c.ClaimId == claimId);
        }

        public List<Claim> GetClaimsByLecturerId(int lecturerId)
        {
            var claims = GetAllClaims();
            return claims.Where(c => c.LecturerId == lecturerId).ToList();
        }

        public void SaveClaim(Claim claim)
        {
            var claims = GetAllClaims();
            var existingClaim = claims.FirstOrDefault(c => c.ClaimId == claim.ClaimId);

            if (existingClaim != null)
            {
                claims.Remove(existingClaim);
            }

            claims.Add(claim);
            WriteData("claims", claims);
        }

        // Document operations
        public List<Document> GetAllDocuments()
        {
            return ReadData<Document>("documents");
        }

        public List<Document> GetDocumentsByClaimId(int claimId)
        {
            var documents = GetAllDocuments();
            return documents.Where(d => d.ClaimId == claimId).ToList();
        }

        public void SaveDocument(Document document)
        {
            var documents = GetAllDocuments();
            var existingDocument = documents.FirstOrDefault(d => d.DocumentId == document.DocumentId);

            if (existingDocument != null)
            {
                documents.Remove(existingDocument);
            }

            documents.Add(document);
            WriteData("documents", documents);
        }

        // Approval operations
        public List<Approval> GetAllApprovals()
        {
            return ReadData<Approval>("approvals");
        }

        public List<Approval> GetApprovalsByClaimId(int claimId)
        {
            var approvals = GetAllApprovals();
            return approvals.Where(a => a.ClaimId == claimId).ToList();
        }

        public void SaveApproval(Approval approval)
        {
            var approvals = GetAllApprovals();
            var existingApproval = approvals.FirstOrDefault(a => a.ApprovalId == approval.ApprovalId);

            if (existingApproval != null)
            {
                approvals.Remove(existingApproval);
            }

            approvals.Add(approval);
            WriteData("approvals", approvals);
        }

        /// <summary>
        /// Gets the next available ID for a data type
        /// </summary>
        /// <param name="dataType">Type of data</param>
        /// <returns>Next available ID</returns>
        public int GetNextId(string dataType)
        {
            return dataType.ToLower() switch
            {
                "users" => GetAllUsers().Count > 0 ? GetAllUsers().Max(u => u.UserId) + 1 : 1,
                "claims" => GetAllClaims().Count > 0 ? GetAllClaims().Max(c => c.ClaimId) + 1 : 1,
                "documents" => GetAllDocuments().Count > 0 ? GetAllDocuments().Max(d => d.DocumentId) + 1 : 1,
                "approvals" => GetAllApprovals().Count > 0 ? GetAllApprovals().Max(a => a.ApprovalId) + 1 : 1,
                "lecturers" => GetAllLecturers().Count > 0 ? GetAllLecturers().Max(l => l.LecturerId) + 1 : 1,
                _ => 1
            };
        }
    }
}