using System;
using System.Text;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Shared.AppStartup.Store;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace MyWorksheet.AppStartup;

public static class AuthConfig
{
    public const string SecretPw = "02EB94B6-0ECF-4744-A882-E9A2047E3E30";

    public static readonly TimeSpan CookieTimeoutUser = new TimeSpan(30, 0, 0, 1);
    public static readonly TimeSpan RefreshCookieTimeout = new TimeSpan(1, 0, 0, 1);
    public static readonly DateTime BackupPlanTime = new DateTime(1, 1, 1, 23, 59, 59, DateTimeKind.Utc);

    public static void UseCustomBearerAuthentification(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            if (context.Request.Query.TryGetValue("token", out var hasCookieAsQuery))
            {
                context.Request.Headers.Add("Authorization", new[] { $"{hasCookieAsQuery}" });
            }
            //else if (context.Request.Query.TryGetValue("access_token", out var hasAccessTokenAsQuery))
            //{
            //	context.Request.Headers.Add("Authorization", new[] { $"{hasAccessTokenAsQuery}" });
            //}
            await next();
        });

        app.UseAuthentication()
            .UseAuthorization();

    }

    public static void AddCustomBearerAuthentification(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<AppUser, Role>()
            .AddUserStore<AppUserStore>()
            .AddUserManager<AppUserManager>()
            .AddRoleManager<AppRoleManager>()
            .AddSignInManager<AppUserSignInManager>()
            .AddDefaultTokenProviders();

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = configuration.GetSection("TokenSettings").GetValue<string>("Issuer"),
                    ValidateIssuer = false,
                    ValidAudience = configuration.GetSection("TokenSettings").GetValue<string>("Audience"),
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration.GetSection("TokenSettings").GetValue<string>("Key"))),
                    ValidateIssuerSigningKey = false,
                    ValidateLifetime = false,
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/hubs")))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        else
                        {
                            var tokenValue = context.Request.Headers["Authorization"].ToString();
                            if (tokenValue.StartsWith("Bearer "))
                            {
                                context.Token = tokenValue?.Remove(0, "Bearer ".Length);
                            }
                        }
                        return Task.CompletedTask;
                    }
                };
            }); ;
    }
}