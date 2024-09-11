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
            var fEngine = tProvider.GetField("_engine", BindingFlags.NonPublic | BindingFlags.Instance);
            var engine = fEngine.GetValue(serviceProvider);
            var tEngine = engine.GetType();
            var pCallSiteFactory = tEngine.GetProperty("CallSiteFactory", BindingFlags.NonPublic | BindingFlags.Instance);
            var callSiteFactory = pCallSiteFactory.GetValue(engine);
            var fCallSiteCache = callSiteFactory.GetType().GetField("_callSiteCache", BindingFlags.NonPublic | BindingFlags.Instance);
            var callSiteCache = (IDictionary)fCallSiteCache.GetValue(callSiteFactory);
            object? key = null;
            foreach (DictionaryEntry item in callSiteCache)
            {
                if (item.Key is Type type && type == typeof(IServiceScopeFactory))
                {
                    key = item.Key;
                    break;
                }
            }

            var tCallSite = tProvider.Assembly.GetType("Microsoft.Extensions.DependencyInjection.ServiceLookup.ConstantCallSite");
            var ctor = tCallSite.GetConstructor([typeof(Type), typeof(object)]);
            var factory = ctor.Invoke([typeof(IServiceScopeFactory), new PinnedServiceScopeFactory((IServiceScopeFactory)engine)]);

            callSiteCache[key] = factory;

            SetRootServices(engine);
        }

        private void SetRootServices(object engine)
        {
            var tEngine = engine.GetType();
            var pRoot = tEngine.GetProperty("Root", BindingFlags.Instance | BindingFlags.Public);
            var engineScope = (IServiceProvider)pRoot.GetValue(engine);

            PinnedScope.RootServices = engineScope;
        }
    }
}
