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
            builder.Services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.Name = "CMCS.Session";
                options.Cookie.SecurePolicy = CookieSecurePolicy.None;
            });

            builder.Services.AddSingleton<TextFileDataService>();

            builder.Services.AddHttpClient("DefaultClient", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
            });

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

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    var cachePeriod = app.Environment.IsDevelopment() ? "60" : "3600";
                    ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age={cachePeriod}");
                }
            });

            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();

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
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();
                var dataService = services.GetRequiredService<TextFileDataService>();

                try
                {
                    logger.LogInformation("Initializing text file storage...");
                    dataService.InitializeSampleData();
                    logger.LogInformation("Text file storage initialized successfully");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while initializing text file storage");
                }
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
                EnsureWwwRootDirectory();
                LogStartupInformation(app, logger);
                DisplayStartupMessage(app);
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

            var subdirectories = new[] { "css", "js", "lib", "images", "uploads" };
            foreach (var subdir in subdirectories)
            {
                var subdirPath = Path.Combine(webRootPath, subdir);
                if (!Directory.Exists(subdirPath))
                {
                    Directory.CreateDirectory(subdirPath);
                    Console.WriteLine($"Created wwwroot/{subdir} directory");
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
            Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
            Console.WriteLine($"Framework: {Environment.Version}");
            Console.WriteLine($"OS: {Environment.OSVersion}");
            Console.WriteLine($"Storage: Text Files");
            Console.WriteLine();
            Console.WriteLine("Available URLs:");
            foreach (var url in app.Urls)
            {
                Console.WriteLine($"   {url}");
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
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Type: {ex.GetType().Name}");

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