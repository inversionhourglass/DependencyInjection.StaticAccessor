using System;

namespace DependencyInjection.StaticAccessor
{
    /// <summary>
    /// Do something when <see cref="Microsoft.Extensions.DependencyInjection.IServiceProviderFactory{TContainerBuilder}.CreateServiceProvider(TContainerBuilder)"/> is called.
    /// </summary>
    public interface IBuilt
    {
        /// <summary>
        /// Do what you want with the built <see cref="IServiceProvider"/>.
        /// </summary>
        void Handle(IServiceProvider serviceProvider);
    }
}
