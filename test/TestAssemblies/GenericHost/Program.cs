using CommonLib;

namespace GenericHost
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var main = new Main();

            var list = new ServiceProviderList();
            var host = main.Execute(list);

            Console.WriteLine("---------------------------------------------");
            foreach ((var s1, var s2) in list)
            {
                Console.WriteLine(s1 == s2);
            }
            Console.WriteLine("---------------------------------------------");

            await host.StopAsync();
        }
    }
}
