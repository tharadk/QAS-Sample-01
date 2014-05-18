/// QAS Pro On Demand > (c) QAS Ltd > www.qas.com
/// Web > Verification > VerifyBase
/// Provide common functionality and value transfer


using System;
using System.Web;
using System.Net;                       // Proxy class
using com.qas.proweb;					// QuickAddress services


namespace com.qas.prowebintegration
{
    /// <summary>
    /// Web > Verification > VerifyBase
    /// This is the base class for the pages of the scenario
    /// It provides common functionality and simple inter-page value passing
    /// </summary>
    public class VerifyBase : System.Web.UI.Page
    {
        /** Attributes & Constants **/


        private QuickAddress m_searchService = null;
        private string[] m_asInputAddress;
        // Page filenames
        private const string PAGE_REFINE = "VerifyRefine.aspx";


        /** Methods **/


        /// Create a new QuickAddress service, connected to the configured server
        protected QuickAddress theQuickAddress
        {
            get
            {
                if (m_searchService == null)
                {
                    // Retrieve server URL from web.config
                    string sServerURL = System.Configuration.ConfigurationSettings.AppSettings[Constants.KEY_SERVER_URL];

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
                        m_searchService = new QuickAddress(sServerURL, sUsername, sPassword, proxy);
                    }
                    else
                    {
                        m_searchService = new QuickAddress(sServerURL, sUsername, sPassword);
                    }
                }
                return m_searchService;
            }
        }

        /// Get the layout from the config
        protected string GetLayout()
        {
            string sLayout = "";
            string sDataID = Request[Constants.FIELD_DATA_ID];

            if (sDataID != null && sDataID != "")
            {
                // Look for a layout specific to this datamap 
                sLayout = System.Configuration.ConfigurationManager.AppSettings[Constants.KEY_LAYOUT + "." + sDataID];

                if (sLayout == null || sLayout == "")
                {
                    // No layout found specific to this datamap - try the default
                    sLayout = System.Configuration.ConfigurationManager.AppSettings[Constants.KEY_LAYOUT];
                }
            }
            return sLayout;
        }

        /// Transfer out of the scenario to display the formatted address
        protected void GoFinalPage(string sMoniker)
        {
            FormatAddress(sMoniker);
            Server.Transfer(Constants.PAGE_FINAL_ADDRESS);
        }

        /// Transfer out of the scenario to display the found address
        protected void GoFinalPage()
        {
            Server.Transfer(Constants.PAGE_FINAL_ADDRESS);
        }

        /// Transfer to the address refinement page, when additional information is required
        protected void GoRefinePage()
        {
            Server.Transfer(PAGE_REFINE);
        }

        /// Transfer out of the scenario to display the input address, after exception thrown
        protected void GoErrorPage(Exception x)
        {
            // Copy input lines through to output
            SetAddressResult(GetInputAddress);
            SetAddressInfo("address verification " + Constants.Routes.Failed + ", so the entered address has been used");
            SetErrorInfo(x.Message);
            Server.Transfer(Constants.PAGE_FINAL_ADDRESS);
        }

        /// Transfer out of the scenario to display the input address, after verification failed
        protected void GoErrorPage(Constants.Routes route, string sReason)
        {
            // Copy input lines through to output
            SetAddressResult(GetInputAddress);
            SetAddressInfo("address verification " + route + ", so the entered address has been used");
            SetErrorInfo(sReason);
            Server.Transfer(Constants.PAGE_FINAL_ADDRESS);
        }

        /// <summary>
        /// Retrieve a final formatted address from the moniker, which came from the picklist
        /// </summary>
        /// <param name="sMoniker">Search Point Moniker of address to retrieve</param>
        protected void FormatAddress(string sMoniker)
        {
            try
            {
                // Format the address
                FormattedAddress tAddressResult = theQuickAddress.GetFormattedAddress(sMoniker, GetLayout());
                SetDPVInfo((Constants.DPVStatus)Enum.Parse(typeof(Constants.DPVStatus), tAddressResult.DPVStatus.ToString()));   
                SetAddressResult(tAddressResult);
            }
            catch (Exception x)
            {
                SetAddressResult(GetInputAddress);
                SetAddressInfo("address verification is not available, so the entered address has been used");
                SetErrorInfo(x.Message);
            }
        }


