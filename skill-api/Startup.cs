using System;
using Manager.Infrastructure;
using Manager.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace Manager
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
            ConfigureLogger();

            services.AddCors(option => option.AddPolicy("ManagerPolicy", builder => {
                builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            }));

            services.AddDbContext<ApplicationContext>(opt => opt.UseMySql(Configuration["ConnectionString"]));           
            services.AddScoped<ISkillRepository, SkillRepository>();
            services.AddControllers().AddJsonOptions(options => {
                options.JsonSerializerOptions.IgnoreNullValues = true;
            });
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseCors("ManagerPolicy");

            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Employee Manager API");
            });

            loggerFactory.AddSerilog();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void ConfigureLogger()
        {
            Log.Logger = new LoggerConfiguration()
                                .Enrich.FromLogContext()
                                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(Configuration["ElasticUri"]))
                                {
                                    AutoRegisterTemplate = true
                                })
                                .CreateLogger();
        }
    }
}
