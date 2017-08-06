using log4net;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace WebApplication55
{
    public class LogAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext filterContext)
        {

            var v = filterContext.ControllerContext.Controller.GetType();
            ILog log = log4net.LogManager.GetLogger(v);
            log.Info("started " + filterContext.ActionDescriptor.ActionName + "IP " + this.GetClientIpAddress(filterContext.Request));
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var v = actionExecutedContext.ActionContext.ControllerContext.Controller.GetType();
            ILog log = log4net.LogManager.GetLogger(v);
            log.Info("end " + actionExecutedContext.ActionContext.ActionDescriptor.ActionName +"IP " + this.GetClientIpAddress(actionExecutedContext.Request));
        }

        public string GetClientIpAddress(HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return IPAddress.Parse(((HttpContextBase)request.Properties["MS_HttpContext"]).Request.UserHostAddress).ToString();
            }
            if (request.Properties.ContainsKey("MS_OwinContext"))
            {
                return IPAddress.Parse(((OwinContext)request.Properties["MS_OwinContext"]).Request.RemoteIpAddress).ToString();
            }
            return null;
        }
    }
}