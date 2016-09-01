using Business.Abstract;
using Business.Concrete;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebUI.Infrastructure.Abstract;
using WebUI.Infrastructure.Concrete;

namespace WebUI.Infrastructure
{
    public class NinjectDependencyResolver : System.Web.Mvc.IDependencyResolver
    {
        private IKernel kernel;

        public NinjectDependencyResolver()
        {
            kernel = new StandardKernel();
            AddBindings();
        }

        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }

        private void AddBindings()
        {
            //user management
            kernel.Bind<IActionRepository>().To<EFActionRepository>();
            kernel.Bind<IModuleRepository>().To<EFModuleRepository>();
            kernel.Bind<IRoleRepository>().To<EFRoleRepository>();
            kernel.Bind<IModulesInRoleRepository>().To<EFModulesInRoleRepository>();
            kernel.Bind<IUserRepository>().To<EFUserRepository>();

            //karental
            kernel.Bind<ICarBrandRepository>().To<EFCarBrandRepository>();
            kernel.Bind<ICarRepository>().To<EFCarRepository>();
            kernel.Bind<ICarModelRepository>().To<EFCarModelRepository>();
            kernel.Bind<ICustomerRepository>().To<EFCustomerRepository>();
            kernel.Bind<IOwnerRepository>().To<EFOwnerRepository>();
            kernel.Bind<IDriverRepository>().To<EFDriverRepository>();
            kernel.Bind<IInvoiceRepository>().To<EFInvoiceRepository>();
            kernel.Bind<IRentRepository>().To<EFRentRepository>();
            kernel.Bind<IExpenseRepository>().To<EFExpenseRepository>();
            kernel.Bind<IRentPositionRepository>().To<EFRentPositionRepository>();

            //others
            //kernel.Bind<ICompanyRepository>().To<EFCompanyRepository>();
            kernel.Bind<ILogRepository>().To<EFLogRepository>();
            kernel.Bind<IAuthProvider>().To<DummyAuthProvider>();
            kernel.Bind<ICityRepository>().To<EFCityRepository>();
            kernel.Bind<IDummyNotificationRepository>().To<EFDummyNotificationRepository>();
            kernel.Bind<ICarExpenseRepository>().To<EFCarExpenseRepository>();
            kernel.Bind<ICarPackageRepository>().To<EFCarPackageRepository>();
            kernel.Bind<IApiRentRepository>().To<EFApiRentRepository>();
        }
    }
}