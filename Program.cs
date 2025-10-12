using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using contract_monthly_claim_system_cs.Models.DataModels;
using System.IO;

namespace contract_monthly_claim_system_cs
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Create web application builder with configuration
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                Args = args,
                WebRootPath = "wwwroot",
                ContentRootPath = Directory.GetCurrentDirectory()
            });

            // Add configuration
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            // Add services to the container with proper MVC configuration
            builder.Services.AddControllersWithViews();

            // Add Entity Framework Core with SQL Server
            builder.Services.AddDbContext<CMCSDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("CMCSDatabase")));

            // Add session services
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = System.TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.Name = "CMCS.Session";
            });

            // Build the application
            var app = builder.Build();

            // Ensure wwwroot directory exists
            var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            if (!Directory.Exists(webRootPath))
            {
                Directory.CreateDirectory(webRootPath);

                // Create subdirectories
                var cssPath = Path.Combine(webRootPath, "css");
                var jsPath = Path.Combine(webRootPath, "js");
                var libPath = Path.Combine(webRootPath, "lib");

                Directory.CreateDirectory(cssPath);
                Directory.CreateDirectory(jsPath);
                Directory.CreateDirectory(libPath);

                // Create default CSS file if it doesn't exist
                var defaultCssPath = Path.Combine(cssPath, "site.css");
                if (!File.Exists(defaultCssPath))
                {
                    File.WriteAllText(defaultCssPath, "/* Default CSS File */\nbody { margin: 0; padding: 0; }");
                }
            }

            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            else
            {
                // Development-specific configuration
                app.UseDeveloperExceptionPage();
            }

            // Configure middleware pipeline
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            // Use session middleware - must be after UseRouting and before MapControllerRoute
            app.UseSession();

            // Configure endpoints
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Auth}/{action=Index}/{id?}");

            // Run the application
            app.Run();
        }
    }
}