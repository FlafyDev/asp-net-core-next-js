using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace Boxing
{
  public class Startup
  {
    public IHostEnvironment CurrentEnvironment { get; protected set; }
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddControllersWithViews();

      services.AddSpaStaticFiles(configuration =>
      {
        configuration.RootPath = "ClientApp/build";
      });

      services.AddSession(options =>
      {
        options.IdleTimeout = TimeSpan.FromMinutes(1);
      });

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostEnvironment env, ILoggerFactory loggerFactory)
    {
      // If not requesting /api*, rewrite to / so SPA app will be returned

      app.UseDefaultFiles();
      app.UseHttpsRedirection();
      app.UseStaticFiles();

      app.UseForwardedHeaders(new ForwardedHeadersOptions
      {
        // Read and use headers coming from reverse proxy: X-Forwarded-For X-Forwarded-Proto
        // This is particularly important so that HttpContet.Request.Scheme will be correct behind a SSL terminating proxy
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
      });

      app.UseSession();

      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
      });

      if (env.IsDevelopment())
      {
        app.UseSpa(spa =>
        {
          spa.UseProxyToSpaDevelopmentServer("http://localhost:3000/");
        });

        app.UseDeveloperExceptionPage();
      }

    }
  }
}
