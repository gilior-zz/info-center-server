using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceProcess;
using System.Web.Http;

namespace WebApplication55.Controllers
{
    public class notificationController : ApiController
    {
        // GET api/<controller>
        public KeyValuePair<int, int>[] Get()
        {
            var flat = WebCacheManager.Instance.GetValue<DanelVersion[]>("flat-vers");
            Dictionary<int, int> statuses = new Dictionary<int, int>();
            foreach (var item in flat)
            {
                statuses.Add(item.id, -1);
                ServiceController sc = new ServiceController(item.winNotificationName, item.serverName);
                try
                {
                    statuses[item.id] = (int)sc.Status;
                }
                catch (Exception) { }
            }
            var toArray = statuses.ToArray();
            return toArray;
        }

        // GET api/<controller>/5

        public string Get(string id)
        {
            string res = "not exists";
            var v = id.Replace("^", ".").Split(';');
            var service = v[0];
            var server = v[1];

            ServiceController sc = new ServiceController(service, server);
            try
            {
                res = sc.Status.ToString();
            }
            catch (Exception) { }

            return res;
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public int Put(int id, [FromBody] ServiceControllerStatus toStatus)
        {
            try
            {
                var v = WebCacheManager.Instance.GetValue<IGrouping<Version, DanelVersion>[]>("vers");
                var item = v.SelectMany(i => i).FirstOrDefault(i => i.id == id);
                ServiceController sc = new ServiceController(item.winNotificationName, item.serverName);
                switch (toStatus)
                {

                    case ServiceControllerStatus.Running:
                        sc.Start();
                        sc.WaitForStatus(ServiceControllerStatus.Running);
                        break;

                    case ServiceControllerStatus.Stopped:
                        sc.Stop();
                        sc.WaitForStatus(ServiceControllerStatus.Stopped);
                        break;
                }
                return (int)toStatus;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}