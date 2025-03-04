using BackendTemplate.Application.ServicesInterface;
using BackendTemplate.Domain.Entities;
using BackendTemplate.Infrastructure.DbContext;
using BackendTemplate.Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;
using Serilog;
using Serilog.Sinks.Grafana.Loki;


namespace BackendTemplate.Infrastructure.Extensions
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IKeyVaultService, KeyVaultService>();
            services.AddScoped<IUserService, UserService>();

            // Resolve KeyVaultService through its interface
            var keyVaultService = services.BuildServiceProvider().GetRequiredService<IKeyVaultService>();

            var connString = keyVaultService.GetSecretAsync("SQLServerDBConnection").Result;

            // Register DbContext with SQL Server
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connString));

            // Configure Identity
            services.AddIdentity<UserEntity, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            return services;
        }

        public static IServiceCollection AddLoggingConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                                .ReadFrom.Configuration(configuration) // Reads settings from appsettings.json
                                .WriteTo.Console()
                                .WriteTo.GrafanaLoki("http://loki:3100") // Loki service in Docker
                                .WriteTo.File("Logs/BackendTemplate.log",                                
                                rollingInterval: RollingInterval.Hour) 
                                .Enrich.FromLogContext()
                                .CreateLogger();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddSerilog();
            });

            return services;
        }

        public static IApplicationBuilder ApplyMigrations(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();

            var dbContext = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

            dbContext.Database.Migrate();

            return app;
        }

        public static IApplicationBuilder SeeData(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();

            var dbContext = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
            var userManager = serviceScope.ServiceProvider.GetService<UserManager<UserEntity>>();
            var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

            ApplicationDbContext.SeedData(serviceScope.ServiceProvider, userManager, roleManager).Wait();

            // Enable Prometheus metrics
            app.UseMetricServer();
            app.UseHttpMetrics();

            return app;
        }

    }
}
