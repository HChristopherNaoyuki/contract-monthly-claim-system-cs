using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = "wwwroot",
    ContentRootPath = Directory.GetCurrentDirectory()
});

// Add services to the container with proper MVC configuration
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

// Add session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

var app = builder.Build();

// Ensure wwwroot directory exists
var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
if (!Directory.Exists(webRootPath))
{
    Directory.CreateDirectory(webRootPath);

    // Create subdirectories
    Directory.CreateDirectory(Path.Combine(webRootPath, "css"));
    Directory.CreateDirectory(Path.Combine(webRootPath, "js"));
    Directory.CreateDirectory(Path.Combine(webRootPath, "lib"));

    // Create default CSS file if it doesn't exist
    var defaultCssPath = Path.Combine(webRootPath, "css", "site.css");
    if (!File.Exists(defaultCssPath))
    {
        await File.WriteAllTextAsync(defaultCssPath, "/* Default CSS */\nbody { margin: 0; padding: 0; }");
    }
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Use static files with explicit configuration
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(webRootPath),
    RequestPath = ""
});

app.UseRouting();
app.UseAuthorization();

// Use session middleware
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Index}/{id?}");

app.Run();