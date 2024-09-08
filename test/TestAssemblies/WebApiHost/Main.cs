using CommonLib;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace WebApiHost
{
    public class Main
    {
        public (IHost host, string address) Execute(ServiceProviderList list)
        {
            IHost host;
#if NET6_0_OR_GREATER
            var builder = WebApplication.CreateBuilder();
            builder.WebHost.UseUrls("http://127.0.0.1:0");

            ConfigureServices(builder.Services);
            builder.Host.UsePinnedScopeServiceProvider();

            var app = builder.Build();

            Configure(app);

            host = app;
#else
            host = Host
                    .CreateDefaultBuilder()
                    .ConfigureWebHostDefaults(builder =>
                    {
                        builder
                            .ConfigureServices(ConfigureServices)
                            .Configure(Configure)
                            .UseUrls("http://127.0.0.1:0");
                    })
                    .UsePinnedScopeServiceProvider()
                    .Build();
#endif
            host.Start();

            var addressesFeature = host.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>();

            return (host, addressesFeature!.Addresses.First());

            void ConfigureServices(IServiceCollection services)
            {
                services.AddControllers().AddCurrentApplicationPart();

                services.AddHttpContextAccessor();

                services.AddSingleton(list);
            }

            void Configure(IApplicationBuilder app)
            {
                app.UseDeveloperExceptionPage();
                app.UseRouting();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
            }
        }
    }
}
