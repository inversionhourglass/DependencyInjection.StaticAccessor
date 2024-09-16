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
            VersionCheck(tProvider.Assembly.GetName().Version);
            var pCallSiteFactory = tProvider.GetProperty("CallSiteFactory", BindingFlags.NonPublic | BindingFlags.Instance);
            var callSiteFactory = pCallSiteFactory.GetValue(serviceProvider);
            var fCallSiteCache = callSiteFactory.GetType().GetField("_callSiteCache", BindingFlags.NonPublic | BindingFlags.Instance);
            var callSiteCache = (IDictionary)fCallSiteCache.GetValue(callSiteFactory);
            object? key = null;
            foreach (DictionaryEntry item in callSiteCache)
            {
                var pType = item.Key.GetType().GetProperty("Type");
                var type = (Type)pType.GetValue(item.Key);
                if (type == typeof(IServiceScopeFactory))
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

            SetRootServices(serviceProvider);
        }

        private void SetRootServices(IServiceProvider serviceProvider)
        {
            var tServiceProvider = serviceProvider.GetType();
            var pRoot = tServiceProvider.GetProperty("Root", BindingFlags.Instance | BindingFlags.NonPublic);
            var engineScope = (IServiceProvider)pRoot.GetValue(serviceProvider);

            PinnedScope.RootServices = engineScope;
        }

        private void VersionCheck(Version version)
        {
            var minVersion = new Version(7, 0, 0);
            var maxVersion = new Version(8, 0, 0);

            if (version < minVersion || version >= maxVersion) throw new NotSupportedException($"The version of Microsoft.Extensions.DependencyInjection is {version}, which is out of the allowed range [{minVersion}, {maxVersion}).");
        }
    }
}
