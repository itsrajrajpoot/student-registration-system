using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StudentRegistrationSystem.Data;
using StudentRegistrationSystem.Middleware;
using StudentRegistrationSystem.Models;
using StudentRegistrationSystem.Repositories;
using StudentRegistrationSystem.Services;

var builder = WebApplication.CreateBuilder(args);

#region ------------------- SERILOG -------------------

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

#endregion

#region ------------------- DATABASE -------------------

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

#endregion

#region ------------------- IDENTITY -------------------

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Home/AccessDenied";
});

#endregion

#region ------------------- SERVICES -------------------

builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IStudentService, StudentService>();

#endregion

#region ------------------- AUTOMAPPER -------------------

builder.Services.AddAutoMapper(typeof(Program));

#endregion

#region ------------------- FLUENT VALIDATION -------------------

builder.Services.AddFluentValidationAutoValidation();

#endregion

#region ------------------- SWAGGER -------------------

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#endregion

#region ------------------- MVC -------------------

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

#endregion

#region ------------------- CORS -------------------

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

#endregion

var app = builder.Build();

#region ------------------- ROLE + SYSTEM ADMIN SEEDING -------------------

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    // ✅ Only required roles
    string[] roles =
    {
        "SystemAdmin",
        "ViewStudent",
        "AddStudent",
        "EditStudent",
        "DeleteStudent"
    };

    // Create roles if not exist
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    // 🔥 Ensure System Admin exists
    var adminEmail = "admin@system.com";
    var adminPassword = "Admin@123";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FullName = "System Administrator",
            EmailConfirmed = true
        };

        await userManager.CreateAsync(adminUser, adminPassword);
    }

    // 🔥 ALWAYS ensure SystemAdmin role is assigned
    if (!await userManager.IsInRoleAsync(adminUser, "SystemAdmin"))
    {
        await userManager.AddToRoleAsync(adminUser, "SystemAdmin");
    }
}

#endregion

#region ------------------- MIDDLEWARE -------------------

app.UseSerilogRequestLogging();
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseRouting();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

#endregion

#region ------------------- ROUTING -------------------

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.MapRazorPages();

#endregion

app.Run();