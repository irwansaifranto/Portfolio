using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebUI.Infrastructure.Abstract;
using WebUI.Models;
using WebUI.Extension;

namespace WebUI.Controllers
{
    public class AccountController : Controller
    {
        private IAuthProvider auth;

        public AccountController(IAuthProvider authParam)
        {
            auth = authParam;
        }

        public ActionResult Login(string ReturnUrl = null)
        {
            if (Request.IsAuthenticated)
                return RedirectToAction("Index", "Dashboard");
            else
            {
                if (ReturnUrl != null)
                    ViewBag.ReturnUrl = ReturnUrl;
                return View();
            }
        }

        [HttpPost]
        public ActionResult Login(LoginModel model, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                if (auth.Authenticate(model.Username, model.Password))
                {
                    //Session["username"] = model.Username;

                    FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                        1,
                        model.Username,  //user id
                        DateTime.Now,
                        DateTime.Now.AddMinutes(60),  // expiry
                        false,  //do not remember
                        "/");
                    HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName,
                                                       FormsAuthentication.Encrypt(authTicket))
                                                       {
                                                           HttpOnly = true,
                                                           Expires = authTicket.Expiration
                                                       };
                    //Response.Cookies.Add(cookie);
					Response.SetCookie(cookie);
                    //redirect ke halaman sebelumnya
                    if (ReturnUrl != null)
                        return Redirect(ReturnUrl);
                    else
                        return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    ModelState.AddModelError("", auth.Message);
                };
            }
            return View();
        }

        /// <summary>
        /// clear session
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            auth.Logout();

            Session["userId"] = null;
            Session["username"] = null;
            Session["rolename"] = null;

            FormsAuthentication.SignOut();

            //return RedirectToAction("Login", "Account", new { area = "" });
            //FormsAuthentication.RedirectToLoginPage("returnUrl=/pmo");
            //return Redirect(FormsAuthentication.LoginUrl + "?returnUrl=/");
            return Redirect(FormsAuthentication.LoginUrl);
        }

        //[Authorize(Roles = "User, Admin")]
        //public ActionResult Edit()
        //{
        //    string username = (string)Session["username"];
        //    User user = repo.FindByPk(username);
        //    if (user == null)
        //    {
        //        if (repo.ResponseCode == Utilities._FAIL_UNAUTHORIZE)
        //        {
        //            FormsAuthentication.SignOut();
        //            return Redirect(FormsAuthentication.LoginUrl);
        //        }
        //    }

        //    //kamus

        //    //algoritma
        //    user.Password = "";

        //    //ViewBag
        //    ViewBag.PageTitle = "Edit Profile";
        //    ViewBag.action = "edit";

        //    return View("Form", user);
        //}

        //[Authorize(Roles = "User, Admin")]
        //[HttpPost]
        //public ActionResult Edit(User user)
        //{
        //    //kamus
        //    bool loadView = false;

        //    //validation
        //    ModelState.Remove("Password");
        //    if (ModelState.IsValid)
        //    {
        //        if (user.Password != "")
        //        {
        //            user.Password = Utilities.Sha256_hash(user.Password);
        //        }
        //        repo.EditSelf(user);

        //        if (repo.ResponseCode == Utilities._SUCCESS) //berhasil
        //        {
        //            TempData[AppMessage._SESS_MESSSAGE] = "User " + user.Username + " berhasil diubah";

        //            return RedirectToAction("Detail");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", repo.ResponseMessage);
        //            user.Password = "";
        //            loadView = true;
        //        }
        //    }
        //    else
        //    {
        //        user.Password = "";
        //        loadView = true;
        //    }

        //    //algoritma

        //    //ViewBag
        //    ViewBag.PageTitle = "Edit User - " + user.Username;
        //    ViewBag.action = "edit";

        //    return View("Form", user);
        //}

        //[Authorize(Roles = "User, Admin")]
        //public ActionResult EditPassword()
        //{
        //    //kamus
        //    EditPasswordViewModel model = new EditPasswordViewModel();

        //    //ViewBag
        //    ViewBag.PageTitle = "Edit Password";
        //    ViewBag.action = "edit";

        //    return View("FormPassword", model);
        //}

        //[Authorize(Roles = "User, Admin")]
        //[HttpPost]
        //public ActionResult EditPassword(EditPasswordViewModel model)
        //{
        //    //kamus
        //    bool loadView = false;
        //    string username = (string)Session["username"];

        //    //validation
        //    if (ModelState.IsValid)
        //    {
        //        model.OldPassword = Utilities.Sha256_hash(model.OldPassword);
        //        model.Password = Utilities.Sha256_hash(model.Password);

        //        repo.EditPassword(username, model.OldPassword, model.Password);

        //        if (repo.ResponseCode == Utilities._SUCCESS) //berhasil
        //        {
        //            TempData[AppMessage._SESS_MESSSAGE] = "Password " + username + " berhasil diubah";

        //            return RedirectToAction("Detail");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", repo.ResponseMessage);
        //            model.OldPassword = "";
        //            model.Password = "";
        //            model.RepeatPassword = "";
        //            loadView = true;
        //        }
        //    }
        //    else
        //    {
        //        model.OldPassword = "";
        //        model.Password = "";
        //        model.RepeatPassword = "";
        //        loadView = true;
        //    }

        //    //algoritma

        //    //ViewBag
        //    ViewBag.PageTitle = "Edit Password";
        //    ViewBag.action = "edit";

        //    return View("FormPassword", model);
        //}

        //[Authorize(Roles="User, Admin")]
        //public ActionResult Detail()
        //{
        //    //kamus
        //    string username = (string) Session["username"];
        //    User user = null;
        //    Department department = new Department();
        //    Position position = new Position();
        //    Planner planner = new Planner();
        //    ReleaseCodePR releaseCodePR = null;
        //    ReleaseCodeSE releaseCodeSE = null;

        //    //algoritma
        //    user = repo.FindByPk(username);
        //    if (user == null)
        //    {
        //        if (repo.ResponseCode == Utilities._FAIL_UNAUTHORIZE)
        //        {
        //            FormsAuthentication.SignOut();
        //            return Redirect(FormsAuthentication.LoginUrl);
        //        }
        //        else
        //        {
        //            throw new HttpException(404, repo.ResponseMessage);
        //        }
        //    }

        //    if (user.DepartmentCode != null)
        //    {
        //        department = repoDepartment.FindByPk(user.DepartmentCode);
        //    }
        //    if (user.Position != null)
        //    {
        //        position = repoPosition.FindByPk(user.Position);
        //    }
        //    if (user.ReleaseCodePr != null)
        //    {
        //        releaseCodePR = repoReleaseCodePR.FindByPk(user.ReleaseCodePr);
        //    }
        //    if (user.ReleaseCodeSe != null)
        //    {
        //        releaseCodeSE = repoReleaseCodeSE.FindByPk(user.ReleaseCodeSe);
        //    }

        //    //ViewBag
        //    ViewBag.PageTitle = "My Profile";
        //    ViewBag.department = department;
        //    ViewBag.position = position;
        //    ViewBag.planner = planner;
        //    ViewBag.ReleaseCodePR = releaseCodePR;
        //    ViewBag.ReleaseCodeSE = releaseCodeSE;

        //    return View(user);
        //}
    }
}
