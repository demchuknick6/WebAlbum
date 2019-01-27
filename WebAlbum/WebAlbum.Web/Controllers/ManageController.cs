using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using WebAlbum.DomainModel;
using WebAlbum.Web.Models.Manage;

namespace WebAlbum.Web.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private IAuthenticationManager _authenticationManager;
        private const string ErrorMsg = "An error has occurred while processing your request. ";

        public ApplicationSignInManager SignInManager
        {
            get => _signInManager ?? HttpContext.GetOwinContext()
                       .Get<ApplicationSignInManager>();
            private set => _signInManager = value;
        }

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext()
                       .GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        public IAuthenticationManager AuthenticationManager
        {
            get => _authenticationManager ?? HttpContext
                       .GetOwinContext().Authentication;
            private set => _authenticationManager = value;
        }

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager,
            ApplicationSignInManager signInManager, 
            IAuthenticationManager authenticationManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            AuthenticationManager = authenticationManager;
        }     

        public ActionResult Index()
        {
            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
            };
            return View(model);
        }

        public ActionResult Delete()
        {
            return View();
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed()
        {
            try
            {
                var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                var user = await UserManager.FindByIdAsync(userId);
                var userRoles = await UserManager.GetRolesAsync(userId);

                foreach (var i in userRoles)
                {
                    if (!i.Equals(Role.Admin))
                    {
                        await UserManager.RemoveFromRoleAsync(user.Id, i);
                    }
                }
                var userRolesUpdated = await UserManager.GetRolesAsync(userId);
                if (userRolesUpdated.Count > 0)
                {
                    ViewBag.Message = "You can not delete Admin Page!";
                }
                else
                {
                    var result = await UserManager.DeleteAsync(user);
                    if (result.Succeeded)
                    {
                        AuthenticationManager.SignOut();
                        return RedirectToAction("Index", "Home");
                    }
                    AddErrors(result);
                }     
            }
            catch (Exception e)
            {
                ViewBag.Message = ErrorMsg + e.Message;
                                  
            }

            return View();
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), 
                    model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInAsync(user, false);
                    }

                    ViewBag.Message = "Your password has been changed.";
                    return View("Index");
                }
                AddErrors(result);
            }
            catch (Exception e)
            {
                ViewBag.Message = ErrorMsg + e.Message;
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, 
                DefaultAuthenticationTypes.TwoFactorCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties
            {
                IsPersistent = isPersistent
            }, await user.GenerateUserIdentityAsync(UserManager));
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            return user?.PasswordHash != null;
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }
    }
}