using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApplication55.Controllers
{
    public class faqController : ApiController
    {



        // GET: api/faq
        public SupportIssueResponse Get()
        {
            SupportIssueResponse res = new WebApplication55.SupportIssueResponse();
            var v = WebCacheManager.Instance.GetValue<SupportIssue[]>("sis");
            var sorted = v.OrderByDescending(i => i.ts).ToArray();
            var t = WebCacheManager.Instance.GetValue<DateTime>("sisTime");
            res.time = t;
            res.sis = sorted;
            return res;
        }

        // GET: api/faq/5
        public SupportIssueResponse Get(int id)
        {
            SupportIssueResponse res = new WebApplication55.SupportIssueResponse();
            var v = WebCacheManager.Instance.GetValue<SupportIssue[]>("sis").Where(i => i.id == id);
            var t = WebCacheManager.Instance.GetValue<DateTime>("sisTime");
            res.time = t;

            res.sis = v.ToArray();
            return res;
        }

        // POST: api/faq
        public IHttpActionResult Post([FromBody]SupportIssue value)
        {

            if (value.lnks != null && value.lnks.Any())
                foreach (var item in value.lnks)
                {
                    if (!File.Exists(item.pth))
                        return NotFound();
                }
            using (var con = new SqlConnection())
            {
                con.ConnectionString = @"Data Source=DANEL-DB\S16;Initial Catalog=support_new;Integrated Security=True;";

                con.Open();
                using (SqlCommand SupportIssuesUpdate = new SqlCommand("SupportIssuesUpdate", con, null))
                {
                    SupportIssuesUpdate.CommandType = System.Data.CommandType.StoredProcedure;
                    SupportIssuesUpdate.Parameters.AddWithValue("@Problem", value.prb);
                    SupportIssuesUpdate.Parameters.AddWithValue("@Solution", value.sln);
                    SupportIssuesUpdate.Parameters.AddWithValue("@ModuleID", value.mID);
                    SupportIssuesUpdate.Parameters.AddWithValue("@ID", null);
                    var prm = SupportIssuesUpdate.Parameters.Add("@newID", SqlDbType.Int);
                    prm.Direction = ParameterDirection.Output;
                    SupportIssuesUpdate.ExecuteScalar();
                    int newSupportIssueID = int.Parse(prm.Value.ToString());
                    value.id = newSupportIssueID;

                    if (value.mID.HasValue)
                    {
                        var mdls = WebCacheManager.Instance.GetValue<Module[]>("mdls");
                        var nme = mdls.FirstOrDefault(i => i.id == value.mID).name;
                        value.mdlName = nme;
                    }
                    value.ts = DateTime.Now;
                    if (value.lnks != null)
                    {
                        foreach (var item in value.lnks)
                        {

                            using (SqlCommand supportIssueLinksUpdate = new SqlCommand("SupportIssueLinksUpdate", con, null))
                            {
                                supportIssueLinksUpdate.CommandType = System.Data.CommandType.StoredProcedure;
                                supportIssueLinksUpdate.Parameters.AddWithValue("@SupportIssueID", newSupportIssueID);
                                supportIssueLinksUpdate.Parameters.AddWithValue("@Path", item.pth);
                                supportIssueLinksUpdate.Parameters.AddWithValue("@ID", -1);
                                prm = supportIssueLinksUpdate.Parameters.Add("@newID", SqlDbType.Int);
                                prm.Direction = ParameterDirection.Output;
                                supportIssueLinksUpdate.ExecuteScalar();
                                int newSupportIssueLinkID = int.Parse(prm.Value.ToString());
                                item.id = newSupportIssueLinkID;
                                item.sIid = newSupportIssueID;
                            }
                        }
                    }

                    var sis = WebCacheManager.Instance.GetValue<SupportIssue[]>("sis").ToList();
                    sis.Add(value);
                    var toArray = sis.ToArray();
                    WebCacheManager.Instance.SetValue("sis", toArray);
                    WebCacheManager.Instance.SetValue("sisTime", DateTime.Now);
                }
            }
            return Ok(value);
        }

        // PUT: api/faq/5
        public IHttpActionResult Put(int id, [FromBody]SupportIssue value)
        {
            using (var con = new SqlConnection())
            {
                con.ConnectionString = @"Data Source=DANEL-DB\S16;Initial Catalog=support_new;Integrated Security=True;";

                con.Open();
                using (SqlCommand SupportIssuesUpdate = new SqlCommand("SupportIssuesUpdate", con, null))
                {
                    SupportIssuesUpdate.CommandType = System.Data.CommandType.StoredProcedure;
                    SupportIssuesUpdate.Parameters.AddWithValue("@Problem", value.prb);
                    SupportIssuesUpdate.Parameters.AddWithValue("@Solution", value.sln);
                    SupportIssuesUpdate.Parameters.AddWithValue("@ModuleID", value.mID);
                    SupportIssuesUpdate.Parameters.AddWithValue("@ID", id);
                    SupportIssuesUpdate.ExecuteNonQuery();

                }

                bool supportIssueLinksDeletionSucess = false;
                using (SqlCommand supportIssueLinksDelete = new SqlCommand("SupportIssueLinksDelete", con, null))
                {
                    supportIssueLinksDelete.CommandType = System.Data.CommandType.StoredProcedure;
                    try
                    {
                        var hasLnks = value.lnks.Any();
                        if (hasLnks)
                        {
                            var newLnks = value.lnks;
                            foreach (var item in newLnks)
                            {
                                supportIssueLinksDelete.Parameters.Clear();
                                supportIssueLinksDelete.Parameters.AddWithValue("@ID", value.id);
                                supportIssueLinksDelete.ExecuteNonQuery();
                            }
                        }
                        supportIssueLinksDeletionSucess = true;

                    }
                    catch (Exception)
                    {


                    }

                }

                if (supportIssueLinksDeletionSucess)
                {
                    using (SqlCommand supportIssueLinksUpdate = new SqlCommand("SupportIssueLinksUpdate", con, null))
                    {
                        supportIssueLinksUpdate.CommandType = System.Data.CommandType.StoredProcedure;
                        var hasLnks = value.lnks.Any();
                        if (hasLnks)
                        {
                            var newLnks = value.lnks;
                            foreach (var item in newLnks)
                            {
                                supportIssueLinksUpdate.Parameters.Clear();
                                supportIssueLinksUpdate.Parameters.AddWithValue("@ID", default(int));
                                supportIssueLinksUpdate.Parameters.AddWithValue("@SupportIssueID", value.id);
                                supportIssueLinksUpdate.Parameters.AddWithValue("@Path", item.pth);
                                supportIssueLinksUpdate.ExecuteNonQuery();
                            }
                        }

                    }
                }

                var arr = WebCacheManager.Instance.GetValue<SupportIssue[]>("sis");
                for (int i = 0; i < arr.Length; i++)
                {
                    if (arr[i].id == id)
                    {
                        arr[i] = value;
                        arr[i].ts = DateTime.Now;
                    }
                }


                if (value.mID.HasValue)
                {
                    var mdls = WebCacheManager.Instance.GetValue<List<Module>>("mdls-flat");
                    var v = mdls.FirstOrDefault(j => j.id == value.mID);
                    if (v != null)
                        value.mdlName = v.name;
                }
                value.ts = DateTime.Now;
                WebCacheManager.Instance.SetValue("sisTime", DateTime.Now);
            }
            return Ok(value);
        }

        // DELETE: api/faq/5
        public IHttpActionResult Delete(int id)
        {
            using (var con = new SqlConnection())
            {
                con.ConnectionString = @"Data Source=DANEL-DB\S16;Initial Catalog=support_new;Integrated Security=True;";

                con.Open();
                using (SqlCommand supportIssuesDelete = new SqlCommand("SupportIssuesDelete", con, null))
                {
                    supportIssuesDelete.CommandType = CommandType.StoredProcedure;
                    supportIssuesDelete.Parameters.AddWithValue("@ID", id);
                    supportIssuesDelete.ExecuteNonQuery();
                }
            }
            var sis = WebCacheManager.Instance.GetValue<SupportIssue[]>("sis").ToList();
            var v = sis.RemoveAll(i => i.id == id);
            var toArray = sis.ToArray();
            WebCacheManager.Instance.SetValue("sis", toArray);

            WebCacheManager.Instance.SetValue("sisTime", DateTime.Now);
            return Ok();
        }
    }
}
