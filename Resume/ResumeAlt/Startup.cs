using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols;
using Resume.Configuration;
using Resume.Models;
using Microsoft.WindowsAzure.Storage;

namespace Resume
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
            // app insights
            services.AddApplicationInsightsTelemetry(Configuration);

            // authentication
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();

            // db connection
            services.AddDbContext<Models.AppContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Database")));

            // MVC
            services.AddMvc().AddRazorPagesOptions(action => {}).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // dependency injection
            services.Configure<Configuration.SendGrid>(Configuration.GetSection("SendGrid"));
            services.Configure<ApplicationInsights>(Configuration.GetSection("ApplicationInsights"));
            services.Configure<CloudStorage>(Configuration.GetSection("CloudStorage"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // dev flag
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            // only allow use on this site
            var cookiePolicyOptions = new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Strict,
            };

            app.UseCookiePolicy(cookiePolicyOptions);
            app.UseAuthentication();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
