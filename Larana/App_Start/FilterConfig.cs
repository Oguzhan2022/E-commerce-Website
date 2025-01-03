using System.Web.Mvc;

namespace Larana
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new AuthorizeAttribute());

            filters.Add(new RequireHttpsAttribute());
        }
    }
}
