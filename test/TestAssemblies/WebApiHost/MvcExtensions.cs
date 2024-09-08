using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace WebApiHost
{
    public static class MvcExtensions
    {
        public static IMvcBuilder AddCurrentApplicationPart(this IMvcBuilder builder)
        {
            var currentAssembly = typeof(MvcExtensions).Assembly;
            if (currentAssembly != Assembly.GetEntryAssembly())
            {
                builder.AddApplicationPart(currentAssembly);
            }

            return builder;
        }
    }
}
