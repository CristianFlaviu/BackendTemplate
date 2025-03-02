using BackendTemplate.API.Configuration;
using BackendTemplate.Application.ServicesInterface;
using BackendTemplate.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BackendTemplate.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigureServices(builder);

            ConfigureAuthentication(builder);

            var app = builder.Build();

            ConfigureMiddleware(app);

            app.ApplyMigrations();

            app.SeeData();

            app.Run();
        }

        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen();

            builder.Services.AddInfrastructureServices(builder.Configuration);
        }

        private static void ConfigureAuthentication(WebApplicationBuilder builder)
        {
            // Resolve KeyVaultService through its interface
            var keyVaultService = builder.Services.BuildServiceProvider().GetRequiredService<IKeyVaultService>();

            // Retrieve data from  Key Vault
            var jwtSecret = keyVaultService.GetSecretAsync("jwt-secret").Result ?? throw new InvalidOperationException("JwtSettings jwt-secret is missing in the configuration.");
            var googleClientId = keyVaultService.GetSecretAsync("Google-ClientId").Result ?? throw new InvalidOperationException("Google-ClientId secret is missing in the configuration.");
            var googleClientSecret = keyVaultService.GetSecretAsync("Google-ClientSecret").Result ?? throw new InvalidOperationException("Google-ClientSecret secret is missing in the configuration.");

             var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            })
            .AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = googleClientId;
                googleOptions.ClientSecret = googleClientSecret;
            });
        }

        private static void ConfigureMiddleware(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
        }
    }
}
