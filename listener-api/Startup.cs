using listener_api.Listeners;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Steeltoe.Messaging.RabbitMQ.Config;
using Steeltoe.Messaging.RabbitMQ.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using listener_api.Interfaces;
using Refit;

namespace listener_api
{
    public class Startup
    {
        public const string RECEIVE_AND_CONVERT_QUEUE = "slack_comunication_queue";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddRabbitServices();
            services.AddRabbitAdmin();
            services.AddRabbitQueue(new Queue(RECEIVE_AND_CONVERT_QUEUE));
            services.AddSingleton<EmployeeListeners>();
            services.AddRabbitListeners<EmployeeListeners>();
            services.Configure<RabbitOptions>(options =>
            {
                options.Addresses = Configuration["Addresses"];
            });
            services.AddRefitClient<IslackApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration["APISlack"]));
             }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
