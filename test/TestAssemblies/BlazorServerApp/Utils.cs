using DependencyInjection.StaticAccessor;

namespace BlazorServerApp
{
    public static class Utils
    {
        public static void M()
        {
            Console.WriteLine($"Utils.M: {PinnedScope.ScopedServices?.GetHashCode()}");
        }
    }
}
