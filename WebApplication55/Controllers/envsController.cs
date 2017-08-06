using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceProcess;
using System.Web.Http;

namespace WebApplication55.Controllers
{
    public class envsController : ApiController
    {
        // GET: api/envs
        ILog log = log4net.LogManager.GetLogger(typeof(envsController));
        public DanelVersionResponse Get()
        {

            DanelVersionResponse res = new WebApplication55.DanelVersionResponse();
            var v = WebCacheManager.Instance.GetValue<IGrouping<Version, DanelVersion>[]>("vers");
            var t = WebCacheManager.Instance.GetValue<DateTime>("versTime");
            res.time = t;
            res.vers = v;

            return res;
        }





        // GET: api/envs/5
        public DanelVersionResponse Get(int id)
        {
            DanelVersionResponse res = new WebApplication55.DanelVersionResponse();
            var v = WebCacheManager.Instance.GetValue<IGrouping<Version, DanelVersion>[]>("vers");
            var item = v.SelectMany(i => i).FirstOrDefault(i => i.id == id);
            if (item != null)
            {
                ServiceController sc = new ServiceController(item.winNotificationName, item.serverName);
                try
                {
                    item.winNotificationIsUp = sc.Status == ServiceControllerStatus.Running;
                    item.winNotificationStatus = (int)sc.Status;
                }
                catch (Exception) { item.winNotificationStatus = -1; }
                sc = new ServiceController(item.winServiceName, item.serverName);
                try
                {
                    item.winServiceIsUp = sc.Status == ServiceControllerStatus.Running;
                    item.winListenerStatus = (int)sc.Status;
                }
                catch (Exception) { item.winListenerStatus = -1; }
            }


            res.ver = item;
            return res;
        }

        // POST: api/envs
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/envs/5
        //0-stop 1-start 2-restart //winservice
        //10-stop 11-start 12-restart //notification
        public DanelVersionResponse Put(int id, [FromBody]DanelVersion value)
        {
            ServiceController sc;
            switch (id)
            {
                case 0:
                    sc = new ServiceController(value.winServiceName, value.serverName);
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped);
                    value.winServiceIsUp = false;
                    value.winListenerStatus = (int)ServiceControllerStatus.Stopped;
                    break;
                case 10:
                    sc = new ServiceController(value.winNotificationName, value.serverName);
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped);
                    value.winNotificationIsUp = false;
                    value.winNotificationStatus = (int)ServiceControllerStatus.Stopped;
                    break;
                case 1:
                    sc = new ServiceController(value.winServiceName, value.serverName);
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running);
                    value.winServiceIsUp = true;
                    value.winListenerStatus = (int)ServiceControllerStatus.Running;
                    break;
                case 11:
                    sc = new ServiceController(value.winNotificationName, value.serverName);
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running);
                    value.winNotificationIsUp = true;
                    value.winNotificationStatus = (int)ServiceControllerStatus.Running;
                    break;
                case 2:
                    sc = new ServiceController(value.winServiceName, value.serverName);
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped);
                    value.winListenerStatus = (int)ServiceControllerStatus.Stopped;
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running);
                    value.winServiceIsUp = true;
                    value.winListenerStatus = (int)ServiceControllerStatus.Running;
                    break;
                case 12:
                    sc = new ServiceController(value.winNotificationName, value.serverName);
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped);
                    value.winNotificationStatus = (int)ServiceControllerStatus.Stopped;
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running);
                    value.winNotificationIsUp = true;
                    value.winNotificationStatus = (int)ServiceControllerStatus.Running;
                    break;
            }
            return new DanelVersionResponse() { ver = value };
        }

        // DELETE: api/envs/5
        public void Delete(int id)
        {
        }
    }
}
