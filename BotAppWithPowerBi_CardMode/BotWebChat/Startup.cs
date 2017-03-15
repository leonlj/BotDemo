using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BotWebChat.Startup))]
namespace BotWebChat
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
