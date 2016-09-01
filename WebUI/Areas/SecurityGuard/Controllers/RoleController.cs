using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using SecurityGuard.Services;
using SecurityGuard.Interfaces;
using SecurityGuard.ViewModels;
using WebUI.Controllers;
using Business.Abstract;
using Business.Entities;
using WebUI.Infrastructure;

namespace WebUI.Areas.SecurityGuard.Controllers
{
    //[Authorize(Roles="SecurityGuard")]
    [AuthorizeUser(ModuleName = "Security Guard")]
    public partial class RoleController : BaseController
    {

        #region ctors

        private readonly IRoleService roleService;
        private IModuleRepository moduleRepo;
        private IActionRepository actionRepo;
        private IRoleRepository roleRepo;
        private IModulesInRoleRepository repoModulesInRole;

        public RoleController(IModuleRepository repoParam, IActionRepository repoActionParam, IRoleRepository repoRole, IModulesInRoleRepository repoModulesInRole)
        {
            this.roleService = new RoleService(Roles.Provider);
            this.moduleRepo = repoParam;
            this.actionRepo = repoActionParam;
            this.roleRepo = repoRole;
            this.repoModulesInRole = repoModulesInRole;
        }

        #endregion

        public virtual ActionResult Index()
        {
            ManageRolesViewModel model = new ManageRolesViewModel();
            model.Roles = new SelectList(roleService.GetAllRoles());
            model.RoleList = roleService.GetAllRoles();

            return View(model);
        }

        #region Create Roles Methods

        [HttpGet]
        public virtual ActionResult CreateRole()
        {
            return View(new RoleViewModel());
        }

