using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.StaticAccessor
{
    internal sealed class PinnedServiceScopeFactory(IServiceScopeFactory factory) : IServiceScopeFactory
    {
        public IServiceScope CreateScope()
        {
            var scope = factory.CreateScope();

            return new PinnedScope(scope);
        }
    }
}
