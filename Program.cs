using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using contract_monthly_claim_system_cs.Models.DataModels;
using contract_monthly_claim_system_cs.Services;
using System.IO;

namespace contract_monthly_claim_system_cs
{
    public class Program
    {
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

            // Add Entity Framework Core with SQL Server
            var connectionString = builder.Configuration.GetConnectionString("CMCSDatabase");
            builder.Services.AddDbContext<CMCSDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Add session services
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = System.TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.Name = "CMCS.Session";
            });

            // Add custom services
            builder.Services.AddScoped<DatabaseService>();

            var app = builder.Build();

            // Ensure wwwroot directory exists
            var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            if (!Directory.Exists(webRootPath))
            {
                Directory.CreateDirectory(webRootPath);
            }

            // Initialize database
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<CMCSDbContext>();
                context.Database.EnsureCreated();
                DatabaseInitializer.Initialize(context);
            }

            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Auth}/{action=Index}/{id?}");

            app.Run();
        }
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
                        IsActive = true
                    },
                    new User
                    {
                        Name = "John",
                        Surname = "Smith",
                        Username = "lecturer",
                        Password = "lecturer123",
                        Role = UserRole.Lecturer,
                        Email = "john.smith@university.com",
                        IsActive = true
                    },
                    new User
                    {
                        Name = "Sarah",
                        Surname = "Johnson",
                        Username = "coordinator",
                        Password = "coordinator123",
                        Role = UserRole.ProgrammeCoordinator,
                        Email = "sarah.johnson@university.com",
                        IsActive = true
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
            }
        }
    }
}