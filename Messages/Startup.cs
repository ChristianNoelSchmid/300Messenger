using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _300Messenger.Messages.Models;
using _300Messenger.Messages.Models.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace _300Messenger.Messages
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
            services.AddControllers();

            services.AddHttpClient();

            services.AddDbContextPool<AppDbContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection"))
            );

            // The Dependency Injection Methods - Uncomment the AddSingleton method to
            // inject the In-Memory Repository. Uncomment AddScoped for the Database
            // Repository
            //services.AddSingleton<IMessageSessionRepository, MockMessageSessionRepository>();
            //services.AddSingleton<IMessageRepository, MockMessageRepository>()
            services.AddScoped<IMessageSessionRepository, SQLMessageSessionRepository>();
        }
        /// <summary> 
        /// Method configures the HTTP request pipeline. Middleware is adjusted here.
        /// </summary>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
