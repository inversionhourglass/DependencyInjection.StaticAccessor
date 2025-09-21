using CommonLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GenericHost
{
    public class Main
    {
        public IHost Execute(ServiceProviderList list)
        {
            var hostAppBuilder = Host.CreateApplicationBuilder();

            hostAppBuilder.Services.AddSingleton(list);
            hostAppBuilder.Services.AddHostedService<TestHostedService>();

            hostAppBuilder.UsePinnedScopeServiceProvider();

            var host = hostAppBuilder.Build();

            host.Start();

            return host;
        }
    }
}
