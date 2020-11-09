using System;
using System.IO;
using System.Reflection;
using System.Security.Authentication;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using JPlayer.Api.Middleware;
using JPlayer.Business;
using JPlayer.Business.Services;
using JPlayer.Data.Dao;
using JPlayer.Lib.Contract;
using JPlayer.Lib.Mapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace JPlayer.Api.AppStart
{
    // ReSharper disable once UnusedMember.Global
    public class Startup
    {
        private readonly string _appName;
        private readonly string _assemblyName;
        private readonly string _assemblyVersion;
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            this._configuration = configuration;
            AssemblyName assembly = typeof(Program).Assembly.GetName();
            this._assemblyName = assembly.Name;
            this._assemblyVersion = assembly.Version?.ToString();
            this._appName = this._assemblyName?.Split('.')[0];
        }

        /// <summary>
        ///     This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Application parameters
            string connString = this._configuration.GetConnectionString("sqlite");
            string authCookieName = this._configuration.GetValue<string>("Authentication:CookieName");
            int authExpirationTime = this._configuration.GetValue<int>("Authentication:ExpirationTime");

            // Database
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlite(connString);
                options.EnableSensitiveDataLogging();
                options.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
            });

            // Dependency Injection
            services.AddTransient<AuthService>();
            services.AddTransient<UserService>();
            services.AddTransient<ProfileService>();
            services.AddTransient<FunctionService>();
            services.AddTransient<ObjectMapper>();

            // Routing
            services.AddCustomAuthentication(authCookieName, authExpirationTime);
            services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            // Swagger
            services.AddCustomSwaggerGen(this._assemblyName, this._assemblyVersion, this._appName);
        }

        /// <summary>
        ///     This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            // Middleswares
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseExceptionHandler(new ExceptionHandlerOptions {ExceptionHandler = new ExceptionMiddleware().Invoke});
            app.UseEndpoints(endpoints => endpoints.MapControllers());

            // Swagger
            app.UseSwagger();
            app.UseSwaggerUI(options => options.SwaggerEndpoint($"/swagger/{this._assemblyName}/swagger.json", this._assemblyName));

            // Database
            app.EnsureDbCreated();
        }
    }

    internal static class StartupHelper
    {
        public static void EnsureDbCreated(this IApplicationBuilder app)
        {
            using IServiceScope serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
            ApplicationDbContext context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.EnsureCreated();
        }

        public static void AddCustomSwaggerGen(this IServiceCollection services, string assemblyName, string appVersion, string appName)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(assemblyName, new OpenApiInfo {Title = assemblyName, Version = appVersion});
                options.DescribeAllParametersInCamelCase();
                string[] docs = Directory.GetFiles(AppContext.BaseDirectory, $"{appName}.*.xml", SearchOption.TopDirectoryOnly);
                foreach (string xmlPath in docs)
                    options.IncludeXmlComments(xmlPath);
            });
        }

        public static void AddCustomAuthentication(this IServiceCollection services, string cookieName, int expirationTime)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = cookieName;
                    options.ExpireTimeSpan = TimeSpan.FromHours(expirationTime);
                    options.Events = new CookieAuthenticationEvents
                    {
                        OnRedirectToAccessDenied = context =>
                        {
                            context.Response.StatusCode = 403;
                            context.Response.ContentType = "application/json";
                            JsonSerializer.SerializeAsync(context.Response.Body, new AuthenticationException(GlobalLabelCodes.AuthNotAuthorized).AsApiError());
                            context.Response.Body.FlushAsync().ConfigureAwait(false);
                            return Task.CompletedTask;
                        },
                        OnRedirectToLogin = context =>
                        {
                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";
                            JsonSerializer.SerializeAsync(context.Response.Body, new AuthenticationException(GlobalLabelCodes.AuthNotAuthorized).AsApiError());
                            context.Response.Body.FlushAsync().ConfigureAwait(false);
                            return Task.CompletedTask;
                        }
                    };
                });
        }
    }
}