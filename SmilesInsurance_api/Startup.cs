using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Prometheus;
using SmilesInsurance_api.Configurations;
using SmilesInsurance_api.Data;
using SmilesInsurance_api.Filters;
using SmilesInsurance_api.Helpers;
using SmilesInsurance_api.Middlewares;
using SmilesInsurance_api.Startups;
using System;

namespace SmilesInsurance_api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            if (configuration.GetSection("Project") is null) throw new ArgumentNullException("Project is not setted in appsetting.json");

            ProjectConfiguration = configuration.GetSection("Project")?.Get<ProjectSetting>();
            OAuthConfiguration = configuration.GetSection("OAuth")?.Get<OAuthSetting>();
            MasstransitConfiguration = configuration.GetSection("Masstransit")?.Get<MasstransitSetting>();
            QuartzConfiguration = configuration.GetSection("Quartz")?.Get<QuartzSetting>();
        }

        private IConfiguration Configuration { get; }
        private ProjectSetting ProjectConfiguration { get; }
        private OAuthSetting OAuthConfiguration { get; }
        private MasstransitSetting MasstransitConfiguration { get; }
        private QuartzSetting QuartzConfiguration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(ErrorFilter));
                options.Filters.Add(new ValidateModelFilter());
            })
                .AddNewtonsoftJson(options =>
                {
                    //ReferenceLoopHandling : ไม่ใส่ผลของ dto ซ้ำใน dto;
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                    // NullValueHandling : ถ้าค่าเป็น null จะไม่แสดงใน response;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                });

            services.AddHttpContextAccessor();

            services.AddResponseCaching();

            // CORS *
            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

            // HealthChecks
            services.AddHealthChecks()
                .AddDbContextCheck<AppDBContext>(tags: new[] { "ready" })
                .ForwardToPrometheus();

            // AutoMapper *
            services.AddAutoMapper(typeof(Startup));

            // DBContext *
            services.AddDbContext<AppDBContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Masstransit (RabbitMQ & Kafka)
            services.AddMassTransit(ProjectConfiguration, MasstransitConfiguration);

            // Dependency Injection *
            services.ConfigDependency(Configuration);

            // Quart (Cron Job) *
            services.AddQuart(QuartzConfiguration);

            // Swagger (API Document) *
            services.AddSwaggerWithOAuth(ProjectConfiguration, OAuthConfiguration);

            // Authenication *
            services.AddAuthorizationWithOAuth(OAuthConfiguration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseSwaggerWithOAuth(ProjectConfiguration);

            //app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseHttpMetrics();

            app.UseResponseCaching();

            app.UseAuthentication();

            app.UseAuthorization();

            // CORS
            app.UseCors("MyPolicy");

            // HealthChecks
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
                {
                    ResponseWriter = HealthCheckResponseWriter.WriteResponseReadiness,
                    Predicate = (check) => check.Tags.Contains("ready")
                });

                endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
                {
                    ResponseWriter = HealthCheckResponseWriter.WriteResponseLiveness,
                    Predicate = (check) => !check.Tags.Contains("ready")
                });

                endpoints.MapMetrics();
            });

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}