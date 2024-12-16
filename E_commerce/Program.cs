using E_commerce.Data;
using E_commerce.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container

// Register the ApplicationDbContext with a connection string from appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add session services
builder.Services.AddDistributedMemoryCache();  // In-memory cache for session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Set session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;  // Essential cookies for session
});

// Add authentication using cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Login";  // Redirect to login page if not authenticated
        options.LogoutPath = "/Admin/Logout"; // Redirect to logout page when logging out
        options.ExpireTimeSpan = TimeSpan.FromSeconds(3600); // Set session expiration time (1 hour)
        options.SlidingExpiration = true;  // Automatically renew cookie expiration time on activity
    });

// Add controllers with views (MVC pattern)
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Enable static file serving (CSS, JS, images, etc.)
app.UseStaticFiles();

// Enable session middleware
app.UseSession();

// Enable routing middleware (required for the app to know where to direct incoming requests)
app.UseRouting();

// Enable authentication middleware
app.UseAuthentication();

// Enable authorization middleware (this must be placed after UseRouting() but before UseEndpoints())
app.UseAuthorization();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();  // Display detailed error pages in development
}
else
{
    app.UseExceptionHandler("/Home/Error");  // Display a generic error page in production
    app.UseHsts();  // Enforce HTTPS (important for security)
}

// Enable middleware for HTTPS redirection
app.UseHttpsRedirection();

// Configure routing to handle requests
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Run the application
app.Run();
