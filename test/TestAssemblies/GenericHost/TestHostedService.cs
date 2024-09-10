using CommonLib;
using DependencyInjection.StaticAccessor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GenericHost
{
    internal class TestHostedService(IServiceProvider serviceProvider, ServiceProviderList list) : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
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

                var locker1 = new EventWaitHandle(false, EventResetMode.ManualReset);
                var locker2 = new EventWaitHandle(false, EventResetMode.ManualReset);
                var t1 = Task.Run(() =>
                {
                    locker1.WaitOne();
                    Console.WriteLine(2);
                    list.Add((outerScope.ServiceProvider, PinnedScope.Services));
                    locker2.Set();
                });
                var t2 = Task.Run(() =>
                {
                    using (var pScope = outerScope.ServiceProvider.CreateScope())
                    {
                        list.Add((pScope.ServiceProvider, PinnedScope.Services));
                        Console.WriteLine(1);
                        locker1.Set();
                        locker2.WaitOne();
                    }
                });
                await Task.WhenAll(t1, t2);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
