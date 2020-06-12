using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Messages.Models;
using Messages.Models.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Messages
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

            services.AddSignalR();
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" }
                );
            });

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
            app.UseResponseCaching();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
 
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<SessionHub>("/SessionHub");
            });
        }
    }
}