        [HttpPost]
        public virtual ActionResult CreateRole(string roleName)
        {
            JsonResponse response = new JsonResponse();

            if (string.IsNullOrEmpty(roleName))
            {
                response.Success = false;
                response.Message = "You must enter a role name.";
                response.CssClass = "red";

                return Json(response);
            }

            try
            {
                roleService.CreateRole(roleName);

                if (Request.IsAjaxRequest())
                {
                    response.Success = true;
                    response.Message = "Role created successfully!";
                    response.CssClass = "green";

                    return Json(response);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                if (Request.IsAjaxRequest())
                {
                    response.Success = false;
                    response.Message = ex.Message;
                    response.CssClass = "red";

                    return Json(response);
                }

                ModelState.AddModelError("", ex.Message);
            }

            return RedirectToAction("Index");
        }

        #endregion

        #region Delete Roles Methods

        /// <summary>
        /// This is an Ajax method.
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual ActionResult DeleteRole(string roleName)
        {
            JsonResponse response = new JsonResponse();

            if (string.IsNullOrEmpty(roleName))
            {
                response.Success = false;
                response.Message = "You must select a Role Name to delete.";
                response.CssClass = "red";

                return Json(response);
            }

            roleService.DeleteRole(roleName);

            response.Success = true;
            response.Message = roleName + " was deleted successfully!";
            response.CssClass = "green";

            return Json(response);
        }

        [HttpPost]
        public ActionResult DeleteRoles(string roles, bool throwOnPopulatedRole)
        {
            JsonResponse response = new JsonResponse();
            response.Messages = new List<ResponseItem>();

            if (string.IsNullOrEmpty(roles))
            {
                response.Success = false;
                response.Message = "You must select at least one role.";
                return Json(response);
            }

            string[] roleNames = roles.Split(',');
            StringBuilder sb = new StringBuilder();

            ResponseItem item = null;

            foreach (var role in roleNames)
            {
                if (!string.IsNullOrEmpty(role))
                {
                    try
                    {
                        string[] user = roleService.GetUsersInRole(role);
                        if((user.Length <= 0)||(!throwOnPopulatedRole))
                            repoModulesInRole.DeleteByRole(roleRepo.FindByName(role).role_id);
                        roleService.DeleteRole(role, throwOnPopulatedRole);

                        

                        item = new ResponseItem();
                        item.Success = true;
                        item.Message = "Deleted this role successfully - " + role;
                        item.CssClass = "green";
                        response.Messages.Add(item);

                        //sb.AppendLine("Deleted this role successfully - " + role + "<br />");
                    }
                    catch (System.Configuration.Provider.ProviderException ex)
                    {
                        //sb.AppendLine(role + " - " + ex.Message + "<br />");

                        item = new ResponseItem();
                        item.Success = false;
                        item.Message = ex.Message;
                        item.CssClass = "yellow";
                        response.Messages.Add(item);
                    }
                }
            }

            response.Success = true;
            response.Message = sb.ToString();

            return Json(response);
        }

        #endregion

        #region Get Users In Role methods

        /// <summary>
        /// This is an Ajax method that populates the 
        /// Roles drop down list.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllRoles()
        {
            var list = roleService.GetAllRoles();

            List<SelectObject> selectList = new List<SelectObject>();

            foreach (var item in list)
            {
                selectList.Add(new SelectObject() { caption = item, value = item });
            }

            return Json(selectList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetUsersInRole(string roleName)
        {
            var list = roleService.GetUsersInRole(roleName);

            return Json(list, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region add modules to role
        public virtual ActionResult AddModule(string role)
        {
            if (role == null)
            {
                return RedirectToAction("Index");
            }

            AddModulesToRoleViewModel model = new AddModulesToRoleViewModel();
            List<Modules> temp = new List<Modules>();
            string render = "";

            model.RoleName = role;
            model.Actions = actionRepo.Find();
            //var rootModule = moduleRepo.Find().FindAll(x => x.ParentModule == null);
            roles r = roleRepo.FindByName(role);

            //foreach (Module m in rootModule) {
            //    moduleRepo.GetAllChildrenInModule(m.ModuleName,ref temp, 0);
            //}

            //foreach (Module m in temp) {
            //    render += DrawRowModule(m.lvl, m, model.Actions, r);
            //}

            List<Modules> modules = moduleRepo.Find();
            List<ModuleViewModel> mvms = new ModuleViewModel().MapRecursive(modules);
            foreach (ModuleViewModel m in mvms)
            {
                render += DrawRowModule(m.Level, m, model.Actions, r);
            }

            model.Render = render;

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult AddModulesToRole(string[] modules, string roleName)
        {
            JsonResponse response = new JsonResponse();

            try
            {
                roleRepo.AddModuleAndAction(modules, roleName);

                if (Request.IsAjaxRequest())
                {
                    response.Success = true;
                    response.Message = "Module added successfully!";
                    response.CssClass = "green";

                    return Json(response);
                }

                return RedirectToAction("AddModule");
            }
            catch (Exception ex)
            {
                if (Request.IsAjaxRequest())
                {
                    response.Success = false;
                    response.Message = ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.InnerException.Message + "\r\n" + ex.InnerException.StackTrace;
                    response.CssClass = "red";

                    return Json(response);
                }

                ModelState.AddModelError("", ex.Message);
            }

            return RedirectToAction("AddModule");
        }

        #endregion

        //helper
        public string DrawRowModule(int level, ModuleViewModel m, List<Business.Entities.Actions> actions, roles role)
        {
            string collumn = "col-md-" + 9 / (actions.Count + 1);
            string style = "margin-left:" + 15 * level + "px";
            string row = @"<div class='row' style='border-bottom:1px solid #000'>
                            <div class='col-md-3'><div style='" + style + "'>" + m.ModuleName + @"</div></div>";

            foreach (Business.Entities.Actions a in actions)
            {
                bool found = false;

                if (m.Actions.Count > 0)
                {
                    ModulesInRoles mr = repoModulesInRole.FindByRoleAndModule(role.role_id, m.Id);
                    found = false;
                    if (mr != null)//check action
                    {
                        var enumerator = mr.Actions.GetEnumerator();
                        while (enumerator.MoveNext() && !found)
                        {
                            if (enumerator.Current.Id == a.Id)
                            {
                                found = true;
                            }
                        }
                    }
                    if (found)
                        row += "<div class='" + collumn + @"'><input type='checkbox' checked='checked' value='" + m.Id + ";" + a.Id + "' /></div>";
                    else
                        row += "<div class='" + collumn + @"'><input type='checkbox' value='" + m.Id + ";" + a.Id + "' /></div>";
                }
                else
                {
                    row += "<div class='" + collumn + @"'>-</div>";
                }
            }

            row += " </div>";

            return row;
        }
    }
}
