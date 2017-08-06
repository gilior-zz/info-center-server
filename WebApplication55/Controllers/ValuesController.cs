using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace WebApplication55.Controllers
{

    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ValuesController : ApiController
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
        public DanelVersionResponse GetVers()
        {
            DanelVersionResponse res = new WebApplication55.DanelVersionResponse();
            var v = WebCacheManager.Instance.GetValue<IGrouping<Version,DanelVersion>[]> ("vers");
            var t = WebCacheManager.Instance.GetValue<DateTime>("versTime");
            res.time = t;
            res.vers = v;
            return res;
        }
        public LinkResponse GetLnks()
        {
            

            LinkResponse res = new WebApplication55.LinkResponse();
            var v = WebCacheManager.Instance.GetValue<IGrouping<string,Link>[]>("lnks");
            var t = WebCacheManager.Instance.GetValue<DateTime>("lnksTime");
            res.time = t;
            res.lnks = v;
            return res;
        }
        //private T BuildResponse<T, U>(string item, U u) where T : new(), 
        //{
        //    var v = WebCacheManager.Instance.GetValue(item, u);
        //    var t = WebCacheManager.Instance.GetValue($"{item}Time", )DateTime.Now);
        //    var r = new T();
        //    return r;
        //}
        public ModuleResponse GetMdls()
        {
          

            ModuleResponse res = new WebApplication55.ModuleResponse();
            var v = WebCacheManager.Instance.GetValue<Module[]>("mdls");
            var t = WebCacheManager.Instance.GetValue<DateTime>("mdlsTime");
            res.time = t;
            res.mdls = v;
            return res;
        }
        public SupportIssueResponse GetSIS()
        {          
            SupportIssueResponse res = new WebApplication55.SupportIssueResponse();
            var v = WebCacheManager.Instance.GetValue<SupportIssue[]>("sis");
            var t = WebCacheManager.Instance.GetValue<DateTime>("sisTime");
            res.time = t;
            res.sis = v;
            return res;
        }
        public RollerResponse GetRlr()
        {           

            RollerResponse res = new WebApplication55.RollerResponse();
            var v = WebCacheManager.Instance.GetValue<Roller[]>("rlrs");
            var t = WebCacheManager.Instance.GetValue<DateTime>("rlrsTime");
            res.time = t;
            res.news = v;
            return res;
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]SupportIssue value)
        {
            this.bl.AddFaQ(value);
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]SupportIssue value)
        {
            this.bl.UpdateFaq(id, value);
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
            this.bl.RemoveFaQ(id);
        }
    }
}
