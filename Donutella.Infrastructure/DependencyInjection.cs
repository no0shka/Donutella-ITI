using Donutella.Application.Interfaces;
using Donutella.Application.Services;
using Donutella.Application.Services.AuthService;
using Donutella.Infrastructure.Data;
using Donutella.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Donutella.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductService, ProductService>();

        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICategoryService, CategoryService>();

        services.AddScoped<IWishlistRepository, WishlistRepository>();
        services.AddScoped<IWishlistService, WishlistService>();

        services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();
        services.AddScoped<ISubCategoryService, SubCategoryService>();

        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IReviewService, ReviewService>();

        // Authentication
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}