        /** Stored properties **/


        /// Write out the address result - into the Request as cookies (server side only)
        protected void SetAddressResult(FormattedAddress tAddressResult)
        {
            Request.Cookies.Remove(Constants.FIELD_ADDRESS_LINES);
            foreach (AddressLine tLine in tAddressResult.AddressLines)
            {
                Request.Cookies.Add(new HttpCookie(Constants.FIELD_ADDRESS_LINES, tLine.Line));
            }
            AddAddressWarnings(tAddressResult);
        }

        /// Write out the address result - into the Request as cookies (server side only)
        protected void SetAddressResult(string[] asAddress)
        {
            Request.Cookies.Remove(Constants.FIELD_ADDRESS_LINES);
            foreach (string sLine in asAddress)
            {
                Request.Cookies.Add(new HttpCookie(Constants.FIELD_ADDRESS_LINES, sLine));
            }
        }

        /// Country display name (i.e. Australia)
        protected string GetCountry()
        {
            return Request[Constants.FIELD_COUNTRY_NAME];
        }

        /// Country display name (i.e. Australia)
        protected void SetCountry(string sCountry)
        {
            Request.Cookies.Set(new HttpCookie(Constants.FIELD_COUNTRY_NAME, sCountry));
        }

        /// Error information returned through the exception
        protected void SetErrorInfo(string sErrorInfo)
        {
            Request.Cookies.Set(new HttpCookie(Constants.FIELD_ERROR_INFO, sErrorInfo));
        }

        /// Entered address to check
        protected string[] GetInputAddress
        {
            get
            {
                if (m_asInputAddress == null)
                {
                    m_asInputAddress = Request.Form.GetValues(Constants.FIELD_INPUT_LINES);
                }
                return m_asInputAddress;
            }
        }

        /// Moniker of the final address
        protected string GetMoniker()
        {
            return Request[Constants.FIELD_MONIKER];
        }

        /// Get the state of the verification searching
        protected string GetAddressInfo()
        {
            return Request[Constants.FIELD_ADDRESS_INFO];
        }

        /// Get the address information, HTML transformed
        protected string GetAddressInfoHTML()
        {
            return GetAddressInfo().Replace("\n", "<br />");
        }

        /// Set the state of the verification searching
        protected void SetAddressInfo(string sAddressInfo)
        {
            Request.Cookies.Set(new HttpCookie(Constants.FIELD_ADDRESS_INFO, sAddressInfo));
        }

        protected void SetDPVInfo(Constants.DPVStatus status)
        {
            string sDPVStatus = null;
            switch (status)
            {
                case Constants.DPVStatus.DPVConfirmed:
                    sDPVStatus = "DPV validated";
                    break;
                case Constants.DPVStatus.DPVNotConfirmed:
                    sDPVStatus = "WARNING - DPV not validated";
                    break;
                case Constants.DPVStatus.DPVConfirmedMissingSec:
                    sDPVStatus = "DPV validated but secondary number incorrect or missing";
                    break;
                case Constants.DPVStatus.DPVLocked:
                    sDPVStatus = "WARNING - DPV validation locked";
                    break;
                case Constants.DPVStatus.DPVSeedHit:
                    sDPVStatus = "WARNING - DPV - Seed address hit";
                    break;
                default:
                    sDPVStatus = "";
                    break;
            }
            Request.Cookies.Set(new HttpCookie(Constants.FIELD_DPVSTATUS, sDPVStatus));
        }

        /// Add formatted address warnings to the address info
        protected void AddAddressWarnings(FormattedAddress tAddressResult)
        {
            if (tAddressResult.IsOverflow)
            {
                SetAddressInfo(GetAddressInfo() + "\nWarning: Address has overflowed the layout &#8211; elements lost");
            }
            if (tAddressResult.IsTruncated)
            {
                SetAddressInfo(GetAddressInfo() + "\nWarning: Address elements have been truncated");
            }
        }

        protected string GetRequestTag()
        {
            return Request.Form["RequestTag"];
        }
    }
}
