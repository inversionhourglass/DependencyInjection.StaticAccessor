using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.StaticAccessor
{
    /// <summary>
    /// Do something before calling the <see cref="IServiceProviderFactory{TContainerBuilder}.CreateServiceProvider(TContainerBuilder)"/>.
    /// </summary>
    public interface IBuilding
    {
        /// <summary>
        /// Do what you want with the <see cref="IServiceCollection"/> that is going to be built.
        /// </summary>
        void Handle(IServiceCollection services);
    }
}
