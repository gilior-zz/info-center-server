using System;
using System.Configuration;
using System.Web.Configuration;

namespace WebApplication55
{
    public class WebConfigManager
    {
        private static WebConfigManager _webConfigManager = null;
        
        public static WebConfigManager Instance
        {
            get
            {
                if (_webConfigManager == null)
                {
                    _webConfigManager = new WebConfigManager();
                }

                return _webConfigManager;
            }
        }

        public string this[String key]
        {
            get
            {
                var value = WebConfigurationManager.AppSettings[key];

                if (value != null)
                    return value;

                return null;
            }

            set { Write2WebConfigAppSettings(key, value); }
        }

        
        private static bool Write2WebConfigAppSettings(string key, string value)
        {
            try
            {
                var configuration = WebConfigurationManager.OpenWebConfiguration("~");
                var section = (AppSettingsSection)configuration.GetSection("appSettings");
                section.Settings[key].Value = value;
                configuration.Save();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }


    }
}