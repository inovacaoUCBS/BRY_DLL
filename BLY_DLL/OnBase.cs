using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hyland.Unity;
using Hyland.Types;
using NLog;

namespace BLY_DLL
{
    internal class OnBase: IDisposable
    {
        private static Logger nLog = LogManager.GetCurrentClassLogger();
        public  Application app;
        public Application GetApplication()
        {
            AuthenticationProperties authProperty = Application.CreateOnBaseAuthenticationProperties(AppSettings.AppServer, AppSettings.Username, AppSettings.Password, AppSettings.Odbc);
            try
            {
                app = Application.Connect(authProperty);
                nLog.Debug($"Connectado no Onbase: {AppSettings.AppServer} com usuario {AppSettings.Username} e session Id {app.SessionID}");
                return app;
            }
            catch(Exception ex)
            {
                nLog.Error(ex);
                return null;
            }
        }
        public Document GetDocumentByiD(long documentHandle)
        {
            Document doc = app.Core.GetDocumentByID(documentHandle);
            return doc;
        }

        public void Disconnect()
        {
            if(app != null && app.IsConnected)
            {
                app.Disconnect();
                app.Dispose();
            }
        }

        public void Dispose()
        {
            if (app != null && app.IsConnected)
            {
                app.Disconnect();
                app.Dispose();
                app = null;
            }
        }
    }
}
