using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Claysys.MailWebService
{
    public class CoreConnectionStringManager
    {
        static string _cRMConnectionString;
        public static string CRMConnectionString;



        static CoreConnectionStringManager()
        {
            try
            {
                var path = "/";
                System.Configuration.Configuration config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(path);

                ConnectionStringsSection csSection = config.ConnectionStrings;
                AppSettingsSection appSection = config.AppSettings;

                if (csSection.ConnectionStrings["CRMConnectionString"] != null)
                    CRMConnectionString = csSection.ConnectionStrings["CRMConnectionString"].ConnectionString;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}