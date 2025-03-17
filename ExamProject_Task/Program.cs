using ExamProject_Task.Repository;
using ExamProject_Task.Repository.Admin;
using ExamProject_Task.Repository.DataAccess;
using ExamProject_Task.Repository.Exams;
using ExamProject_Task.Repository.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//builder.Services.AddIdentity<IdentityUser, IdentityRole>()
//.AddEntityFrameworkStores<ApplicationDbContext>()
//.AddDefaultTokenProviders();

//Dependency Injection
builder.Services.AddScoped<IRepository<User>, Repository<User>>();
builder.Services.AddScoped<IRepository<Exam>, Repository<Exam>>();
builder.Services.AddScoped<IRepository<Question>, Repository<Question>>();
builder.Services.AddScoped<IRepository<Choice>, Repository<Choice>>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IExamService, ExamService>();
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();






builder.Services.AddControllersWithViews();
// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute( name: "default",pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<User>>();
    

    await SeedUsersAndRoles(userManager, roleManager);
}


app.Run();


async Task SeedUsersAndRoles(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
{
    // ÇáÊÃßÏ ãä Ãä ÇáÃÏæÇÑ ÛíÑ ãÖÇÝÉ ÞÈá ÅÖÇÝÊåÇ
    string[] roles = { "Admin", "User" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // ÅÖÇÝÉ ÇáãÓÄæá (Admin) áæ áã íßä ãæÌæÏðÇ
    if (userManager.Users.All(u => u.UserName != "admin@example.com"))
    {
        var adminUser = new User
        {
            UserName = "admin@example.com",
            Email = "admin@example.com",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, "Admin@123");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }

    // ÅÖÇÝÉ ãÓÊÎÏã ÚÇÏí (User) áæ áã íßä ãæÌæÏðÇ
    if (userManager.Users.All(u => u.UserName != "user@example.com"))
    {
        var normalUser = new User
        {
            UserName = "user@example.com",
            Email = "user@example.com",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(normalUser, "User@123");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(normalUser, "User");
        }
    }
}