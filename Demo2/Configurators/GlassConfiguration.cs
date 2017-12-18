using Demo2.Controllers;
using Glass.Mapper.Sc;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;

namespace Demo2.Configurators
{
    public class GlassConfigurator:IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
           serviceCollection.AddTransient<ISitecoreContext,SitecoreContext>();
           serviceCollection.AddTransient<NavbarController>();
        }
    }
}