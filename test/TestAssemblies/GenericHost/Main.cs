using CommonLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GenericHost
{
    public class Main
    {
        public IHost Execute(ServiceProviderList list)
        {
#if NET7_0_OR_GREATER
            var hostAppBuilder = Host.CreateApplicationBuilder();

            hostAppBuilder.Services.AddSingleton(list);
            hostAppBuilder.Services.AddHostedService<TestHostedService>();

            hostAppBuilder.UsePinnedScopeServiceProvider();

            var host = hostAppBuilder.Build();
#else
            var builder = Host.CreateDefaultBuilder();

            builder.ConfigureServices(services =>
            {
                services.AddSingleton(list);
                services.AddHostedService<TestHostedService>();
            });

            builder.UsePinnedScopeServiceProvider();

            var host = builder.Build();
#endif

            host.Start();

            return host;
        }
    }
}
