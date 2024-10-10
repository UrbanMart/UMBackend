using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using urbanmart.Models;
using urbanmart.Services;

namespace urbanmart
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Configure DatabaseSettings
            services.Configure<DatabaseSettings>(Configuration.GetSection(nameof(DatabaseSettings)));
            services.AddSingleton<IDatabaseSettings>(sp => sp.GetRequiredService<IOptions<DatabaseSettings>>().Value);

            // Register Services
            services.AddSingleton<ProductsService>();
            services.AddSingleton<OrdersService>();
            services.AddSingleton<UsersService>();
            services.AddSingleton<ProductInventoryService>();
            services.AddSingleton<NotificationsService>();
            services.AddSingleton<VendorFeedbackService>();
            services.AddControllers();
            services.AddSingleton<NotificationJob>();

            // Read CORS settings from configuration
            var allowedOrigins = Configuration["CORS:AllowedOrigins"]?.Split(',');

            // Add CORS policy
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder =>
                    {
                        if (allowedOrigins != null)
                        {
                            builder.WithOrigins(allowedOrigins)
                                   .AllowAnyMethod()
                                   .AllowAnyHeader();
                        }
                    });
            });

            // Swagger configuration
            services.AddSwaggerGen(c =>
            {
                var environment = Configuration["ASPNETCORE_ENVIRONMENT"];
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = string.IsNullOrEmpty(environment) ? "urbanmart API" : $"urbanmart API - {environment}",
                    Version = "v1"
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            // Log the current environment
            logger.LogInformation("Environment: {EnvironmentName}", env.EnvironmentName);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", $"urbanmart API - {env.EnvironmentName}"));

            app.UseHttpsRedirection();

            app.UseRouting();

            // Enable CORS
            app.UseCors("AllowSpecificOrigin");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
