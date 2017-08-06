using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApplication55.Controllers
{
    public class newsController : ApiController
    {
        // GET: api/news
        public RollerResponse Get()
        {
            RollerResponse res = new WebApplication55.RollerResponse();
            var v = WebCacheManager.Instance.GetValue<Roller[]>("news");
            var t = WebCacheManager.Instance.GetValue<DateTime>("newsTime");
            res.time = t;
            res.news = v;
            return res;
        }

        // GET: api/news/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/news
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/news/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/news/5
        public void Delete(int id)
        {
        }
    }
}
