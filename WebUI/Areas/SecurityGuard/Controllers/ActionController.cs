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
using System.Data.SqlClient;
using WebUI.Infrastructure;

namespace WebUI.Areas.SecurityGuard.Controllers
{
    [AuthorizeUser(ModuleName = "Security Guard")]
    public partial class ActionController : BaseController
    {
        #region ctors

        private IActionRepository actionRepo;

        public ActionController(IActionRepository repoParam)
        {
            this.actionRepo = repoParam;
        }

        #endregion

        public virtual ActionResult Index()
        {
            //kamus
            ManageActionsViewModel model = new ManageActionsViewModel();
            var actions = actionRepo.Find();
            string[] actionsArr;

            //algo
            actionsArr = new string[actions.Count];
            for(int i =0;i<actions.Count; ++i){
                actionsArr[i] = actions[i].ActionName;
            }
            model.Actions = new SelectList(actionsArr);
            model.ActionList = actionsArr;
            return View(model);
        }

        #region Create Actions Methods

        [HttpGet]
        public virtual ActionResult CreateAction()
        {
            return View(new ActionViewModel());
        }

        [HttpPost]
        public virtual ActionResult CreateAction(string actionName)
        {
            JsonResponse response = new JsonResponse();

            if (string.IsNullOrEmpty(actionName))
            {
                response.Success = false;
                response.Message = "You must enter a action name.";
                response.CssClass = "red";

                return Json(response);
            }

            try
            {
                Business.Entities.Actions a = new Business.Entities.Actions();
                a.Id = Guid.NewGuid();
                a.ActionName = actionName;
                actionRepo.Create(a);
                
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
                    response.Message = ex.InnerException.Message;
                    response.CssClass = "red";

                    return Json(response);
                }

                ModelState.AddModelError("", ex.InnerException.Message);
            }

            return RedirectToAction("Index");
        }

        #endregion

        #region Delete Roles Methods

        /// <summary>
        /// This is an Ajax method.
        /// </summary>
        /// <param name="actionName"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual ActionResult DeleteAction(string actionName)
        {
            JsonResponse response = new JsonResponse();

            if (string.IsNullOrEmpty(actionName))
            {
                response.Success = false;
                response.Message = "You must select a Action Name to delete.";
                response.CssClass = "red";

                return Json(response);
            }
            try
            {
                actionRepo.Delete(actionName, true);

                response.Success = true;
                response.Message = actionName + " was deleted successfully!";
                response.CssClass = "green";

                return Json(response);
            }
            catch (Exception ex)
            {      
                response.Success = false;
                response.Message = "Action was used in modules";
                response.CssClass = "red";

                return Json(response);
            }
        }

        [HttpPost]
        public ActionResult DeleteActions(string actions, bool throwOnPopulatedAction)
        {
            JsonResponse response = new JsonResponse();
            response.Messages = new List<ResponseItem>();

            if (string.IsNullOrEmpty(actions))
            {
                response.Success = false;
                response.Message = "You must select at least one action.";
                return Json(response);
            }

            string[] actionNames = actions.Split(',');
            StringBuilder sb = new StringBuilder();

            ResponseItem item = null;

            foreach (var action in actionNames)
            {
                if (!string.IsNullOrEmpty(action))
                {
                    try
                    {
                        actionRepo.Delete(action, throwOnPopulatedAction);//module not yet deleted

                        item = new ResponseItem();
                        item.Success = true;
                        item.Message = "Deleted this action successfully - " + action;
                        item.CssClass = "green";
                        response.Messages.Add(item);

                        //sb.AppendLine("Deleted this role successfully - " + role + "<br />");
                    }
                    catch (Exception ex)
                    {
                        //sb.AppendLine(role + " - " + ex.Message + "<br />");

                        item = new ResponseItem();
                        item.Success = false;
                        item.Message = "Action was used in modules";
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

        #region Get Modules In Action methods

        /// <summary>
        /// This is an Ajax method that populates the 
        /// Actions drop down list.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllAction()
        {
            var list = actionRepo.Find();

            List<SelectObject> selectList = new List<SelectObject>();

            foreach (Business.Entities.Actions item in list)
            {
                selectList.Add(new SelectObject() { caption = item.ActionName, value = item.ActionName.ToString() });
            }

            return Json(selectList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetModulesInAction(string actionName)
        {
            var list = actionRepo.GetModulesInAction(actionName);

            return Json(list, JsonRequestBehavior.AllowGet);
        }


        #endregion
    }
}
