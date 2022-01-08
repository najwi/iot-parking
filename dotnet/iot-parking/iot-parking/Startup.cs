using iot_parking.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MQTTnet.Client.Options;

using iot_parking.Services;
using iot_parking.Settings;
using Microsoft.Extensions.Hosting;

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

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IMqttClientOptions>(serviceProvider =>
            {
                var clientSettings = AppSettingsProvider.ClientSettings;
                var brokerHostSettings = AppSettingsProvider.BrokerHostSettings;

                var optionBuilder = new MqttClientOptionsBuilder();
                optionBuilder
                    .WithCredentials(clientSettings.UserName, clientSettings.Password)
                    .WithClientId(clientSettings.Id)
                    .WithTcpServer(brokerHostSettings.Host, brokerHostSettings.Port);
                return optionBuilder.Build();
            });

            services.AddSingleton<IMqttClientService, MqttClientService>();
            services.AddSingleton<IHostedService>(serviceProvider =>
            {
                return serviceProvider.GetService<IMqttClientService>();
            });

            services.AddControllersWithViews();
            services.AddDbContextPool<DatabaseContext>(options => 
                options.UseMySql(Configuration.GetConnectionString("DatabaseContext"), ServerVersion.AutoDetect(Configuration.GetConnectionString("DatabaseContext"))));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
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
            });
        }
    }
}
