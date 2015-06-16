using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ForumApiTest.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security;


namespace ForumApiTest.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email, RegisterTime = DateTime.Now };

            IdentityResult result = await UserManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        //POST api/Account/AddInfo
        [Route("AddInfo")]
        public async Task<IHttpActionResult> AddInfo(UserInfoBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string userId = User.Identity.GetUserId();
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                var userInfo = context.AspNetUserInfo.Where(u => u.id == userId).FirstOrDefault();
                if (userInfo == null)
                {
                    //add user info
                    userInfo = new ApplicationUserInfo();
                    userInfo.id = userId;
                    userInfo.Nickname = model.Nickname;
                    userInfo.Signature = model.Signature;
                    userInfo.Avatar = model.Avatar;
                    userInfo.ArticleCount = model.ArticleCount;
                    userInfo.FellowCount = model.FellowCount;
                    userInfo.UpdateTime = DateTime.Now;
                    context.AspNetUserInfo.Add(userInfo);
                    context.SaveChanges();
                }
                else
                {
                    //update user info
                    userInfo.Nickname = model.Nickname;
                    userInfo.Signature = model.Signature;
                    userInfo.Avatar = model.Avatar;
                    userInfo.ArticleCount = model.ArticleCount;
                    userInfo.FellowCount = model.FellowCount;
                    userInfo.UpdateTime = DateTime.Now;
                    context.SaveChanges();
                }
            }
            return Ok();
        }

        //GET api/Account/GetInfo
        [Route("GetInfo")]
        public async Task<UserInfoBindingModel> GetInfo(UserInfoBindingModel model)
        {
            string userId = User.Identity.GetUserId();
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                var userInfo = context.AspNetUserInfo.Where(u => u.id == userId).FirstOrDefault();
                if (userInfo == null)
                    return null;
                else
                    return new UserInfoBindingModel
                    {
                        Nickname = userInfo.Nickname,
                        Signature = userInfo.Signature,
                        Avatar = userInfo.Avatar,
                        ArticleCount = userInfo.ArticleCount,
                        FellowCount = userInfo.FellowCount,
                        UpdateTime = userInfo.UpdateTime
                    };
            }
        }

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

    }
}
