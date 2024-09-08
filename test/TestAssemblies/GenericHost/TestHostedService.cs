using CommonLib;
using DependencyInjection.StaticAccessor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GenericHost
{
    internal class TestHostedService(IServiceProvider serviceProvider, ServiceProviderList list) : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            list.Add((serviceProvider, PinnedScope.RootServices));
            list.Add((serviceProvider, PinnedScope.Services));

            using (var outerScope = serviceProvider.CreateScope())
            {
                list.Add((outerScope.ServiceProvider, PinnedScope.Services));

                using (var innerScope1 = outerScope.ServiceProvider.CreateScope())
                {
                    list.Add((innerScope1.ServiceProvider, PinnedScope.Services));
                }

                using (var innerScope2 = outerScope.ServiceProvider.CreateScope())
                {
                    list.Add((innerScope2.ServiceProvider, PinnedScope.Services));
                }
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
