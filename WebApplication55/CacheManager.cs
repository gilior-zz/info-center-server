using System;
using System.ComponentModel;
using System.Web;

namespace WebApplication55
{
    public class WebCacheManager 
    {
        // Singleton Functions
        private WebCacheManager() { }

        private static WebCacheManager _cacheManager = null;
        public static WebCacheManager Instance
        {
            get { return _cacheManager ?? (_cacheManager = new WebCacheManager()); }
        }

        public T GetValue<T>(string name )
        {
            if (HttpRuntime.Cache[name] == null)
                return default(T);
            var retVal = HttpRuntime.Cache[name];
            return (T)retVal;
        }



        public void SetValue<T>(string key, T val)
        {
            HttpRuntime.Cache[key] = val;
        }


        public object this[String Name]
        {
            get
            {
                return HttpRuntime.Cache[Name] ?? null;
            }

            set
            {
                HttpRuntime.Cache[Name] = value;
            }
        }
    }
}
