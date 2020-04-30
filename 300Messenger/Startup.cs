using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _300Messenger.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace _300Messenger
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
            services.AddControllersWithViews();

            services.AddDbContextPool<AppDbContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection"))
            );

            // The Dependency Injection Methods - Uncomment the AddSingleton method to
            // inject the In-Memory Repository. Uncomment AddScoped for the Database
            // Repository
            //services.AddSingleton<IMessageRepository, MockMessageRepository>();
            services.AddScoped<IMessageRepository, SQLMessageRepository>();
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
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // For Middleware, the application uses:
            // UseHttpsRedirection - to allow redirects for requests
            // UseStaticFiles - allows static html, css, javascript, and image files
            //                  to be used in site.
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
