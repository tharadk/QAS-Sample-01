using System;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.SessionState;
using System.Net;
using com.qas.proweb;							// QuickAddress services

namespace com.qas.prowebintegration 
{
	/// <summary>
	/// Summary description for Global.
	/// </summary>
	public class Global : System.Web.HttpApplication
	{
		public Global()
		{
			InitializeComponent();
		}	
		
		protected void Application_Start(Object sender, EventArgs e)
		{

		}
 
		protected void Session_Start(Object sender, EventArgs e)
		{

		}

		protected void Application_BeginRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_EndRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_AuthenticateRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_Error(Object sender, EventArgs e)
		{

		}

		protected void Session_End(Object sender, EventArgs e)
		{

		}

		protected void Application_End(Object sender, EventArgs e)
		{

		}

        /// Create a new QuickAddress service, connected to the configured server
        public static QuickAddress NewQuickAddress()
        {
            // Retrieve server URL from web.config
            string sServerURL = System.Configuration.ConfigurationManager.AppSettings[Constants.KEY_SERVER_URL];

            // Retrieve Username from web.config
            string sUsername = System.Configuration.ConfigurationManager.AppSettings[Constants.KEY_USERNAME];

            // Retrieve Password from web.config
            string sPassword = System.Configuration.ConfigurationManager.AppSettings[Constants.KEY_PASSWORD];

            // Retrieve proxy address Value from web.config
            string sProxyAddress = System.Configuration.ConfigurationSettings.AppSettings[Constants.KEY_PROXY_ADDRESS];

            // Retrieve proxy username Value from web.config
            string sProxyUsername = System.Configuration.ConfigurationSettings.AppSettings[Constants.KEY_PROXY_USERNAME];

            // Retrieve proxy password Value from web.config
            string sProxyPassword = System.Configuration.ConfigurationSettings.AppSettings[Constants.KEY_PROXY_PASSWORD];


            // Create QuickAddress search object
            if (String.IsNullOrEmpty(sProxyAddress) != true)
            {
                IWebProxy proxy = new WebProxy(sProxyAddress, true);

                NetworkCredential credentials = new NetworkCredential(sProxyUsername, sProxyPassword);
                proxy.Credentials = credentials;

                // Create QuickAddress search object with proxy server
                return new QuickAddress(sServerURL, sUsername, sPassword, proxy);
            }

            return new QuickAddress(sServerURL, sUsername, sPassword);
        }
			
		#region Web Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
		}
		#endregion
	}
}

