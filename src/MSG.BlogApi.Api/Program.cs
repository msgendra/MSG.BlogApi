using MSG.BlogApi.Application.Posts;
using MSG.BlogApi.Infrastructure;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddSerilog((services, configuration) => configuration
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services));

    var connectionString = builder.Configuration.GetConnectionString("BlogDb")
        ?? throw new InvalidOperationException("Connection string 'BlogDb' is not configured.");

    builder.Services.AddInfrastructure(connectionString);
    builder.Services.AddSingleton(TimeProvider.System);
    builder.Services.AddScoped<BlogPostService>();

    builder.Services.AddControllers();
    builder.Services.AddProblemDetails();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    app.UseExceptionHandler();
    app.UseStatusCodePages();
    app.UseSerilogRequestLogging();

    app.UseSwagger();
    app.UseSwaggerUI();

    app.MapControllers();
    app.MapHealthChecks("/health");

    await app.Services.ApplyMigrationsAsync();
    await app.RunAsync();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}
