using CommonLib;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace WebApiHost
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var list = new ServiceProviderList();
            var main = new Main();

            (var host, var _) = main.Execute(list);

            await host.WaitForShutdownAsync();
        }
    }
}
