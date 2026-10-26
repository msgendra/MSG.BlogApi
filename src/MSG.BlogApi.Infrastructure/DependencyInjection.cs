using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MSG.BlogApi.Application.Posts;
using MSG.BlogApi.Infrastructure.Persistence;

namespace MSG.BlogApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<BlogDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql => npgsql.EnableRetryOnFailure()));

        services.AddScoped<IBlogPostRepository, BlogPostRepository>();

        services.AddHealthChecks().AddDbContextCheck<BlogDbContext>("database");

        return services;
    }

    public static async Task ApplyMigrationsAsync(this IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<BlogDbContext>();
        await db.Database.MigrateAsync();
    }
}
