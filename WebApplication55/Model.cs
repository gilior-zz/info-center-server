using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Web;

namespace WebApplication55
{

    public class LinksSection : ConfigurationSection
    {


        [ConfigurationProperty("link", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(LinkCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public LinkCollection Links
        {
            get
            {
                LinkCollection linksCollection =
                    (LinkCollection)base["link"];

                return linksCollection;
            }

            set
            {
                LinkCollection linkCollection = value;
            }

        }


        public LinksSection()
        {
            LinkConfigElement link = new LinkConfigElement();
            Links.Add(link);

        }

    }
    public class LinkCollection : ConfigurationElementCollection
    {


        public LinkCollection()
        {

        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new LinkConfigElement();
        }

        protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((LinkConfigElement)element).nm;
        }

        public LinkConfigElement this[int index]
        {
            get
            {
                return (LinkConfigElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        new public LinkConfigElement this[string Name]
        {
            get
            {
                return (LinkConfigElement)BaseGet(Name);
            }
        }


        public int IndexOf(LinkConfigElement link)
        {
            return BaseIndexOf(link);
        }

        public void Add(LinkConfigElement link)
        {
            BaseAdd(link);

            // Your custom code goes here.

        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);

            // Your custom code goes here.

        }

        public void Remove(LinkConfigElement link)
        {
            if (BaseIndexOf(link) >= 0)
            {
                BaseRemove(link.nm);
                // Your custom code goes here.
                Console.WriteLine("UrlsCollection: {0}", "Removed collection element!");
            }
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);

            // Your custom code goes here.

        }

        public void Remove(string name)
        {
            BaseRemove(name);

            // Your custom code goes here.

        }

        public void Clear()
        {
            BaseClear();

            // Your custom code goes here.
            Console.WriteLine("UrlsCollection: {0}", "Removed entire collection!");
        }

    }
    public class LinkConfigElement : ConfigurationElement
    {
        public LinkConfigElement(String name, String path, string category)
        {
            this.nm = name;
            this.pth = path;
            this.ctg = category;
        }

        public LinkConfigElement()
        {

        }

        [ConfigurationProperty("name", IsRequired = true)]
        public string nm
        {
            get
            {
                return (string)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }

        [ConfigurationProperty("path", IsRequired = true, IsKey = true)]
        public string pth
        {
            get
            {
                return (string)this["path"];
            }
            set
            {
                this["path"] = value;
            }
        }

        [ConfigurationProperty("category", IsRequired = true)]
        public string ctg
        {
            get
            {
                return (string)this["category"];
            }
            set
            {
                this["category"] = value;
            }
        }

    }
    public class DanelVersion
    {
        public Version vr { get; set; }
        public Version appVr { get; set; }
        public string dbName { get; set; }
        public string fp { get; set; }
        public string serverName { get; set; }
        public string listenerPort { get; set; }
        public string notificationPort { get; set; }

        public string wcfport { get; set; }
        public string notificationSubscriptionsPort { get; set; }
        public string clientFolder { get; set; }
        public string winServiceName { get; set; }
        public string winNotificationName { get; set; }
        public bool winServiceIsUp { get; set; }
        public bool winNotificationIsUp { get; set; }
        public int id { get; set; }
        public string version { get; set; }
        public string appVersion { get; set; }
        public string listenerPorts { get; set; }
        public string notificationPorts { get; set; }
        public string sqlInstance { get; set; }
        public int[] lckdMdls { get; set; }
        public int winNotificationStatus { get; internal set; }
        public int winListenerStatus { get; internal set; }




        //public DanelVersion(
        //    Version version,
        //    Version appVersion,
        //    string dbName,
        //    string filePath,
        //    string serverName,
        //    string listenerPort,
        //    string notificationPort,
        //      string wcfport,
        //    string notificationSubscriptionsPort,
        //    string clientFolder,
        //    string winServiceName,
        //    string winNotificationName,
        //    string sqlInstance,
        //    int[] lckdMdls)
        //{
        //    this.serverName = serverName;
        //    this.vr = version;
        //    this.appVr = appVersion;
        //    this.dbName = dbName;
        //    this.fp = filePath;
        //    this.listenerPort = listenerPort;
        //    this.notificationPort = notificationPort;
        //    this.wcfport = wcfport;
        //    this.notificationSubscriptionsPort = notificationSubscriptionsPort;
        //    this.clientFolder = clientFolder;
        //    this.winServiceName = winServiceName;

        //    this.winNotificationName = winNotificationName;
        //    this.version = $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        //    this.appVersion = $"{appVersion.Major}.{appVersion.Minor}.{appVersion.Build}.{appVersion.Revision}";
        //    this.sqlInstance = sqlInstance;
        //    this.listenerPorts = $"{listenerPort};{wcfport}";
        //    this.notificationPorts = $"{notificationPort};{notificationSubscriptionsPort}";
        //    this.lckdMdls = lckdMdls;
        //}
    }

    public enum CustomServiceControllerStatus
    {
        NotExists = -1,
        ContinuePending = 5,
        Paused = 7,
        PausePending = 6,
        Running = 4,
        StartPending = 2,
        Stopped = 1,
        StopPending = 3,
    }


    public class DanelVersionResponse
    {
        public IGrouping<Version, DanelVersion>[] vers { get; set; }
        public DanelVersion[] flatVers { get; set; }
        public DateTime time { get; set; }
        public DanelVersion ver { get; internal set; }
    }
    public class Link
    {
        public string ctg { get; set; }
        public string nm { get; set; }
        public string pth { get; set; }
    }
    public class LinkResponse
    {
        public IGrouping<string, Link>[] lnks { get; set; }
        public DateTime time { get; set; }
    }
    public class SupportIssue
    {
        public int? id { get; set; }
        public string prb { get; set; }
        public string sln { get; set; }
        public string mdlName { get; set; }
        public int? mID { get; set; }
        public Module mod { get; set; }
        public DateTime ts { get; set; }
        public SupportIssueLink[] lnks { get; set; }
        public Object[] files { get; set; }
        //public Object[] files { get; set; }

    }
    public class SupportIssueLink
    {
        public int id { get; set; }
        public int sIid { get; set; }
        public string pth { get; set; }
        public string nm { get; set; }
      


    }
    public class SupportIssueResponse
    {
        public SupportIssue[] sis { get; set; }
        public DateTime time { get; set; }
    }
    public class Module
    {
        public int id { get; set; }
        public int? pID { get; set; }
        public Module[] children { get; set; }
        public string name { get; set; }
        public string sepcificLicenseSettings { get; internal set; }
    }

    public class ModuleResponse
    {
        public Module[] mdls { get; set; }
        public DateTime time { get; set; }
    }

    public class Roller
    {
        public int id { get; set; }
        public string msg { get; set; }
        public DateTime time { get; set; }
    }

    public class RollerResponse
    {
        public Roller[] news { get; set; }
        public DateTime time { get; set; }
    }
    public static class Helper
    {
        public static Nullable<T> ToNullable<T>(this string str) where T : struct
        {
            if (string.IsNullOrEmpty(str)) return null;

            else return Convert.ChangeType(str, typeof(T)) as T?;
        }

        public static IEnumerable<Module> Descendants(this Module root)
        {
            var nodes = new Stack<Module>(new[] { root });
            while (nodes.Any())
            {
                Module node = nodes.Pop();
                yield return node;
                foreach (var n in node.children) nodes.Push(n);
            }
        }

        public static void WaitForStatusSpan(this ServiceController serviceController, ServiceControllerStatus serviceControllerStatus)
        {
            serviceController.WaitForStatus(serviceControllerStatus, TimeSpan.FromMinutes(30));
        }

    }
}