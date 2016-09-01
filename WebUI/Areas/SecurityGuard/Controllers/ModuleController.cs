using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using SecurityGuard.ViewModels;
using WebUI.Controllers;
using Business.Abstract;
using System.Data.SqlClient;
using Business.Entities;
using WebUI.Infrastructure;

namespace WebUI.Areas.SecurityGuard.Controllers
{
    //[Authorize(Roles="SecurityGuard")]
    [AuthorizeUser(ModuleName = "Security Guard")]
    public partial class ModuleController : BaseController
    {
        #region ctors

        private IModuleRepository moduleRepo;
        private IActionRepository actionRepo;
        private IModulesInRoleRepository modulesInRoleRepo;

        public ModuleController(IModuleRepository repoParam, IActionRepository repoActionParam, IModulesInRoleRepository repoModulesInRole)
        {
            this.moduleRepo = repoParam;
            this.actionRepo = repoActionParam;
            this.modulesInRoleRepo = repoModulesInRole;
        }

        #endregion

        public virtual ActionResult Index()
        {
            //kamus
            ManageModulesViewModel model = new ManageModulesViewModel();
            var modules = moduleRepo.Find();
            string[] actionsArr = new string[100];

            //algo
            actionsArr = new string[modules.Count];
            for(int i =0;i<modules.Count; ++i){
                actionsArr[i] = modules[i].ModuleName;
            }
            model.Modules = new SelectList(actionsArr);
            model.ModuleList = actionsArr;
            return View(model);
        }

        #region Create Actions Methods

        [HttpGet]
        public virtual ActionResult CreateModule()
        {
            return View(new ModuleViewModel());
        }

        [HttpPost]
        public virtual ActionResult CreateModule(string moduleName, string parentModule)
        {
            JsonResponse response = new JsonResponse();

            if (string.IsNullOrEmpty(moduleName))
            {
                response.Success = false;
                response.Message = "You must enter a module name.";
                response.CssClass = "red";

                return Json(response);
            }

            try
            {
                Modules a = new Modules();
                a.Id = Guid.NewGuid();
                a.ModuleName = moduleName;
                if(!string.IsNullOrWhiteSpace(parentModule))
                    a.ParentModule = parentModule;
                moduleRepo.Create(a);
                
                if (Request.IsAjaxRequest())
                {
                    response.Success = true;
                    response.Message = "Module created successfully!";
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
                    response.Message = ex.InnerException.Message;
                    response.CssClass = "red";

                    return Json(response);
                }

                ModelState.AddModelError("", ex.InnerException.Message);
            }

            return RedirectToAction("Index");
        }

        #endregion

        #region Get treeview binding
        [HttpGet]
        public virtual ActionResult GetModuleBinding(string moduleName){
            List<Modules> tempModules;
            List<Modules> modules = moduleRepo.Find();
            List<ModuleViewModel> response = new List<ModuleViewModel>();
            bool hasChildren = false;

            tempModules = modules;
            if (moduleName == null)
            {
                modules = modules.FindAll(x => x.ParentModule == null);
            }
            else {
                modules = modules.FindAll(x => x.ParentModule == moduleName);
            }

            for (int i=0; i < modules.Count; ++i) {
                hasChildren = false;
                int countChildren = tempModules.FindAll(x => x.ParentModule == modules[i].ModuleName).Count;
                if(countChildren > 0){
                    hasChildren = true;
                }

                ModuleViewModel m = new ModuleViewModel()
                {
                    ModuleName = modules[i].ModuleName,
                    ParentModule = modules[i].ParentModule,
                    HasChildren = hasChildren
                };
                response.Add(m);
            }

            return Json(response.ToArray(), JsonRequestBehavior.AllowGet);
        } 
        

        #endregion

        #region add Action to Module

        /// <summary>
        /// Return two lists:
        ///   1)  a list of Actions not in module
        ///   2)  a list of Actions in module
        /// </summary>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        [HttpGet]
        public virtual ActionResult AddAction(string moduleName)
        {
            if (string.IsNullOrEmpty(moduleName))
            {
                return RedirectToAction("Index");
            }

            AddActionsToModuleViewModel model = new AddActionsToModuleViewModel();
            model.ModuleName = moduleName;
            model.GUID = moduleRepo.FindByName(moduleName).Id.ToString();

            List<Business.Entities.Actions> availableActions = actionRepo.Find();
            List<Business.Entities.Actions> usedActions = moduleRepo.GetActionsInModule(moduleName);

            //used action
            foreach (Business.Entities.Actions a in usedActions)
            {
                availableActions.RemoveAll(x => x.ActionName == a.ActionName);
            }

            model.AvailableActions = new SelectList(availableActions,"Id","ActionName");
            model.AddedActions = new SelectList(usedActions, "Id", "ActionName");

            return View(model);
        }

        public virtual ActionResult GrantActionsToModule(string moduleId, string actions) {
            JsonResponse response = new JsonResponse();
            response.Messages = new List<ResponseItem>();

            string[] actionIds = actions.Split(',');
            StringBuilder sb = new StringBuilder();

            ResponseItem item = null;

            foreach (string s in actionIds)
            {
                if (!string.IsNullOrWhiteSpace(s))
                {
                    try
                    {
                        moduleRepo.addAction(new Guid(moduleId), new Guid(s));

                        item = new ResponseItem();
                        item.Success = true;
                        response.Message = actionRepo.FindByPk(new Guid(s)).ActionName + " was added successfully!";
                        response.CssClass = "green";
                        response.Messages.Add(item);
                    }
                    catch (Exception ex)
                    {
                        item = new ResponseItem();
                        item.Success = false;
                        response.Success = false;
                        response.Message = ex.Message;
                        response.CssClass = "red";
                        response.Messages.Add(item);
                    }
                }
            }

            return Json(response);
        }

