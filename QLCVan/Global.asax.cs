using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.IO;


namespace QLCVan
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            StreamReader sr = new StreamReader(Server.MapPath(@"Counter.txt"));
            string count = sr.ReadLine();
            sr.Close();
            sr.Dispose();

            Application["HitOnline"] = 0;
            Application["HitCount"] = count;
        }

        protected void Session_Start(object sender, EventArgs e)
        {

            Session["User"] = "";
            Application.Lock();

            int hitOnline = 0;
            int hitCount = 0;

            // Bảo vệ trường hợp null hoặc không đúng định dạng
            if (Application["HitOnline"] != null)
                int.TryParse(Application["HitOnline"].ToString(), out hitOnline);

            if (Application["HitCount"] != null)
                int.TryParse(Application["HitCount"].ToString(), out hitCount);

            Application["HitOnline"] = hitOnline + 1;
            Application["HitCount"] = hitCount + 1;

            Application.UnLock();

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {
            Application["HitOnline"] = int.Parse(Application["HitOnline"].ToString()) - 1;
        }

        protected void Application_End(object sender, EventArgs e)
        {

        }

    }
}
