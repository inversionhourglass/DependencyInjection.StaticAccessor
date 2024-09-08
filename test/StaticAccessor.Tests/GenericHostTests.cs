using CommonLib;
using GenericHost;
using System.Threading.Tasks;
using Xunit;

namespace StaticAccessor.Tests
{
    public class GenericHostTests
    {
        [Fact]
        public async Task Test()
        {
            var main = new Main();

            var list = new ServiceProviderList();
            var host = main.Execute(list);

            foreach ((var s1, var s2) in list)
            {
                Assert.Equal(s1, s2);
            }

            await host.StopAsync();
        }
    }
}