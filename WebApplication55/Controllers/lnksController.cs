using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApplication55.Controllers
{
    public class lnksController : ApiController
    {
        // GET: api/lnks    
        public LinkResponse Get()
        {
            LinkResponse res = new WebApplication55.LinkResponse();
            var v = WebCacheManager.Instance.GetValue<IGrouping<string, Link>[]>("lnks");
            var t = WebCacheManager.Instance.GetValue<DateTime>("lnksTime");
            res.time = t;
            res.lnks = v;
            return res;
        }

        // GET: api/lnks/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/lnks
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/lnks/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/lnks/5
        public void Delete(int id)
        {
        }
    }
}
