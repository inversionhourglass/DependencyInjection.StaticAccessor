using CommonLib;
using DependencyInjection.StaticAccessor;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace WebApiHost.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController(IServiceProvider serviceProvider, IHttpContextAccessor accessor, ServiceProviderList list) : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            list.Clear();

            list.Add((accessor.HttpContext?.RequestServices, PinnedScope.ScopedServices));
            list.Add((serviceProvider, PinnedScope.ScopedServices));

            using (var outerScope = serviceProvider.CreateScope())
            {
                list.Add((outerScope.ServiceProvider, PinnedScope.ScopedServices));

                using (var innerScope1 = outerScope.ServiceProvider.CreateScope())
                {
                    list.Add((innerScope1.ServiceProvider, PinnedScope.ScopedServices));
                }

                using (var innerScope2 = outerScope.ServiceProvider.CreateScope())
                {
                    list.Add((innerScope2.ServiceProvider, PinnedScope.ScopedServices));
                }
            }

            return string.Join(',', list.Select(x => x.services1 == x.services2));
        }
    }
}
