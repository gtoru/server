using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Prometheus;
using Serilog;
using Serilog.Events;
using server.core.Api.Authentication;
using server.core.Api.Authorization;
using server.core.Api.Infrastructure;
using server.core.Api.Middleware;
using server.core.Application;
using server.core.Domain.Authentication;
using server.core.Infrastructure;
using AuthorizationPolicy = server.core.Api.Authorization.AuthorizationPolicy;

namespace server.core
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            ConfigureLogger();
            services
                .AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new IsoDateTimeConverter
                    {
                        DateTimeStyles = DateTimeStyles.AdjustToUniversal
                    });
                });
            services.AddSingleton<StatusReporter>();
            services.AddSingleton<ShutdownManager>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddHttpContextAccessor();
            services.AddSingleton<IAuthenticator, JwtAuthenticator>();
            services.AddSingleton<IAuthorizationHandler, AccessLevelHandler>();
            services.AddSingleton<IAuthorizationHandler, UserIdRouteHandler>();
            services.AddCors(options =>
            {
                options.AddPolicy("localhost", builder =>
                {
                    builder
                        .SetIsOriginAllowed(_ => true)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
            });
            services.AddLogging(builder => builder.AddSerilog());
            services.AddDbContext<AppDbContext>(options =>
            {
                var host = Configuration["DB:Host"];
                var port = Configuration["DB:Port"];
                var database = Configuration["DB:Database"];
                var userName = Configuration["DB:Username"];
                var password = Configuration["DB:Password"];
                var logFactory = LoggerFactory.Create(builder => builder.AddSerilog());
                options.UseNpgsql(
                        $"Host={host};Port={port};Database={database};Username={userName};Password={password}")
                    .UseLoggerFactory(logFactory);
            });
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = Configuration["Environment"] == "Production";
                options.SaveToken = true;
                var key = Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]);
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateLifetime = true,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthorizationPolicy.OnlyAdmins, policy =>
                    policy.Requirements.Add(new AccessLevelRequirement(AccessLevel.Administrator)));
                options.AddPolicy(AuthorizationPolicy.EveryoneAllowed, policy =>
                    policy.Requirements.Add(new AccessLevelRequirement(AccessLevel.User))
                );
                options.AddPolicy(AuthorizationPolicy.CanOnlyAccessOwnSessions, policy =>
                    policy.Requirements.Add(new UserIdRouteRequirement("/api/v1/user")));
            });
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "GTO API",
                    Version = "v1"
                });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT based authorization header with Bearer scheme",
                    Type = SecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Scheme = "JSON Web Token",
                    BearerFormat = "Bearer <your_token>"
                });
                options.EnableAnnotations();
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "JSON Web Token",
                            Name = "Authorization",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });
            services.AddSwaggerGenNewtonsoftSupport();
        }

        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IUnitOfWork unitOfWork,
            AppDbContext dbContext,
            StatusReporter statusReporter)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors("localhost");
            }

            app.UseMiddleware<TraceLoggingMiddleware>();
            app.UseSerilogRequestLogging();
            dbContext.Database.Migrate();

            CreateAdminUser(unitOfWork);

            app.UseSwagger(options => { options.RouteTemplate = "docs/{documentName}/docs.json"; });
            app.UseSwaggerUI(options =>
            {
                options.RoutePrefix = "docs";
                options.SwaggerEndpoint("/docs/v1/docs.json", "GTO API v1");
            });

            app.UseRouting();
            app.UseHttpMetrics();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<UnitOfWorkMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapMetrics();
            });

            statusReporter.SetReady();
        }

        private void CreateAdminUser(IUnitOfWork unitOfWork)
        {
            var login = Configuration["Admin:Login"];
            var password = Configuration["Admin:Password"];

            AuthenticationManager.CreateAdmin(unitOfWork, login, password)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            unitOfWork.SaveAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private void ConfigureLogger()
        {
            const string loggingTemplate =
                "[{Timestamp:HH:mm:ss} {Level:u3}] {TraceId} {Message:lj}{NewLine}{Exception}";

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: loggingTemplate)
                .CreateLogger();
        }
    }
}
