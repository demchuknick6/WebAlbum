using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAlbum.Web.Controllers;

namespace WebAlbum.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void HomeIndexViewEqualIndexCshtml()
        {
            var controller = new HomeController();
            var result = controller.Index() as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ViewName);
        }

        [TestMethod]
        public void HomeErrorStringInTempData()
        {
            var controller = new HomeController();
            var result = controller.Error() as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Your file exceeds the maximum upload size (4MB).",
                result.TempData["error"]);
        }
    }
}
