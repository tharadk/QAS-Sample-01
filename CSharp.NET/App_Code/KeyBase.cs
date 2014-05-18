/// QuickAddress Pro Web integration code
/// (C) QAS Ltd, www.qas.com

using System;
using System.Web;
using com.qas.proweb;							// QuickAddress services


namespace com.qas.prowebintegration
{
    /// <summary>
    /// Scenario "Key Search" - flattened picklists
    /// This is the base class for the pages of the scenario
    /// It provides common functionality and facilitates inter-page value passing through hidden fields
    /// </summary>
    public class KeyBasePage : System.Web.UI.Page
    {
        // Page filenames
        protected const string PAGE_BEGIN = "KeyCountry.aspx";
        protected const string PAGE_INPUT = "KeyPrompt.aspx";
        protected const string PAGE_SEARCH = "KeySearch.aspx";
        protected const string PAGE_FORMAT = "KeyAddress.aspx";

        // Field names specific to the Keyfinder scenario
        // Which prompt set is selected - set on PAGE_INPUT, also used by PAGE_SEARCH
        protected const string FIELD_PROMPTSET = "PromptSet";
        // Used to recreate the picklist - set and used by PAGE_SEARCH
        protected const string FIELD_PICKLIST_MONIKER = "PicklistMoniker";
        // The picklist item requiring refinement - set on PAGE_SEARCH, used by PAGE_REFINE
        protected const string FIELD_REFINE_MONIKER = "RefineMoniker";

        /// <summary>
        /// No construction necessary, provides shared functionality
        /// </summary>
        public KeyBasePage()
        {
        }


        /** Helper functions **/


        /// <summary>
        /// Transfer to the initial page, to select the country
        /// </summary>
        protected void GoFirstPage()
        {
            Server.Transfer(PAGE_BEGIN);
        }

        /// <summary>
        /// Transfer to the input page, which prompts for address terms
        /// </summary>
        protected void GoInputPage()
        {
            Server.Transfer(PAGE_INPUT);
        }

        /// <summary>
        /// Transfer to the address searching and picklist display page
        /// </summary>
        protected void GoSearchPage()
        {
            Server.Transfer(PAGE_SEARCH);
        }
        
        /// <summary>
        /// Transfer to the address confirmation page to retrieve the found address
        /// </summary>
        /// <param name="sMoniker"></param>
        protected void GoFormatPage(string sMoniker)
        {
            if (sMoniker != null)
            {
                SetMoniker(sMoniker);
            }
            SetRoute(Constants.Routes.Okay);
            Server.Transfer(PAGE_FORMAT);
        }

        /// <summary>
        /// Transfer to the address confirmation page for manual address entry, after capture failed
        /// </summary>
        /// <param name="route"></param>
        protected void GoErrorPage(Constants.Routes route)
        {
            SetRoute(route);
            Server.Transfer(PAGE_FORMAT);
        }

        /// <summary>
        /// Transfer to the address confirmation page for manual address entry - include reason for failure
        /// </summary>
        /// <param name="route"></param>
        /// <param name="sMessage"></param>
        protected void GoErrorPage(Constants.Routes route, string sMessage )
        {
            SetRoute(route);
            SetErrorInfo(sMessage);
            Server.Transfer(PAGE_FORMAT);
        }

        /// <summary>
        /// Transfer to the address confirmation page for manual address entry, after exception thrown
        /// </summary>
        /// <param name="x"></param>
        protected void GoErrorPage(Exception x)
        {
            SetRoute(Constants.Routes.Failed);
            SetErrorInfo(x.Message);
            Server.Transfer(PAGE_FORMAT);
        }

        /// <summary>
        /// Transfer out of the scenario to the final (summary) page
        /// </summary>
        protected void GoFinalPage()
        {
            Server.Transfer(Constants.PAGE_FINAL_ADDRESS);
        }

        /* Common field rendering routines */

        /// <summary>
        /// Propagate a value through, from the Request into a hidden field on our page
        /// </summary>
        /// <param name="sKey"></param>
        protected void RenderRequestString(string sKey)
        {
            string sValue = Request[sKey];
            RenderHiddenField(sKey, sValue);
        }

        /// <summary>
        /// Propagate values through, from the Request to hidden fields on our page
        /// </summary>
        /// <param name="sKey"></param>
        protected void RenderRequestArray(string sKey)
        {
            string[] asValues = Request.Params.GetValues(sKey);
            if (asValues != null)
            {
                foreach (string sValue in asValues)
                {
                    RenderHiddenField(sKey, sValue);
                }
                // Add dummy entry to 1-sized arrays to allow array subscripting in JavaScript
                if (asValues.Length == 1)
                {
                    RenderHiddenField(sKey, null);
                }
            }
        }

