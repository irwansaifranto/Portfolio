using System.Web.Mvc;

namespace WebUI.Areas.Dummy
{
    public class DummyAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Dummy";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Dummy_default",
                "Dummy/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
