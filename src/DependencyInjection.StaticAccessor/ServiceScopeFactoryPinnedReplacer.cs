using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Reflection;

namespace DependencyInjection.StaticAccessor
{
    /// <summary>
    /// Replace <see cref="IServiceScopeFactory"/> with <see cref="PinnedServiceScopeFactory"/>
    /// </summary>
    public sealed class ServiceScopeFactoryPinnedReplacer : IBuilt
    {
        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public void Handle(IServiceProvider serviceProvider)
        {
            var tProvider = serviceProvider.GetType();
            var pCallSiteFactory = tProvider.GetProperty("CallSiteFactory", BindingFlags.NonPublic | BindingFlags.Instance);
            var callSiteFactory = pCallSiteFactory.GetValue(serviceProvider);
            var fCallSiteCache = callSiteFactory.GetType().GetField("_callSiteCache", BindingFlags.NonPublic | BindingFlags.Instance);
            var callSiteCache = (IDictionary)fCallSiteCache.GetValue(callSiteFactory);
            object? key = null;
            foreach (DictionaryEntry item in callSiteCache)
            {
                var pServiceIdentifier = item.Key.GetType().GetProperty("ServiceIdentifier");
                var serviceIdentifier = pServiceIdentifier.GetValue(item.Key);
                var pServiceType = serviceIdentifier.GetType().GetProperty("ServiceType");
                var serviceType = (Type)pServiceType.GetValue(serviceIdentifier);
                if (serviceType == typeof(IServiceScopeFactory))
                {
                    key = item.Key;
                    break;
                }
            }

            var pRoot = tProvider.GetProperty("Root", BindingFlags.NonPublic | BindingFlags.Instance);
            var root = (IServiceScopeFactory)pRoot.GetValue(serviceProvider);
            var tCallSite = tProvider.Assembly.GetType("Microsoft.Extensions.DependencyInjection.ServiceLookup.ConstantCallSite");
            var ctor = tCallSite.GetConstructor([typeof(Type), typeof(object)]);
            var factory = ctor.Invoke([typeof(IServiceScopeFactory), new PinnedServiceScopeFactory(root)]);

            callSiteCache[key] = factory;
        }
    }
}
