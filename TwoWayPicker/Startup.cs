using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TwoWayPicker.Startup))]
namespace TwoWayPicker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
        }
    }
}
