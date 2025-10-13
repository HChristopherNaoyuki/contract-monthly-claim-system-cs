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

namespace contract_monthly_claim_system_cs
{
    /// <summary>
    /// Main program class for the Contract Monthly Claim System
    /// Handles application startup and configuration
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main entry point for the application
        /// </summary>
        /// <param name="args">Command line arguments</param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                Args = args,
                WebRootPath = "wwwroot",
                ContentRootPath = Directory.GetCurrentDirectory()
            });

            // Add configuration
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            // Add services to the container
            builder.Services.AddControllersWithViews();

            // Configure database context with connection resilience
            ConfigureDatabaseServices(builder);

            // Add session services
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.Name = "CMCS.Session";
            });

            // Add custom services
            builder.Services.AddScoped<DatabaseService>();

            var app = builder.Build();

            // Ensure wwwroot directory exists
            EnsureWwwRootDirectory();

            // Initialize database with error handling
            InitializeDatabase(app);

            // Configure the HTTP request pipeline
            ConfigureRequestPipeline(app);

            app.Run();
        }

        /// <summary>
        /// Configures database services with connection resilience
        /// </summary>
        /// <param name="builder">WebApplication builder</param>
        private static void ConfigureDatabaseServices(WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("CMCSDatabase");

            if (string.IsNullOrEmpty(connectionString))
            {
                // Fallback to LocalDB for development
                connectionString = @"Server=(localdb)\mssqllocaldb;Database=CMCS_Database;Trusted_Connection=true;TrustServerCertificate=true;";
            }

            builder.Services.AddDbContext<CMCSDbContext>(options =>
            {
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    // Configure connection resilience
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null);
                });
            });

            // Register database health check service
            builder.Services.AddSingleton<IDatabaseHealthService, DatabaseHealthService>();
        }

        /// <summary>
        /// Ensures wwwroot directory exists
        /// </summary>
        private static void EnsureWwwRootDirectory()
        {
            var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            if (!Directory.Exists(webRootPath))
            {
                Directory.CreateDirectory(webRootPath);
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
                    var healthResult = healthService.CheckDatabaseHealthAsync().GetAwaiter().GetResult();

                    if (healthResult.IsHealthy)
                    {
                        logger.LogInformation("Database connection successful. Ensuring database is created...");

                        // Ensure database is created and migrations are applied
                        context.Database.EnsureCreated();

                        // Seed initial data
                        DatabaseInitializer.Initialize(context);

                        logger.LogInformation("Database initialized successfully.");
                    }
                    else
                    {
                        logger.LogWarning("Database connection issues: {Message}", healthResult.Message);
                        logger.LogWarning("Application will run in limited mode without database access.");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while initializing the database.");
                    logger.LogWarning("Application will run in limited mode without database access.");
                }
            }
        }

        /// <summary>
        /// Configures the HTTP request pipeline
        /// </summary>
        /// <param name="app">Web application instance</param>
        private static void ConfigureRequestPipeline(WebApplication app)
        {
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            else
            {
                // Detailed errors in development
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Auth}/{action=Index}/{id?}");
        }
    }

    /// <summary>
    /// Database health service interface
    /// </summary>
    public interface IDatabaseHealthService
    {
        /// <summary>
        /// Checks database health and connectivity
        /// </summary>
        /// <returns>Database health result</returns>
        Task<DatabaseHealthResult> CheckDatabaseHealthAsync();
    }

    /// <summary>
    /// Database health service implementation
    /// </summary>
    public class DatabaseHealthService : IDatabaseHealthService
    {
        private readonly CMCSDbContext _context;
        private readonly ILogger<DatabaseHealthService> _logger;

        /// <summary>
        /// Initializes a new instance of DatabaseHealthService
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="logger">Logger instance</param>
        public DatabaseHealthService(CMCSDbContext context, ILogger<DatabaseHealthService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Checks database health and connectivity
        /// </summary>
        /// <returns>Database health result</returns>
        public async Task<DatabaseHealthResult> CheckDatabaseHealthAsync()
        {
            try
            {
                // Test basic connection
                var canConnect = await _context.Database.CanConnectAsync();

                if (canConnect)
                {
                    // Test simple query execution
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
    /// Represents the result of a database health check
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
    }

    /// <summary>
    /// Database initializer for seeding initial data
    /// </summary>
    public static class DatabaseInitializer
    {
        /// <summary>
        /// Initializes the database with default data
        /// </summary>
        /// <param name="context">Database context</param>
        public static void Initialize(CMCSDbContext context)
        {
            if (!context.Users.Any())
            {
                // Add default users
                var users = new List<User>
                {
                    new User
                    {
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

                context.Users.AddRange(users);
                context.SaveChanges();

                // Add lecturer details
                var lecturer = context.Users.First(u => u.Username == "lecturer");
                var lecturerDetail = new Lecturer
                {
                    LecturerId = lecturer.UserId,
                    EmployeeNumber = "EMP001",
                    Department = "Computer Science",
                    HourlyRate = 150.00m,
                    ContractStartDate = DateTime.Now.AddYears(-1),
                    ContractEndDate = DateTime.Now.AddYears(1)
                };

                context.Lecturers.Add(lecturerDetail);
                context.SaveChanges();

                // Add sample claims for testing
                var sampleClaim = new Claim
                {
                    LecturerId = lecturer.UserId,
                    MonthYear = DateTime.Now.ToString("yyyy-MM"),
                    HoursWorked = 40,
                    HourlyRate = 150.00m,
                    Amount = 6000.00m,
                    Status = ClaimStatus.Submitted,
                    SubmissionComments = "Sample claim for testing",
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                };

                context.Claims.Add(sampleClaim);
                context.SaveChanges();
            }
        }
    }
}