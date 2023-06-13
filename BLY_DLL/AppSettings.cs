using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLY_DLL
{
    internal static class AppSettings
    {
        public static string AppServer { get;set; }
        public static string Odbc { get; set; }
        public static string Username { get; set; }
        public static string Password { get; set; }

        static AppSettings()
        {
            AppServer = System.Configuration.ConfigurationManager.AppSettings["AppServer"];
            Odbc = System.Configuration.ConfigurationManager.AppSettings["Odbc"];
            Username = System.Configuration.ConfigurationManager.AppSettings["Username"];
            Password = System.Configuration.ConfigurationManager.AppSettings["Password"];
            //AppServer = "http://ecmhml.unimedcbs.com.br/appserver/Service.asmx";
            //Odbc = "Onbase";
            //Username = "manager";
            //Password = "UnimedCBS@2022";
        }
    }
}