        public virtual ActionResult ViewModuleAction()
        {
            ViewBag.modules = moduleRepo.Find();
            ViewBag.actions = actionRepo.Find();
            return PartialView();
        }

        public virtual ActionResult RevokeActionsForModule(string moduleId, string actions)
        {
            JsonResponse response = new JsonResponse();
            response.Messages = new List<ResponseItem>();

            string[] actionIds = actions.Split(',');
            StringBuilder sb = new StringBuilder();

            ResponseItem item = null;

            foreach (string s in actionIds)
            {
                if (!string.IsNullOrWhiteSpace(s))
                {
                    try
                    {
                        //remove action in repo
                        moduleRepo.removeAction(new Guid(moduleId), new Guid(s));

                        //remove action in ModulesInRole
                        modulesInRoleRepo.RemoveAction(new Guid(moduleId), new Guid(s));

                        //remove all empty actions in ModulesInRole
                        modulesInRoleRepo.DeleteByModule(new Guid(moduleId));

                        item = new ResponseItem();
                        item.Success = true;
                        response.Message = actionRepo.FindByPk(new Guid(s)).ActionName + " was removed successfully!";
                        response.CssClass = "green";
                        response.Messages.Add(item);
                    }
                    catch (Exception ex)
                    {
                        item = new ResponseItem();
                        item.Success = false;
                        response.Success = false;
                        response.Message = ex.Message;
                        response.CssClass = "red";
                        response.Messages.Add(item);
                    }
                }
            }

            return Json(response);
        }

        #endregion

        #region Delete Roles Methods

        /// <summary>
        /// This is an Ajax method.
        /// </summary>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual ActionResult DeleteModule(string moduleName)
        {
            JsonResponse response = new JsonResponse();

            if (string.IsNullOrEmpty(moduleName))
            {
                response.Success = false;
                response.Message = "You must select a Action Module to delete.";
                response.CssClass = "red";

                return Json(response);
            }
            try
            {
                moduleRepo.Delete(moduleName, false);

                response.Success = true;
                response.Message = moduleName + " was deleted successfully!";
                response.CssClass = "green";

                return Json(response);
            }
            catch (Exception ex)
            {      
                response.Success = false;
                response.Message = ex.InnerException.Message;
                response.CssClass = "red";

                return Json(response);
            }
        }

        [HttpPost]
        public ActionResult DeleteModules(string moduleName, bool throwOnPopulatedModule)
        {
            JsonResponse response = new JsonResponse();
            response.Messages = new List<ResponseItem>();

            if (string.IsNullOrEmpty(moduleName))
            {
                response.Success = false;
                response.Message = "You must select at least one module.";
                return Json(response);
            }

            StringBuilder sb = new StringBuilder();

            ResponseItem item = null;

            
            if (!string.IsNullOrEmpty(moduleName))
            {
                try
                {
                    moduleRepo.Delete(moduleName, throwOnPopulatedModule);//roles not yet deleted

                    item = new ResponseItem();
                    item.Success = true;
                    item.Message = "Deleted this module successfully - " + moduleName;
                    item.CssClass = "green";
                    response.Messages.Add(item);

                    //sb.AppendLine("Deleted this role successfully - " + role + "<br />");
                }
                catch (SqlException ex)
                {
                    //sb.AppendLine(role + " - " + ex.Message + "<br />");

                    item = new ResponseItem();
                    item.Success = false;
                    item.Message = ex.InnerException.Message;
                    item.CssClass = "yellow";
                    response.Messages.Add(item);
                }
            }
            
            response.Success = true;
            response.Message = sb.ToString();

            return Json(response);
        }

        #endregion

        #region Get Actions In Module methods

        /// <summary>
        /// This is an Ajax method that populates the 
        /// Actions drop down list.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllModule()
        {
            var list = moduleRepo.Find();

            List<SelectObject> selectList = new List<SelectObject>();

            selectList.Add(new SelectObject() { caption = "[None]", value = "[None]" });
            foreach (Modules item in list)
            {
                selectList.Add(new SelectObject() { caption = item.ModuleName, value = item.ModuleName.ToString() });
            }
            
            return Json(selectList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetListModule()
        {
            List<Modules> tempModules = new List<Modules>();
            List<ModuleViewModel> result = new List<ModuleViewModel>();
            List<Modules> modules = moduleRepo.Find();
            //modules = modules.FindAll(x => x.ParentModule == null);

            //foreach (Module m in modules)
            //{
            //    tempModules.Add(m);
            //    moduleRepo.GetAllChildrenInModule(m.ModuleName, ref tempModules, 0);
            //}

            //foreach (Module m in tempModules)
            //{
            //    result.Add(new ModuleViewModel(m));
            //}

            result = new ModuleViewModel().MapRecursive(modules);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetActionsInModule(string moduleName)
        {
            var list = moduleRepo.GetActionsInModule(moduleName);

            return Json(list, JsonRequestBehavior.AllowGet);
        }


        #endregion
    }
}
