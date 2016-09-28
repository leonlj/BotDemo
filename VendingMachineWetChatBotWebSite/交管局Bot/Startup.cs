using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(交管局bot.Startup))]
namespace 交管局bot
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
