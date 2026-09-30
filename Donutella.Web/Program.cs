using Donutella.Application.Interfaces;
using Donutella.Application.Services;
using Donutella.Application.Services.AuthService;
using Donutella.Application.Services.JWTService;
using Donutella.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Donutella.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ==============================
// MVC
// ==============================
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });


// ==============================
// Infrastructure
// ==============================
builder.Services.AddInfrastructure(builder.Configuration);


// ==============================
// Repositories & UnitOfWork
// ==============================
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped(
    typeof(IGenericRepository<>),
    typeof(GenericRepository<>)
);

builder.Services.AddScoped<IProductRepository, ProductRepository>();


// ==============================
// Application Services
// ==============================
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();
builder.Services.AddScoped<IWishlistService, WishlistService>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IReviewService, ReviewService>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
// ==============================
// Authentication & JWT
// ==============================
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IJwtService, JwtService>();


// ==============================
// Cart & Order
// ==============================
builder.Services.AddScoped<ICartService, CartService>();

builder.Services.AddScoped<IOrderServiceU, OrderServiceU>();

builder.Services.AddHttpClient<IGeminiService, GeminiService>();


// ==============================
// Build Application
// ==============================
var app = builder.Build();


// ==============================
// HTTP Request Pipeline
// ==============================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();


// ==============================
// MVC Routing
// ==============================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);


app.Run();
