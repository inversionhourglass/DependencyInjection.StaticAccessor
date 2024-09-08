using CommonLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GenericHost
{
    public class Main
    {
        public IHost Execute(ServiceProviderList list)
        {
            var builder = Host.CreateDefaultBuilder();

            builder.ConfigureServices(services =>
            {
                services.AddSingleton(list);
                services.AddHostedService<TestHostedService>();
            });

            builder.UsePinnedScopeServiceProvider();

            var host = builder.Build();
            host.Start();

            return host;
        }
    }
}
