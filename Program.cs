using HospitalManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using HospitalManagementSystem.Repository.Interfaces;
using HospitalManagementSystem.Repository.Repositories;
using HospitalManagementSystem.Models;
using HospitalManagementSystem.Repository.Interfaces;
using HospitalManagementSystem.Repository.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AppCon")));

// ?? Add Identity with Roles
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// ?? Add repositories
// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AppCon")));

// ? Identity for Admin
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// ? Cookie auth for Doctor/Patient (named scheme)
builder.Services.AddAuthentication()
    .AddCookie("DoctorPatientCookie", options =>
    {
        options.LoginPath = "/Login/LoginPatient"; // fallback login page
        options.AccessDeniedPath = "/Home/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    });

// Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Authorization
builder.Services.AddAuthorization();

// Repositories
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();

var app = builder.Build();

// Seed Admin & Roles
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await SeedAdminUserAsync(userManager, roleManager);
}

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();

// ?? Enable Authentication + Authorization
app.UseAuthentication();
app.UseAuthorization();

// ?? Run Role + Admin Seeder
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.InitializeAsync(services);
}


app.UseAuthentication();
app.UseAuthorization();

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


// ?? Seeding Helper
public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roles = { "Admin", "Doctor", "Patient" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // Default Admin Account
        string adminEmail = "admin@hospital.com";
        string adminPassword = "Admin@123";

        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
// 
          
          
          
          ================ SEED ADMIN ===================
async Task SeedAdminUserAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
{
    string adminEmail = "admin@hms.com";
    string adminPassword = "Admin@123";

    // Create roles if not exist
    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole("Admin"));

    if (!await roleManager.RoleExistsAsync("Doctor"))
        await roleManager.CreateAsync(new IdentityRole("Doctor"));

    if (!await roleManager.RoleExistsAsync("Patient"))
        await roleManager.CreateAsync(new IdentityRole("Patient"));

    // Create Admin user if not exists
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };
        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}
