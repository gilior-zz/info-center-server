using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.Configuration;


namespace WebApplication55
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private BL bl;
        public BL BL
        {
            get
            {
                if (this.bl == null)
                    this.bl = new BL();
                return this.bl;
            }
        }
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            RegisterWebApiFilters(GlobalConfiguration.Configuration.Filters);
            log4net.Config.XmlConfigurator.Configure();
            WebCacheManager.Instance.SetValue("files", this.Server.MapPath("~/Files"));
            this.BL.LoadData();
        }

        public static void RegisterWebApiFilters(System.Web.Http.Filters.HttpFilterCollection filters)
        {
            filters.Add(new LogAttribute());
        }


    }
}
