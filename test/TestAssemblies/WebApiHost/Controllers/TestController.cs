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

            list.Add((accessor.HttpContext?.RequestServices, PinnedScope.Services));
            list.Add((serviceProvider, PinnedScope.Services));

            using (var outerScope = serviceProvider.CreateScope())
            {
                list.Add((outerScope.ServiceProvider, PinnedScope.Services));

                using (var innerScope1 = outerScope.ServiceProvider.CreateScope())
                {
                    list.Add((innerScope1.ServiceProvider, PinnedScope.Services));
                }

                using (var innerScope2 = outerScope.ServiceProvider.CreateScope())
                {
                    list.Add((innerScope2.ServiceProvider, PinnedScope.Services));
                }
            }

            return string.Join(',', list.Select(x => x.services1 == x.services2));
        }
    }
}
