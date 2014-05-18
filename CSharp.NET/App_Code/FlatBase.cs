/// QAS Pro On Demand integration code
/// (C) QAS Ltd, www.qas.com


using System;
using System.Web;
using System.Net;                    // Proxy class
using com.qas.proweb;							// QuickAddress services


namespace com.qas.prowebintegration
{
    /// <summary>
    /// Scenario "Address Capture on the Web" - flattened picklists
    /// This is the base class for the pages of the scenario
    /// It provides common functionality and facilitates inter-page value passing through hidden fields
    /// </summary>
    public class FlatBasePage : System.Web.UI.Page
    {
        // Page filenames
        protected const string PAGE_BEGIN = "FlatCountry.aspx";
        protected const string PAGE_INPUT = "FlatPrompt.aspx";
        protected const string PAGE_SEARCH = "FlatSearch.aspx";
        protected const string PAGE_REFINE = "FlatRefine.aspx";
        protected const string PAGE_FORMAT = "FlatAddress.aspx";

        // Field names specific to the Flattened scenario
        // Which prompt set is selected - set on PAGE_INPUT, also used by PAGE_SEARCH
        protected const string FIELD_PROMPTSET = "PromptSet";
        // Used to recreate the picklist - set and used by PAGE_SEARCH
        protected const string FIELD_PICKLIST_MONIKER = "PicklistMoniker";
        // The picklist item requiring refinement - set on PAGE_SEARCH, used by PAGE_REFINE
        protected const string FIELD_REFINE_MONIKER = "RefineMoniker";

        /// <summary>
        /// No construction necessary, provides shared functionality
        /// </summary>
        public FlatBasePage()
        {
        }



        /** Helper functions **/



        /// Transfer to the initial page, to select the country
        protected void GoFirstPage()
        {
            Server.Transfer(PAGE_BEGIN);
        }

        /// Transfer to the input page, which prompts for address terms
        protected void GoInputPage()
        {
            Server.Transfer(PAGE_INPUT);
        }

        /// Transfer to the address searching and picklist display page
        protected void GoSearchPage()
        {
            Server.Transfer(PAGE_SEARCH);
        }

        /// Transfer to the address refinement page, when additional information is required
        protected void GoRefinementPage(string sMoniker)
        {
            if (sMoniker != null)
            {
                SetRefineMoniker(sMoniker);
            }
            Server.Transfer(PAGE_REFINE);
        }

        /// Transfer to the address confirmation page to retrieve the found address
      protected void GoFormatPage(string sMoniker)
        {
            if (sMoniker != null)
            {
                SetMoniker(sMoniker);
            }
            SetRoute(Constants.Routes.Okay);
            Server.Transfer(PAGE_FORMAT);
        }

        /// Transfer to the address confirmation page for manual address entry, after capture failed
        protected void GoErrorPage(Constants.Routes route)
        {
            SetRoute(route);
            Server.Transfer(PAGE_FORMAT);
        }

        protected void GoErrorPage(Constants.Routes route, string sMessage )
        {
            SetRoute(route);
            SetErrorInfo(sMessage);
            Server.Transfer(PAGE_FORMAT);
        }

        /// Transfer to the address confirmation page for manual address entry, after exception thrown
        protected void GoErrorPage(Exception x)
        {
            SetRoute(Constants.Routes.Failed);
            SetErrorInfo(x.Message);
            Server.Transfer(PAGE_FORMAT);
        }

        /// Transfer out of the scenario to the final (summary) page
        protected void GoFinalPage()
        {
            Server.Transfer(Constants.PAGE_FINAL_ADDRESS);
        }

        /* Common field rendering routines */

        /// Propagate a value through, from the Request into a hidden field on our page
        protected void RenderRequestString(string sKey)
        {
            string sValue = Request[sKey];
            RenderHiddenField(sKey, sValue);
        }

        /// Propagate values through, from the Request to hidden fields on our page
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

        /// Render a hidden field directly into the page
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

        /// Render a boolean hidden field directly into the page
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


        /// Country data identifier (i.e. AUS)
        protected string GetDataID()
        {
            return Request[Constants.FIELD_DATA_ID];
        }

        /// Country display name (i.e. Australia)
        protected string GetCountryName()
        {
            return Request[Constants.FIELD_COUNTRY_NAME];
        }

        protected string GetLayout()
        {
            string sLayout;
            string sDataID = GetDataID();

            // Look for a layout specific to this datamap 
            sLayout = System.Configuration.ConfigurationManager.AppSettings[Constants.KEY_LAYOUT + "." + sDataID];

            if ( sLayout == null || sLayout == "" )
            {
                // No layout found specific to this datamap - try the default
                sLayout = System.Configuration.ConfigurationManager.AppSettings[Constants.KEY_LAYOUT];
            }

            return sLayout;
        }

        /// Prompt set selected
        protected PromptSet.Types GetPromptSet()
        {
            string sValue = Request[FIELD_PROMPTSET];
            return (sValue != null)
                ? (PromptSet.Types) Enum.Parse(typeof(PromptSet.Types), sValue)
                : PromptSet.Types.Optimal;
        }
        protected void SetPromptSet(PromptSet.Types ePromptSet)
        {
            Request.Cookies.Set(new HttpCookie(FIELD_PROMPTSET, ePromptSet.ToString()));
        }

        /// Initial user search (i.e. "14 main street", "boston")
        protected string[] GetInputLines()
        {
            string [] asValues = Request.Params.GetValues(Constants.FIELD_INPUT_LINES);
            return (asValues != null)
                ? asValues
                : new string[0];
        }

        /// Current search state, how we arrived on the address format page (i.e. too many matches)
        protected Constants.Routes GetRoute()
        {
            string sValue = Request[Constants.FIELD_ROUTE];
            return (sValue != null)
                ? (Constants.Routes) Enum.Parse(typeof(Constants.Routes), sValue)
                : Constants.Routes.Undefined;
        }
        private void SetRoute(Constants.Routes eRoute)
        {
            Request.Cookies.Set(new HttpCookie(Constants.FIELD_ROUTE, eRoute.ToString()));
        }

        /// Error information returned through the exception
        protected string GetErrorInfo()
        {
            return Request[Constants.FIELD_ERROR_INFO];
        }
        protected void SetErrorInfo(string sErrorInfo)
        {
            Request.Cookies.Set(new HttpCookie(Constants.FIELD_ERROR_INFO, sErrorInfo));
        }

        /// Moniker of the final address
        protected string GetMoniker()
        {
            return Request[Constants.FIELD_MONIKER];
        }
        private void SetMoniker(string sMoniker)
        {
            Request.Cookies.Set(new HttpCookie(Constants.FIELD_MONIKER, sMoniker));
        }

        /// Moniker of the initial flattened picklist
        protected string GetPicklistMoniker()
        {
            return Request[FIELD_PICKLIST_MONIKER];
        }
        protected void SetPicklistMoniker(string sMoniker)
        {
            Request.Cookies.Set(new HttpCookie(FIELD_PICKLIST_MONIKER, sMoniker));
        }

        /// Moniker of the picklist item to refine
        protected string GetRefineMoniker()
        {
            return Request[FIELD_REFINE_MONIKER];
        }

        private void SetRefineMoniker(string sMoniker)
        {
            Request.Cookies.Set(new HttpCookie(FIELD_REFINE_MONIKER, sMoniker));
        }

        protected string GetRequestTag()
        {
            return Request.Form["RequestTag"];
        }
    }
}
