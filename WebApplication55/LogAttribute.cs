using log4net;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
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
            var userName = filterContext.Request.Headers.FirstOrDefault(i => i.Key == "userName").Value.FirstOrDefault();
            try
            {


                StringBuilder actionParam = new StringBuilder();
                foreach (var item in filterContext.ActionArguments)
                {
                    String val = "";
                    if (!item.Value.GetType().IsClass)
                        actionParam.Append(item.Key).Append("-").Append(item.Value.ToString()).Append(",");
                    else
                        foreach (PropertyInfo propertyInfo in item.Value.GetType().GetProperties())
                        {
                            if (propertyInfo.CanRead)
                            {
                                val = propertyInfo.GetValue(item.Value, null)?.ToString() ?? "null";
                                actionParam.Append(item.Key).Append("-").Append(val).Append(",");
                            }
                        }



                }
                string str = $@"{filterContext.ActionDescriptor.ActionName} {actionParam.ToString()} {userName}";
                log.Info(str);
            }
            catch (Exception)
            {

                log.Error($"log error for user {userName}");
            }


        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            //var v = actionExecutedContext.ActionContext.ControllerContext.Controller.GetType();
            //ILog log = log4net.LogManager.GetLogger(v);
            //StringBuilder actionParam = new StringBuilder();
            //foreach (var item in actionExecutedContext.ActionContext.ActionArguments)
            //{
            //    actionParam.Append(item.Value).Append(",");
            //}
            //string str = $@"{actionExecutedContext.ActionContext.ActionDescriptor.ActionName} {actionParam.ToString()}  {actionExecutedContext.ActionContext.Request.Headers.FirstOrDefault(i => i.Key == "userName").Value.FirstOrDefault()}";
            //log.Info("started " + str);
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