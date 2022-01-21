using iot_parking.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MQTTnet.Client.Options;

using iot_parking.Services;
using iot_parking.Settings;
using Microsoft.Extensions.Hosting;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

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
            services.AddDbContext<DatabaseContext>(options =>
                options.UseMySql(
                    Configuration.GetConnectionString("DatabaseContext"), 
                    ServerVersion.AutoDetect(Configuration.GetConnectionString("DatabaseContext"))));

            services.AddSingleton<IMqttClientOptions>(serviceProvider =>
            {
                var clientSettings = AppSettingsProvider.ClientSettings;
                var brokerHostSettings = AppSettingsProvider.BrokerHostSettings;

                var optionBuilder = new MqttClientOptionsBuilder();
                optionBuilder
                    .WithCredentials(clientSettings.UserName, clientSettings.Password)
                    .WithClientId(clientSettings.Id)
                    .WithCleanSession(true)
                    .WithKeepAlivePeriod(new System.TimeSpan(0, 0, 30))
                    .WithTcpServer(brokerHostSettings.Host, brokerHostSettings.Port);

                optionBuilder.WithTls(new MqttClientOptionsBuilderTlsParameters()
                {
                    UseTls = true,
                    SslProtocol = System.Security.Authentication.SslProtocols.Tls12,
                    Certificates = new List<X509Certificate>()
                    {
                        new X509Certificate2(clientSettings.CertFile, clientSettings.CertPassword)
                    },
                    CertificateValidationHandler = (certContext) =>
                    {
                        X509Chain chain = new X509Chain();
                        chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;
                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.NoFlag;
                        chain.ChainPolicy.VerificationTime = System.DateTime.Now;
                        chain.ChainPolicy.UrlRetrievalTimeout = new System.TimeSpan(0, 0, 0);
                        chain.ChainPolicy.CustomTrustStore.Add(new X509Certificate2(File.ReadAllBytes(brokerHostSettings.CACertFile)));
                        chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;

                        var x5092 = new X509Certificate2(certContext.Certificate);
                        return chain.Build(x5092);
                    }
                });

                return optionBuilder.Build();
            });

            services.AddSingleton<IMqttClientService, MqttClientService>();
            services.AddSingleton<IHostedService>(serviceProvider =>
            {
                return serviceProvider.GetService<IMqttClientService>();
            });

            services.AddControllersWithViews();

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<DatabaseContext>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
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

            app.UseAuthentication();

            app.UseAuthorization();

            IdentityDataInitializer.SeedData(userManager, roleManager);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
