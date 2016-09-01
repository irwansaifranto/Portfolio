using System.Web.Mvc;
using System.Web.Security;
using SecurityGuard.Services;
using SecurityGuard.Interfaces;
using SecurityGuard.ViewModels;
using WebUI.Controllers;
using Business.Abstract;
using Business.Concrete;
using WebUI.Infrastructure;

namespace WebUI.Areas.SecurityGuard.Controllers
{
    //[Authorize(Roles = "SecurityGuard")]
    [AuthorizeUser(ModuleName = "Security Guard")]
    public partial class DashboardController : BaseController
    {

        #region ctors

        private IMembershipService membershipService;
        private IRoleService roleService;
        private IActionRepository actionRepo;
        private IModuleRepository moduleRepo;
        //private IJabatanRepository jabatanRepo;

        public DashboardController()
        {
            this.roleService = new RoleService(Roles.Provider);
            this.membershipService = new MembershipService(Membership.Provider);
            this.actionRepo = new EFActionRepository();
            this.moduleRepo = new EFModuleRepository();
            //this.jabatanRepo = new EFJabatanRepository();
        }

        #endregion


        public virtual ActionResult Index()
        {
            DashboardViewModel viewModel = new DashboardViewModel();
            int totalRecords;

            membershipService.GetAllUsers(0, 20, out totalRecords);
            viewModel.TotalUserCount = totalRecords.ToString();
            viewModel.TotalUsersOnlineCount = membershipService.GetNumberOfUsersOnline().ToString();
            viewModel.TotalRolesCount = roleService.GetAllRoles().Length.ToString();
            viewModel.TotalActionsCount = actionRepo.Find().Count.ToString();
            viewModel.TotalModulesCount = moduleRepo.Find().Count.ToString();
            //viewModel.TotalJabatanCount = jabatanRepo.Find().Count.ToString();

            return View(viewModel);
        }

    }
}
