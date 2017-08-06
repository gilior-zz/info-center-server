using Danel.Common;
using Danel.Common.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using System.Web;

namespace WebApplication55
{
    public class BL
    {
        bool isDevMode = true;
        public void LoadData()
        {
            this.HandleModules();
            this.HandleEnvs();
            this.HandleLinks();
            this.HandleFaQs();
            this.HandleRoller();
        }
        private void HandleFaQs()
        {
            int intervaMinutes = 30 * 60 * 1000;
            RunFunc((obj) => this.LoadFaQsIntoCache(null), intervaMinutes);
        }
        private void HandleModules()
        {
            int intervaMinutes = 12 * 60 * 60 * 1000;
            RunFunc((obj) => this.LoadModulesIntoCache(null), intervaMinutes);
        }
        private void HandleRoller()
        {
            int intervaMinutes = 12 * 60 * 60 * 1000;
            RunFunc((obj) => this.LoadRollerIntoCache(null), intervaMinutes);
        }
        private void HandleLinks()
        {
            int intervaMinutes = 12 * 60 * 60 * 1000;
            RunFunc((obj) => this.LoadLinksIntoCache(null), intervaMinutes);
        }
        private void HandleEnvs()
        {
            int intervaMinutes = 30 * 60 * 1000;
            RunFunc((obj) => this.LoadEnvsIntoCache(null), intervaMinutes);
        }
        private void RunFunc(Action<object> a, int intervaMinutes)
        {

            Timer t = new Timer();
            t.Enabled = true;
            t.Interval = this.isDevMode ? 12 * 60 * 60 * 1000 : intervaMinutes;
            t.Elapsed += (s, e) => { a.Invoke(null); };
            t.Start();
            a.Invoke(null);
        }
        private void LoadRollerIntoCache(object obj)
        {
            using (var con = new SqlConnection())
            {
                con.ConnectionString = @"Data Source=DANEL-DB\S16;Initial Catalog=support_new;Integrated Security=True;";

                con.Open();
                using (SqlCommand cmd = new SqlCommand("rollerSelect", con, null))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    try
                    {
                        List<Roller> rollers = new List<WebApplication55.Roller>();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Roller roller = new WebApplication55.Roller();
                                roller.id = Convert.ToInt32(reader["rol_id"].ToString());
                                roller.msg = reader["rol_massage"].ToString();
                                roller.time = Convert.ToDateTime(reader["rol_timeStamp"].ToString());
                                rollers.Add(roller);
                            }
                        }
                        WebCacheManager.Instance.SetValue("news", rollers.ToArray());
                        WebCacheManager.Instance.SetValue("newsTime", DateTime.Now);
                    }
                    catch
                    {
                    }
                }
            }
        }
        private void LoadFaQsIntoCache(object obj)
        {
            using (var con = new SqlConnection())
            {
                con.ConnectionString = @"Data Source=DANEL-DB\S16;Initial Catalog=support_new;Integrated Security=True;";

                con.Open();
                using (SqlCommand cmd = new SqlCommand("SupportIssuesSelect", con, null))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    try
                    { 
                        Dictionary<int?, SupportIssue> supportIssues = new Dictionary<int?, SupportIssue>();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                SupportIssue si = new SupportIssue();
                                si.id = reader["ID"].ToString().ToNullable<int>();
                                si.mID = reader["ModuleID"].ToString().ToNullable<int>();
                                si.prb = reader["Problem"].ToString();
                                si.sln = reader["Solution"].ToString();
                                si.mdlName = reader["Text"].ToString();

                                si.ts = Convert.ToDateTime(reader["TimeStamp"].ToString());
                                supportIssues.Add(si.id, si);
                            }
                            var mdls = WebCacheManager.Instance.GetValue<Module[]>("mdls");
                            foreach (var item in supportIssues.Values)
                            {
                                bool skip = false;
                                var mID = item.mID;
                                foreach (var mdl in mdls)
                                {
                                    if (skip) break;
                                    var v = mdl.Descendants().Where(i => i.id == mID);
                                    if (v.Any())
                                    {
                                        item.mod = mdl;
                                        skip = true;
                                    }

                                }

                            }
                            reader.NextResult();
                            List<SupportIssueLink> supportIssueLinks = new List<SupportIssueLink>();
                            while (reader.Read())
                            {
                                SupportIssueLink sil = new WebApplication55.SupportIssueLink();
                                sil.id = Convert.ToInt32(reader["ID"].ToString());
                                sil.pth = reader["Path"].ToString();
                                sil.nm = Path.GetFileName(reader["Path"].ToString());
                                sil.sIid = Convert.ToInt32(reader["SupportIssueID"].ToString());
                                supportIssueLinks.Add(sil);
                            }
                            var grpd = supportIssueLinks.GroupBy(i => i.sIid);
                            foreach (var item in grpd)
                            {
                                SupportIssue si;
                                if (supportIssues.TryGetValue(item.Key, out si))
                                    si.lnks = item.ToArray();
                            }
                        }

                        var toArray = supportIssues.Values.ToArray();
                        WebCacheManager.Instance.SetValue("sis", toArray);
                        WebCacheManager.Instance.SetValue("sisTime", DateTime.Now);
                    }
                    catch (Exception) { }
                }


            }
        }
        private void LoadModulesIntoCache(object obj)
        {
            using (var con = new SqlConnection())
            {

                con.ConnectionString = @"Data Source=DANEL-TS\DEV2008R2;Initial Catalog=DanelX;Integrated Security=True;Trusted_Connection=True";

                con.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = $@"{WebConfigManager.Instance["mdls_proc"]}";
                    cmd.Connection = con;
                    try
                    {
                        List<Module> modules = new List<Module>();
                        List<Module> lckdModules = new List<Module>();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Module m = new Module();
                                m.id = Convert.ToInt32(reader["ChildID"].ToString());
                                m.sepcificLicenseSettings = reader["SepcificLicenseSettings"].ToString();
                                m.pID = reader["ParentID"].ToString().ToNullable<int>();
                                m.name = reader["Text"].ToString();
                                modules.Add(m);
                            }
                        }
                        WebCacheManager.Instance.SetValue("mdls-flat", modules);
                        foreach (var item in modules)
                        {
                            var specificLicenseSettings = this.DecryptSpecificLicenseSettings(item.id, item.sepcificLicenseSettings);
                            if (specificLicenseSettings == SpecificLicenseSettings.NeedSpecialLicense)
                                lckdModules.Add(item);
                        }
                        WebCacheManager.Instance.SetValue("lckd-mdls", lckdModules);
                        var lookup = modules.ToLookup(x => x.pID);
                        var tree = this.Build(null, lookup);
                        WebCacheManager.Instance.SetValue("mdls", tree);

                        WebCacheManager.Instance.SetValue("mdlsTime", DateTime.Now);
                    }
                    catch (Exception) { }
                }
            }
        }

        public SpecificLicenseSettings DecryptSpecificLicenseSettings(int moduleID, string encryptedData)
        {
            SpecificLicenseSettings result = SpecificLicenseSettings.NeedSpecialLicense;
            string key1 = (moduleID ^ 05031976).ToString();
            string resultString = Encryptor.Decrypt(encryptedData, key1, 05031976.ToString());
            result = (SpecificLicenseSettings)Enum.Parse(typeof(SpecificLicenseSettings), resultString);
            return result;
        }
        void PrintTree(Module[] tree)
        {
            foreach (Module item in tree)
            {
                printRoot(item);
            }
        }
        void printRoot(Module node)
        {
            printNode(node, 0);
        }
        void printNode(Module node, int level)
        {
            Debug.Write(node.name);
            foreach (Module child in node.children)
            {
                printNode(child, level + 1); //<-- recursive
            }
        }
        private void LoadLinksIntoCache(object obj)
        {
            List<Link> links = new List<Link>();
            // Get the current configuration file.



            // Get the custom configuration section.
            //LinksSection linksSection = ConfigurationManager.GetSection("links") as LinksSection;
            //foreach (LinkConfigElement item in linksSection.Links)
            //{
            //    Link link = new Link();
            //    link.ctg = item.ctg;
            //    link.nm = item.nm;
            //    link.pth = item.pth;
            //    links.Add(link);
            //}

            using (var con = new SqlConnection())
            {
                con.ConnectionString = @"Data Source=DANEL-DB\S16;Initial Catalog=support_new;Integrated Security=True;";

                con.Open();
                using (SqlCommand cmd = new SqlCommand("SystemLinksSelect", con, null))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Link link = new Link();
                            link.ctg = reader["category"].ToString();
                            link.nm = reader["name"].ToString();
                            link.pth = reader["path"].ToString();
                            links.Add(link);
                        }
                    }
                }
            }
            var grpd = links.GroupBy(i => i.ctg);

            var grpdLinks = grpd.ToArray();
            WebCacheManager.Instance.SetValue("lnks", grpdLinks);
            WebCacheManager.Instance.SetValue("lnksTime", DateTime.Now);
        }
        private void LoadEnvsIntoCache(object obj)
        {
            int num = 1;
            //var files = WebCacheManager.Instance.GetValue<string>("files");
            //var filesDirectory = new DirectoryInfo(files.ToString());
            //foreach (var item in filesDirectory.GetFiles())
            //    item.Delete();
            List<DanelVersion> vers = new List<DanelVersion>();
            var sqls = WebConfigManager.Instance["sql"].ToString();
            var servers = sqls.Split(';');
            foreach (var srv in servers)
            {
                using (var con = new SqlConnection())
                {
                    con.ConnectionString = $"Data Source={srv};Initial Catalog=master;Integrated Security=True;";

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = $@"{WebConfigManager.Instance["env_proc"]}";


                        cmd.Connection = con;
                        try
                        {
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                do
                                {
                                    while (reader.Read())
                                    {
                                        string db_name = reader["DB_NAME"].ToString();
                                        string serverName = reader["ServerName"].ToString();
                                        string notificationPort = reader["notificationPort"].ToString();
                                        string listenerPort = reader["listenerPort"].ToString();
                                        string wcfport = reader["wcfport"].ToString();
                                        string sqlInstance = reader["sqlInstance"].ToString();
                                        string notificationSubscriptionsPort = reader["notificationSubscriptionsPort"].ToString();

                                        string clientLocation = GenerateClientLocation(reader, serverName);

                                        string serverLocation = reader["serverLocation"].ToString();
                                        string winServiceName = serverLocation.Substring(serverLocation.LastIndexOf("\\") + 1);

                                        string notificationLocation = reader["notificationLocation"].ToString();
                                        string winNotificationName = notificationLocation.Substring(notificationLocation.LastIndexOf("\\") + 1);
                                        int[] lckdMdls = null;
                                        var v = reader["lckdMdls"].ToString();
                                        if (!string.IsNullOrEmpty(v))
                                        {
                                            v = v.Remove(v.LastIndexOf(','), 1);
                                            lckdMdls = v.Split(',').Select(i => int.Parse(i)).ToArray();
                                        }

                                        //string newFile = GenerateNewFile(files.ToString(), ref num, clientLocation);
                                        DanelVersion danelVersion = GenarateDanelVersion(
                                            reader,
                                            db_name,
                                            clientLocation,
                                            serverName,
                                            listenerPort,
                                            notificationPort,
                                            wcfport,
                                            notificationSubscriptionsPort,
                                            winServiceName,
                                            winNotificationName,
                                            sqlInstance,
                                             lckdMdls);
                                        danelVersion.id = num++;
                                        vers.Add(danelVersion);
                                    }
                                } while (reader.NextResult());

                            }
                        }
                        catch (Exception) { Debug.WriteLine($"fail {cmd.Connection.ConnectionString}"); }
                    }

                }
            }

            IOrderedEnumerable<DanelVersion> sorted = SortVers(vers);
            var toFaltArray = sorted.ToArray();
            WebCacheManager.Instance.SetValue("flat-vers", toFaltArray);
            WebCacheManager.Instance.SetValue("versTime", DateTime.Now);
            var grpd = sorted.GroupBy(x => new Version(x.vr.Major, x.vr.Minor, x.vr.Build, x.vr.Revision));
            var toArray = grpd.ToArray();
            WebCacheManager.Instance.SetValue("vers", toArray);
            WebCacheManager.Instance.SetValue("versTime", DateTime.Now);
        }
        private Module[] Build(int? pid, ILookup<int?, Module> lookup)
        {
            var module_children = lookup[pid].Select(x => new Module()
            {
                id = x.id,
                name = x.name,
                pID = x.pID,
                children = Build(x.id, lookup),
            });
            var toArray = module_children.ToArray();
            return toArray;
        }
        private static IOrderedEnumerable<DanelVersion> SortVers(List<DanelVersion> vers)
        {
            return vers.OrderByDescending(i => i.vr.Major).
                              ThenByDescending(i => i.vr.Minor).
                              ThenByDescending(i => i.vr.Build).
                              ThenByDescending(i => i.vr.Revision);
        }
        private static string GenerateNewFile(string files, ref int num, string clientLocation)
        {
            var dir = new DirectoryInfo(new DirectoryInfo(clientLocation).Parent.FullName);
            var fileName = $"{num++}.url";
            var newFile = Path.Combine(files, fileName);
            using (FileStream fs = File.Create(newFile))
            {
                string content = $"[InternetShortcut]\n URL = file://{dir.FullName}";
                Byte[] info = new UTF8Encoding(true).GetBytes(content);
                fs.Write(info, 0, info.Length);
            }

            return newFile;
        }
        private static string GenerateClientLocation(SqlDataReader reader, string serverName)
        {
            string clientLocation = reader["clientLocation"].ToString();
            var mainFolder = clientLocation.Substring(0, 2);
            if (mainFolder != @"\\")
                clientLocation = clientLocation.Replace(mainFolder, $@"\\{serverName}");
            return clientLocation;
        }
        private static DanelVersion GenarateDanelVersion(SqlDataReader reader,
            string db_name,
            string clientLocation,
            string serverName,
            string listenerPort,
            string notificationPort,
             string wcfport,
            string notificationSubscriptionsPort,
            string winServiceName,
            string winNotificationName,
            string sqlInstance,
            int[] lckdMdls)
        {
            int majorVersion = Convert.ToInt32(reader["MajorVersion"].ToString());
            int minorVersion = Convert.ToInt32(reader["MinorVersion"].ToString());
            int subVersion = Convert.ToInt32(reader["SubVersion"].ToString());
            int buildNumber = Convert.ToInt32(reader["BuildNumber"].ToString());

            var dir = new DirectoryInfo(clientLocation);
            var par = dir.Parent.FullName;

            var v = Path.Combine(par.ToString(), "AppStart.exe");
            clientLocation = v;


            Version version = new Version(majorVersion, minorVersion, subVersion, buildNumber);
            Version appVersion = version;
            if (File.Exists(v))
            {
                FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(v);
                appVersion = new Version(myFileVersionInfo.FileMajorPart, myFileVersionInfo.FileMinorPart, myFileVersionInfo.FileBuildPart, myFileVersionInfo.FilePrivatePart);
            }

            DanelVersion danelVersion = new DanelVersion();
            danelVersion.vr = version;
            danelVersion.appVr = appVersion;
            danelVersion.dbName = db_name;
            danelVersion.fp = clientLocation;
            danelVersion.listenerPort = listenerPort;
            danelVersion.notificationPort = notificationPort;
            danelVersion.wcfport = wcfport;
            danelVersion.notificationSubscriptionsPort = notificationSubscriptionsPort;
            danelVersion.clientFolder = par;
            danelVersion.winServiceName = winServiceName;
            danelVersion.serverName = serverName;
            danelVersion.winNotificationName = winNotificationName;
            danelVersion.sqlInstance = sqlInstance;
            danelVersion.lckdMdls = lckdMdls;
            danelVersion.version = $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
            danelVersion.appVersion = $"{appVersion.Major}.{appVersion.Minor}.{appVersion.Build}.{appVersion.Revision}";
            danelVersion.notificationPorts = $"{notificationPort};{notificationSubscriptionsPort}";
            danelVersion.listenerPorts = $"{listenerPort};{wcfport}";

            //DanelVersion danelVersion = new DanelVersion(
            //     version,
            //     appVersion,
            //     db_name,
            //     clientLocation,
            //     serverName,
            //     listenerPort,
            //     notificationPort,
            //     wcfport,
            //     notificationSubscriptionsPort,
            //     par,
            //     winServiceName,
            //     winNotificationName,
            //     sqlInstance,
            //      lckdMdls);
            return danelVersion;
        }
        public void UpdateFaq(int id, SupportIssue faq)
        {
            using (var con = new SqlConnection())
            {
                con.ConnectionString = @"Data Source=DANEL-DB\S16;Initial Catalog=support_new;Integrated Security=True;";
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SupportIssuesUpdate", con, null))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Problem", faq.prb);
                    cmd.Parameters.AddWithValue("@Solution", faq.sln);
                    cmd.Parameters.AddWithValue("@ModuleID", faq.mID);
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.ExecuteNonQuery();
                }
                using (SqlCommand cmd = new SqlCommand("SupportIssueLinksUpdate", con, null))
                {
                    foreach (var lnk in faq.lnks)
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SupportIssueID", id);
                        cmd.Parameters.AddWithValue("@Path", lnk.pth);
                        cmd.Parameters.AddWithValue("@ID", lnk.id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        public void AddFaQ(SupportIssue faq)
        {
            using (var con = new SqlConnection())
            {
                con.ConnectionString = @"Data Source=DANEL-DB\S16;Initial Catalog=support_new;Integrated Security=True;";
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SupportIssuesUpdate", con, null))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Problem", faq.prb);
                    cmd.Parameters.AddWithValue("@Solution", faq.sln);
                    cmd.Parameters.AddWithValue("@ModuleID", faq.mID);
                    cmd.Parameters.AddWithValue("@ID", null);
                    cmd.ExecuteNonQuery();
                }
                using (SqlCommand cmd = new SqlCommand("SupportIssueLinksUpdate", con, null))
                {
                    foreach (var lnk in faq.lnks)
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SupportIssueID", lnk.sIid);
                        cmd.Parameters.AddWithValue("@Path", lnk.pth);
                        cmd.Parameters.AddWithValue("@ID", lnk.id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        public void RemoveFaQ(int id)
        {
            using (var con = new SqlConnection())
            {
                con.ConnectionString = @"Data Source=DANEL-DB\S16;Initial Catalog=support_new;Integrated Security=True;";
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SupportIssuesDelete", con, null))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.ExecuteNonQuery();
                }
                using (SqlCommand cmd = new SqlCommand("SupportIssueLinksDelete", con, null))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.ExecuteNonQuery();

                }
            }
        }
    }






}