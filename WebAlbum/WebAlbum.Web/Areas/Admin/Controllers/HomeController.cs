using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.AspNet.Identity.Owin;
using WebAlbum.DomainModel;
using WebAlbum.DomainServices;
using WebAlbum.Web.Areas.Admin.Models;

namespace WebAlbum.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly IGenericRepository<ApplicationUser> _userRepository;
        private readonly IMapper _mapper;
        private ApplicationUserManager _userManager;
        private const string ErrorMsg = "An error has occurred while processing your request. ";

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext()
                       .GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        public HomeController(ApplicationUserManager userManager,
            IGenericRepository<ApplicationUser> userRepository, IMapper mapper)
        {
            UserManager = userManager;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public ActionResult Index()
        {
            return View("Index");
        }
        public ActionResult _Users()
        {
            try
            {
                var users = _mapper.Map<IEnumerable<ApplicationUser>,
                    List<IndexAdminViewModel>>(_userRepository.Get());
                return PartialView(users);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public async Task<ActionResult> BlockUser(string userName)
        {
            try
            {
                var user = await UserManager.FindByNameAsync(userName);
                if (user != null && user.LockoutEnabled)
                {
                    await UserManager.SetLockoutEndDateAsync(user.Id,
                        DateTimeOffset.UtcNow.AddDays(1));
                    TempData["result"] = $"User {user.UserName} has been blocked for 1 day.";
                }
                else
                    TempData["result"] = "You can not block Admin Page.";
            }
            catch (Exception e)
            {
                TempData["result"] = ErrorMsg + e.Message;
            }

            return RedirectToAction("_Users", "Home");
        }

        [HttpPost]
        public async Task<ActionResult> UnlockUser(string userName)
        {
            try
            {
                var user = await UserManager.FindByNameAsync(userName);
                if (user != null && user.LockoutEnabled)
                {
                    await UserManager.SetLockoutEndDateAsync(user.Id, DateTimeOffset.UtcNow);
                    TempData["result"] = $"User {user.UserName} has been successfully unlocked.";
                }
                else
                    TempData["result"] = ErrorMsg;
            }
            catch (Exception e)
            {
                TempData["result"] = ErrorMsg + e.Message;
            }

            return RedirectToAction("_Users", "Home");
        }

    }
}