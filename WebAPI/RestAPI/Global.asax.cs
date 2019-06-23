using System;
using System.Web;
using System.Web.Http;

namespace RestAPI
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        protected void Application_BeginRequest(object sender, EventArgs e) //Not triggered with PUT
        {

        }

        protected void Application_EndRequest(object sender, EventArgs e) //Not triggered with PUT
        {
            //your code
        }
    }
}