        /// <summary>
        /// Render a hidden field directly into the page
        /// </summary>
        /// <param name="sKey"></param>
        /// <param name="sValue"></param>
        protected void RenderHiddenField(string sKey, string sValue)
        {
            Response.Write("<input type=\"hidden\" name=\"");
            Response.Write(sKey);
            if (sValue != null)
            {
                Response.Write("\" value=\"");
                Response.Write(HttpUtility.HtmlEncode(sValue));
            }
            Response.Write("\" />\n");
        }

        /// <summary>
        /// Render a boolean hidden field directly into the page
        /// </summary>
        /// <param name="sKey"></param>
        /// <param name="bValue"></param>
        protected void RenderHiddenField(string sKey, bool bValue)
        {
            Response.Write("<input type=\"hidden\" name=\"");
            Response.Write(sKey);
            if (bValue) // Only write a value if it is True
            {
                Response.Write("\" value=\"");
                Response.Write(true.ToString());
            }
            Response.Write("\" />\n");
        }



        /** Stored parameters **/


        /// <summary>
        /// Country data identifier (i.e. AUS)
        /// </summary>
        /// <returns></returns>
        protected string GetDataID()
        {
            return Request[Constants.FIELD_DATA_ID];
        }

        /// <summary>
        /// Country display name (i.e. Australia)
        /// </summary>
        /// <returns></returns>
        protected string GetCountryName()
        {
            return Request[Constants.FIELD_COUNTRY_NAME];
        }

        /// <summary>
        /// Layout display name (i.e Default)
        /// </summary>
        /// <returns></returns>
        protected string GetLayout()
        {
            string sLayout;
            string sDataID = GetDataID();

            // Look for a layout specific to this datamap 
            sLayout = System.Configuration.ConfigurationManager.AppSettings[Constants.KEY_LAYOUT + "." + sDataID];

            if (String.IsNullOrEmpty(sLayout))
            {
                // No layout found specific to this datamap - try the default
                sLayout = System.Configuration.ConfigurationManager.AppSettings[Constants.KEY_LAYOUT];
            }

            return sLayout;
        }

        /// <summary>
        /// Prompt set selected
        /// </summary>
        /// <returns></returns>
        protected PromptSet.Types GetPromptSet()
        {
            string sValue = Request[FIELD_PROMPTSET];
            if (sValue != null)
            {
                return (PromptSet.Types)Enum.Parse(typeof(PromptSet.Types), sValue);
            }
            else
            {
                return PromptSet.Types.Optimal;
            }
        }
        protected void SetPromptSet(PromptSet.Types ePromptSet)
        {
            Request.Cookies.Set(new HttpCookie(FIELD_PROMPTSET, ePromptSet.ToString()));
        }

        /// <summary>
        /// Initial user search (i.e. "14 main street", "boston")
        /// </summary>
        /// <returns></returns>
        protected string[] GetInputLines()
        {
            string[] asValues = Request.Params.GetValues(Constants.FIELD_INPUT_LINES);
            if (asValues != null)
            {
                return asValues;
            }
            else
            {
                return new string[0];
            }
        }

        /// <summary>
        /// Current search state, how we arrived on the address format page (i.e. too many matches)
        /// </summary>
        /// <returns></returns>
        protected Constants.Routes GetRoute()
        {
            string sValue = Request[Constants.FIELD_ROUTE];
            if (sValue != null)
            {
                return (Constants.Routes)Enum.Parse(typeof(Constants.Routes), sValue);
            }
            else
            {
                return Constants.Routes.Undefined;
            }
        }
        private void SetRoute(Constants.Routes eRoute)
        {
            Request.Cookies.Set(new HttpCookie(Constants.FIELD_ROUTE, eRoute.ToString()));
        }

        /// <summary>
        /// Error information returned through the exception
        /// </summary>
        /// <returns></returns>
        protected string GetErrorInfo()
        {
            return Request[Constants.FIELD_ERROR_INFO];
        }
        protected void SetErrorInfo(string sErrorInfo)
        {
            Request.Cookies.Set(new HttpCookie(Constants.FIELD_ERROR_INFO, sErrorInfo));
        }

        /// <summary>
        /// Moniker of the final address
        /// </summary>
        /// <returns></returns>
        protected string GetMoniker()
        {
            return Request[Constants.FIELD_MONIKER];
        }
        private void SetMoniker(string sMoniker)
        {
            Request.Cookies.Set(new HttpCookie(Constants.FIELD_MONIKER, sMoniker));
        }

        /// <summary>
        /// Moniker of the initial flattened picklist
        /// </summary>
        /// <returns></returns>
        protected string GetPicklistMoniker()
        {
            return Request[FIELD_PICKLIST_MONIKER];
        }

        protected void SetPicklistMoniker(string sMoniker)
        {
            Request.Cookies.Set(new HttpCookie(FIELD_PICKLIST_MONIKER, sMoniker));
        }

        protected string GetRequestTag()
        {
            return Request.Form["RequestTag"];
        }
    }
}
