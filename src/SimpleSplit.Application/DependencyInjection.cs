using System.Reflection;
using System.Text;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SimpleSplit.Application.Behaviors;
using SimpleSplit.Application.Features.Security;
using SimpleSplit.Application.Services;

namespace SimpleSplit.Application
{
    public static class DependencyInjection
    {
        private static readonly Assembly ThisAssembly = typeof(DependencyInjection).Assembly;
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();
            
            // MediatR
            var assembly = typeof(DependencyInjection).Assembly;
            services.AddMediatR(assembly)
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(MetricsBehavior<,>));
            services.AddTransient<ISortingParser, SortingParser>();
            services.AddTransient<IConditionParser, ConditionParser>();

            // Mapster
            var config = TypeAdapterConfig.GlobalSettings;
            services.AddSingleton(config);
            services.AddScoped<IMapper, ServiceMapper>();
            config.Scan(ThisAssembly);
            
            // Security
            services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));
            services.AddScoped<JwtOptions>(sp => sp.GetRequiredService<IOptionsSnapshot<JwtOptions>>().Value);
            services.AddTransient<ITokenManager, JwtTokenManager>();
            services.AddSingleton(new InternalAdministrator());

            // Setup Jwt
            var jwtOptions = configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>();
            services
                .AddAuthorization()
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    // options.Events = new JwtBearerEvents()
                    // {
                    //     OnTokenValidated = (ctx) =>
                    //     {
                    //         ctx.Fail("Bye bye");
                    //         return Task.CompletedTask;
                    //     }
                    // };
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        RequireExpirationTime = true,
                        ClockSkew = jwtOptions.TokenLifeTime,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecurityKey)),
                    };
                });

            return services;
        }
    }

}
