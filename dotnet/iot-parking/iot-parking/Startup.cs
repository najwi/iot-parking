using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mqtt.Client.AspNetCore.Extensions;
using Mqtt.Client.AspNetCore.Services;
using Mqtt.Client.AspNetCore.Settings;

namespace iot_parking
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            MapConfiguration();
        }

        public IConfiguration Configuration;

        private void MapConfiguration()
        {
            MapBrokerHostSettings();
            MapClientSettings();
        }

        private void MapBrokerHostSettings()
        {
            BrokerHostSettings brokerHostSettings = new BrokerHostSettings();
            Configuration.GetSection(nameof(BrokerHostSettings)).Bind(brokerHostSettings);
            AppSettingsProvider.BrokerHostSettings = brokerHostSettings;
        }

        private void MapClientSettings()
        {
            ClientSettings clientSettings = new ClientSettings();
            Configuration.GetSection(nameof(ClientSettings)).Bind(clientSettings);
            AppSettingsProvider.ClientSettings = clientSettings;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMqttClientHostedService();
            services.AddSingleton<ExtarnalService>();

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapGet("/mqtt", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });


        }

    }
}
