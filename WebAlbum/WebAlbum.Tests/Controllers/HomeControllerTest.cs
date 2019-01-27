using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAlbum.Web.Controllers;

namespace WebAlbum.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            var controller = new HomeController();
            var result = controller.Index() as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Error()
        {
            var controller = new HomeController();
            var result = controller.Error() as ViewResult;
            Assert.AreEqual("Your file exceeds the maximum upload size (4MB).", 
                result.TempData["error"]);
        }
    }
}
