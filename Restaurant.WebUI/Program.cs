using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Restaurant.Application.Interfaces;
using Restaurant.Application.Services;
using Restaurant.Infrastructure.Data;
using Restaurant.Infrastructure.Repositories;
using Restaurant.Infrastructure.Repository;
using Restaurant.Models;
using Restaurant.WebUI.Filleter;

var builder = WebApplication.CreateBuilder(args);

//// 1️⃣ DbContext
builder.Services.AddDbContext<RestaurantContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//// 2️⃣ Identity
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 5;
})
.AddEntityFrameworkStores<RestaurantContext>()
.AddDefaultTokenProviders();




//// 3️⃣ Session
builder.Services.AddDistributedMemoryCache();
//builder.Services.AddSession(options =>
//{
//    options.IdleTimeout = TimeSpan.FromHours(1);
//    options.Cookie.HttpOnly = true;
//    options.Cookie.IsEssential = true;
//});
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHostedService<Restaurant.Infrastructure.BackgroundServices.OrderStatusBackgroundService>();

//// 4️⃣ Repositories
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IFavouriteRepository, FavouriteRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

//// 5️⃣ Services
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IFavouriteService, FavouriteService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddScoped<IDailyProductOrderRepository, DailyProductOrderRepository>();
builder.Services.AddScoped<IDailyProductOrderService, DailyProductOrderService>();


builder.Services.AddHttpContextAccessor(); // للوصول للـ HttpContext

//// 6️⃣ Controllers with Views + Filters
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<ValidateNameFilter>();
});

var app = builder.Build();

app.UseMiddleware<Restaurant.Web.Middleware.ClosedHoursMiddleware>();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedAdminAsync(services);
}

//// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var admin = await userManager.FindByEmailAsync("admin@restaurant.com");
    if (admin != null)
    {
       
        var currentRoles = await userManager.GetRolesAsync(admin);
        await userManager.RemoveFromRolesAsync(admin, currentRoles);

       
        await userManager.AddToRoleAsync(admin, "Admin");
    }
}




app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

async Task SeedAdminAsync(IServiceProvider services)
{
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "Admin", "Staff", "User" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    string adminEmail = "admin@restaurant.com";
    string adminPassword = "Admin@123";

    var admin = await userManager.FindByEmailAsync(adminEmail);
    if (admin == null)
    {
        var newAdmin = new AppUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FullName = "Main Admin",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(newAdmin, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newAdmin, "Admin");
        }
    }
    else
    {
        var rolesOfAdmin = await userManager.GetRolesAsync(admin);
        if (!rolesOfAdmin.Contains("Admin"))
        {
            await userManager.RemoveFromRolesAsync(admin, rolesOfAdmin);
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}