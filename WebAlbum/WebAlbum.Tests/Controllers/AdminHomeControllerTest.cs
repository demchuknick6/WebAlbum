using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAlbum.DomainModel;
using WebAlbum.DomainServices;
using WebAlbum.Web;

namespace WebAlbum.Tests.Controllers
{
    [TestClass]
    public class AdminHomeControllerTest
    {
        private Web.Areas.Admin.Controllers.HomeController _homeController;
        private IGenericRepository<ApplicationUser> _userRepository;
        private ApplicationUserManager _userManager;
    }
}
