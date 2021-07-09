using Blazor.Twilio.Video.Server.Hubs;
using Blazor.Twilio.Video.Server.Options;
using Blazor.Twilio.Video.Server.Services;
using Blazor.Twilio.Video.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using System.Linq;
using static System.Environment;

namespace Blazor.Twilio.Video.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddSignalR(options => options.EnableDetailedErrors = true)
        .AddMessagePackProtocol();
            services.Configure<TwilioSettings>(settings =>
            {
                settings.AccountSid = GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
                settings.ApiSecret = GetEnvironmentVariable("TWILIO_API_SECRET");
                settings.ApiKey = GetEnvironmentVariable("TWILIO_API_KEY");
            });
            services.AddSingleton<TwilioService>();
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddResponseCompression(opts =>
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                HttpsCompression = HttpsCompressionMode.Compress,
                OnPrepareResponse = context =>
                    context.Context.Response.Headers[HeaderNames.CacheControl] =
                        $"public,max-age={86_400}"
            });
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapHub<NotificationHub>(HubEndpoints.NotificationHub);
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
