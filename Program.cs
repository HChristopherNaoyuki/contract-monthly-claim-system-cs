using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;
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
            if (!string.IsNullOrEmpty(connectionString))
            {
                builder.Services.AddDbContext<CMCSDbContext>(options =>
                    options.UseSqlServer(connectionString));
            }

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
}