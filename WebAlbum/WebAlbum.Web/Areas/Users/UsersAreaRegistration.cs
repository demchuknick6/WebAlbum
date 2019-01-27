using System.Web.Mvc;

namespace WebAlbum.Web.Areas.Users
{
    public class UsersAreaRegistration : AreaRegistration 
    {
        public override string AreaName => "Users";

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                name:"Users_default",
                url:"Users/{controller}/{action}/{id}",
                defaults:new { action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "WebAlbum.Web.Areas.Users.Controllers" }
            );
        }
    }
}