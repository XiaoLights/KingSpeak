using System.Web;
using System.Web.Mvc;

namespace Kingspeak.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new Kingspeak.MyController.MyAuthorizationAttribute());
            filters.Add(new Kingspeak.MyController.AboutErrorAttribute());
         


        }
    }
}
