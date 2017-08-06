using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApplication55.Controllers
{
    public class mdlsController : ApiController
    {
        // GET: api/mdls
        public ModuleResponse Get()
        {
            ModuleResponse res = new WebApplication55.ModuleResponse();
            var v = WebCacheManager.Instance.GetValue<Module[]>("mdls");
            var t = WebCacheManager.Instance.GetValue<DateTime>("mdlsTime");
            res.time = t;
            res.mdls = v;
            return res;
        }

        // GET: api/mdls/5

        //1  - locked-mdls
        public Module[] Get(int id)
        {

            var v = WebCacheManager.Instance.GetValue<List<Module>>("lckd-mdls");
            var vv = v.ToArray();
            return vv;
            //dynamic x = new ExpandoObject();
            //List<dynamic> ls = new List<dynamic>();
            //switch (id)
            //{
            //    case 1:
            //        x.name = "jhjkh"; x.id = 13;
            //        ls.Add(x);
            //        x.name = "jhjkh"; x.id = 30;
            //        ls.Add(x);
            //        x.name = "jhjkh"; x.id = 45;
            //        ls.Add(x);
            //        break;
            //}
            //return ls.ToArray();
        }

        // POST: api/mdls
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/mdls/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/mdls/5
        public void Delete(int id)
        {
        }
    }
}
