#if !NET48
using CommonLib;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiHost;
using Xunit;

namespace StaticAccessor.Tests
{
    public class WebApiHostTests
    {
        [Fact]
        public async Task Test()
        {
            var main = new Main();

            var list = new ServiceProviderList();
            (var host, var address) = main.Execute(list);

            var response = await SendTestRequestAsync(address);

            Assert.DoesNotContain("False", response);

            foreach (var item in list)
            {
                Assert.Equal(item.services1, item.services2);
            }
        }

        private async Task<string> SendTestRequestAsync(string address)
        {
            var message = await new HttpClient().GetAsync($"{address}/test");
            return await message.Content.ReadAsStringAsync();
        }
    }
}
#endif
