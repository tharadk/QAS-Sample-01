/// QAS Pro On Demand > (c) QAS Ltd > www.qas.com
/// Intranet > Rapid Addressing > Standard > RapidBase
/// Provide common functionality and value transfer


using System;
using System.Text;						// StringBuilder
using System.Collections;				// ArrayList class
using System.Net;                       // Proxy class
using com.qas.proweb;					// QuickAddress services


namespace com.qas.prowebintegration
{
    /// <summary>
    /// Intranet > Rapid Addressing > Standard > RapidBase
    /// This is the base class for the pages of the scenario
    /// It provides common functionality and facilitates inter-page value passing through the ViewState
    /// </summary>
    public class RapidBasePage : System.Web.UI.Page
    {
        /** Attributes & Constants **/

        
        static private QuickAddress s_searchService = null;
        // Store the state of the previous page
        private RapidBasePage StoredPage = null;
        // Picklist history, stored in ViewState
        protected HistoryStack m_aHistory = null;
        // List of datamaps available on server
        protected Dataset[] m_atDatasets = null;

        // Enumerate operations that can be performed on a picklist item
        protected enum Commands
        {
            StepIn,									// Step in to sub-picklist
            ForceFormat,							// Force-accept an unrecognised address
            Format,									// Format into final address
            HaltRange,								// User must enter a value within the range shown
            HaltIncomplete,							// User must enter premise details
            None									// No hyperlink action - self-explanatory informational
        };

        // Enumerate the picklist item types (affects icon displayed)
        protected enum Types
        {
            Alias,									// Picklist item is an alias (synonym)
            Info,									// Picklist item is an informational
            InfoWarn,								// Picklist item is a warning informational
            Name,									// Picklist item is a name/person 
            NameAlias,								// Picklist item is a name alias (i.e. forename synonym)
            POBox,									// Picklist item is a PO Box grouping
            Standard								// Picklist item is standard
        };

        // Picklist step-in warnings (displayed on next page)
        protected enum StepinWarnings
        {
            None,									// No warning
            CloseMatches,							// Auto-stepped past close matches
            CrossBorder,							// Stepped into cross-border match
            ForceAccept,							// Force-format step-in performed
            Info,									// Stepped into informational item (i.e. 'Click to Show All')
            Overflow,								// Address elements have overflowed the layout
            PostcodeRecode,							// Stepped into postcode recode
            Truncate,								// Address elements have been truncated by the layout
            DpvStatusConf,							// Delivery point validation Status : DPV Confirmed
            DpvStatusUnConf,						// DPV UnConfirmed
            DpvStatusConfMisSec,					// DPV Confirmed but missing secondary
            DpvLocked,								// DPV Locked
            DpvSeedHit								// DPV Seed Address was Hit
        };

        // Filenames
        private const string PAGE_SEARCH = "RapidSearch.aspx";
        private const string PAGE_FORMAT = "RapidAddress.aspx";

        // Viewstate names
        protected const string FIELD_CALLBACK = "Callback";
        private const string FIELD_ENGINE = "Engine";
        private const string FIELD_HISTORY = "History";
        private const string FIELD_WARNING = "Warning";
        private const string FIELD_DATALIST = "Datalist";

        // Select Box width constants 
        protected const int MAX_DATAMAP_NAME_LENGTH = 26;
        protected const string SELECT_WIDTH = "16em";

        /** Base methods **/


        /// <summary>
        /// Pick up the preceding page, so we can access it's ViewState (see Stored properties section)
        /// </summary>
        virtual protected void Page_Load(object sender, System.EventArgs e)
        {
            if (!IsPostBack && (Context.Handler is RapidBasePage))
            {
                // Retrieve the state of the previous page, so it is available to us
                StoredPage = Context.Handler as RapidBasePage;

                StoredCallback = StoredPage.StoredCallback;
                StoredDataID = StoredPage.StoredDataID;
                StoredSearchEngine = StoredPage.StoredSearchEngine;
                StoredErrorInfo = StoredPage.StoredErrorInfo;
                StoredMoniker = StoredPage.StoredMoniker;
                StoredRoute = StoredPage.StoredRoute;
                StoredWarning = StoredPage.StoredWarning;
                StoredDataMapList = StoredPage.StoredDataMapList;
            }
            else
            {
                // Point stored page to us, as we are the previous page
                StoredPage = this;
            }

            
            // Pick up history, passed around in viewstate
            m_aHistory = StoredPage.GetStoredHistory();
        }


        /// <summary>
        /// Store the history back to the view state prior to rendering
        /// </summary>
        /// <returns></returns>
        protected override object SaveViewState()
        {
            SetStoredHistory(m_aHistory);
            return base.SaveViewState();
        }


        /** Common methods **/


        /// <summary>
        /// Access the QuickAddress service, connected to the configured server
        /// Singleton pattern: maintain a single instance, created only on demand
        /// </summary>
        protected QuickAddress theQuickAddress
        {
            get
            {
                if (s_searchService == null)
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
                        s_searchService = new QuickAddress(sServerURL, sUsername, sPassword, proxy);
                    }
                    else
                    {
                        s_searchService = new QuickAddress(sServerURL, sUsername, sPassword);
                    }
                }
                return s_searchService;
            }
        }

        /// Get the layout from the config file
        protected string GetLayout()
        {
            string sLayout;
            string sDataID = StoredDataID;

            // Look for a layout specific to this datamap 
            sLayout = System.Configuration.ConfigurationManager.AppSettings[Constants.KEY_LAYOUT + "." + sDataID];

            if (sLayout == null || sLayout == "")
            {
                // No layout found specific to this datamap - try the default
                sLayout = System.Configuration.ConfigurationManager.AppSettings[Constants.KEY_LAYOUT];
            }

            return sLayout;
        }

        /// <summary>
        /// Transfer to the address searching and picklist display page
        /// </summary>
        protected void GoSearchPage(string sDataID, QuickAddress.EngineTypes eEngine)
        {
            // Store values back to the view state
            StoredPage.StoredDataID = sDataID;
            StoredPage.SetStoredHistory(m_aHistory);
            StoredPage.StoredSearchEngine = eEngine;

            Server.Transfer(PAGE_SEARCH);
        }

        /// <summary>
        /// Transfer to the address confirmation page to retrieve the found address
        /// </summary>
        protected void GoFormatPage(string sDataID, QuickAddress.EngineTypes eEngine, string sMoniker, StepinWarnings eWarn)
        {
            // Store values back to the view state
            StoredPage.StoredDataID = sDataID;
            StoredPage.StoredMoniker = sMoniker;
            StoredPage.SetStoredHistory(m_aHistory);
            StoredPage.StoredRoute = Constants.Routes.Okay;
            StoredPage.StoredSearchEngine = eEngine;
            StoredPage.StoredWarning = eWarn;

            Server.Transfer(PAGE_FORMAT);
        }

        /// <summary>
        /// Transfer to the address confirmation page for manual address entry, after capture failure
        /// </summary>
        protected void GoErrorPage(Constants.Routes route, string sReason)
        {
            StoredPage.StoredErrorInfo = sReason;
            StoredPage.StoredRoute = route;
            Server.Transfer(PAGE_FORMAT);
        }

        /// <summary>
        /// Transfer to the address confirmation page for manual address entry, after exception thrown
        /// </summary>
        protected void GoErrorPage(Exception x)
        {
            StoredPage.StoredErrorInfo = x.Message;
            StoredPage.StoredRoute = Constants.Routes.Failed;
            Server.Transfer(PAGE_FORMAT);
        }


        /** Page display **/


        /// <summary>
        /// Write out picklist HTML and associated action array to Javascript variables
        /// This is included in two distinct places:
        ///   - main searching page, for whole page updating
        ///   - picklist results frame, for dynamic picklist updating
        /// The picklist picks up and uses the values from the appropriate place
        /// </summary>
        /// <param name="picklist"></param>
        protected void RenderPicklistData(Picklist picklist, string sDepth)
        {
            if (picklist == null)
            {
                // No picklist in this context: write out empty values
                Response.Write("var sPicklistHTML = '';\n");
                Response.Write("var asActions = new Array();");
            }
            else
            {
                // Build sActions into a string, while writing picklistHTML out to Response
                StringBuilder sActions = new StringBuilder();


                /// Build picklist HTML into JavaScript string
                ///   - icon, operation, display text, hover text, postcode, score

                Response.Write("var sPicklistHTML = \"<table class='picklist indent" + sDepth + "'>\\\n");

                for (int i = 0; i < picklist.Length; ++i)
                {
                    PicklistItem item = picklist.Items[i];

                    // Step-in warning
                    StepinWarnings eWarn = StepinWarnings.None;
                    if (item.IsCrossBorderMatch)
                    {
                        eWarn = StepinWarnings.CrossBorder;
                    }
                    else if (item.IsPostcodeRecoded)
                    {
                        eWarn = StepinWarnings.PostcodeRecode;
                    }

                    // Commands: what to do if they click on the item
                    Commands eCmd = Commands.None;
                    if (item.CanStep)
                    {
                        eCmd = Commands.StepIn;
                    }
                    else if (item.IsFullAddress)
                    {
                        eCmd = (item.IsInformation) ? Commands.ForceFormat : Commands.Format;
                    }
                    else if (item.IsUnresolvableRange)
                    {
                        eCmd = Commands.HaltRange;
                    }
                    else if (item.IsIncompleteAddress)
                    {
                        eCmd = Commands.HaltIncomplete;
                    }

                    // Type: indicates the type of icon to display (used in combination with the operation)
                    Types eType = Types.Standard;
                    if (item.IsInformation)
                    {
                        eType = (item.IsWarnInformation) ? Types.InfoWarn : Types.Info;
                        eWarn = StepinWarnings.Info;
                    }
                    else if (item.IsDummyPOBox)
                    {
                        eType = Types.POBox;
                    }
                    else if (item.IsName)
                    {
                        eType = (item.IsAliasMatch) ? Types.NameAlias : Types.Name;
                    }
                    else if (item.IsAliasMatch || item.IsCrossBorderMatch || item.IsPostcodeRecoded)
                    {
                        eType = Types.Alias;
                    }

                    // Start building HTML

                    // Set the class depending on the function & type -> displayed icon
                    string sClass = "stop";
                    if (eCmd == Commands.StepIn)
                    {
                        if (eType == Types.Alias)
                        {
                            sClass = "aliasStep";
                        }
                        else if (eType == Types.Info)
                        {
                            sClass = "infoStep";
                        }
                        else if (eType == Types.POBox)
                        {
                            sClass = "pobox";
                        }
                        else
                        {
                            sClass = "stepIn";
                        }
                    }
                    else if (eCmd == Commands.Format)
                    {
                        if (eType == Types.Alias)
                        {
                            sClass = "alias";
                        }
                        else if (eType == Types.Name)
                        {
                            sClass = "name";
                        }
                        else if (eType == Types.NameAlias)
                        {
                            sClass = "nameAlias";
                        }
                        else
                        {
                            sClass = "format";
                        }
                    }
                    else if ((eCmd == Commands.HaltIncomplete) || (eCmd == Commands.HaltRange))
                    {
                        sClass = "halt";
                    }
                    else if (eType == Types.Info)
                    {
                        sClass = "info";
                    }

                    if (i == 0)
                    {
                        sClass += " first";
                    }

                    // Hyperlink
                    string sAnchorStart = "", sAnchorEnd = "";
                    if (eCmd != Commands.None)
                    {
                        sAnchorStart = "<a href='javascript:action(" + i.ToString() + ");' "
                            + "tabindex='" + (i + 1) + "' "
                            + "title=\\\"" + JavascriptEncode(item.PartialAddress) + "\\\">";
                        sAnchorEnd = "</a>";
                    }

                    string sScore = (item.Score > 0) ? item.Score + "%" : "";

                    // Write out HTML

                    Response.Write("<tr>");
                    Response.Write("<td class='pickitem " + sClass + "'>" + sAnchorStart + "<div>");
                    Response.Write(JavascriptEncode(Server.HtmlEncode(item.Text)) + "</div>" + sAnchorEnd + "</td>");
                    Response.Write("<td class='postcode'>" + JavascriptEncode(Server.HtmlEncode(item.Postcode)) + "</td>");
                    Response.Write("<td class='score'>" + sScore + "</td>");
                    Response.Write("</tr>\\\n");

                    /// Picklist actions - javascript array variable

                    sActions.Append("'" + (eCmd != Commands.None ? eCmd.ToString() : ""));
                    switch (eCmd)
                    {
                        case Commands.StepIn:
                            sActions.Append("(\"" + item.Moniker + "\",\"" + JavascriptEncode(Server.HtmlEncode(item.Text)) + "\",");
                            sActions.Append("\"" + item.Postcode + "\",\"" + item.ScoreAsString + "\",\"" + eWarn.ToString() + "\")");
                            break;
                        case Commands.Format:
                            sActions.Append("(\"" + item.Moniker + "\",\"" + eWarn.ToString() + "\")");
                            break;
                        case Commands.ForceFormat:
                            sActions.Append("(\"" + item.Moniker + "\")");
                            break;
                        case Commands.HaltIncomplete:
                        case Commands.HaltRange:
                            sActions.Append("()");
                            break;
                    }
                    sActions.Append("',");
                }

                // Close off picklist HTML
                Response.Write("</table>\";\n");


                /// Write out Actions
                
            Response.Write("var asActions = new Array(");
            Response.Write(sActions.ToString());
            Response.Write("'');\n");
            }
        }

        /// <summary>
        /// Method override: Use picklist history in order to work out indent depth
        /// </summary>
        protected void RenderPicklistData(Picklist picklist)
        {
            RenderPicklistData(picklist, m_aHistory.Count.ToString());
        }


        /// <summary>
        /// Encode the string so it's value is correct when used as a Javascript string
        /// i.e. Jack's "friendly" dog  ->  Jack\'s \"friendly\" dog
        /// </summary>
        /// <param name="sIn">Plain text string to encode</param>
        /// <returns>String with special characters escaped</returns>
        protected string JavascriptEncode(string str)
        {
            return str.Replace("\\", "\\\\").Replace("'", "\\'").Replace("\"", "\\\"");
        }


        /** Stored properties **/


      /// Name of Javascript function to call on completion
        protected string StoredCallback
        {
            get
            {
                return (string) ViewState[FIELD_CALLBACK];
            }
            set
            {
                ViewState[FIELD_CALLBACK] = value;
            }
        }
        
        /// Country data identifier (i.e. AUS)
        protected string StoredDataID
        {
            get
            {
                return (string) ViewState[Constants.FIELD_DATA_ID];
            }
            set
            {
                ViewState[Constants.FIELD_DATA_ID] = value;
            }
        }

        /// Stored List of datamaps available on server
        protected Dataset[] StoredDataMapList
        {
            get
            {
                return (Dataset[])ViewState[FIELD_DATALIST];
            }
            set
            {
                ViewState[FIELD_DATALIST] = value;
            }
        }

        /// Additional address/error information
        protected string StoredErrorInfo
        {
            get
            {
                return (string) ViewState[Constants.FIELD_ERROR_INFO];
            }
            set
            {
                ViewState[Constants.FIELD_ERROR_INFO] = value;
            }
        }

        /// Moniker of the address
        protected string StoredMoniker
        {
            get
            {
                return (string) ViewState[Constants.FIELD_MONIKER];
            }
            set
            {
                ViewState[Constants.FIELD_MONIKER] = value;
            }
        }

        /// How we arrived on the formatting page (i.e. pre-search check failed)
        protected Constants.Routes StoredRoute
        {
            get
            {
                object objValue = ViewState[Constants.FIELD_ROUTE];
                return (objValue != null) ? (Constants.Routes)objValue : Constants.Routes.Undefined;
            }
            set
            {
                ViewState[Constants.FIELD_ROUTE] = value;
            }
        }

        /// Search engine selected
        protected QuickAddress.EngineTypes StoredSearchEngine
        {
            get
            {
                object objValue = ViewState[FIELD_ENGINE];
                return (objValue != null) ? (QuickAddress.EngineTypes)objValue : QuickAddress.EngineTypes.Typedown;
            }
            set
            {
                ViewState[FIELD_ENGINE] = value;
            }
        }

        /// Step-in warning (i.e. Postcode has been recoded)
        protected StepinWarnings StoredWarning
        {
            get
            {
                object objValue = ViewState[FIELD_WARNING];
                return (objValue != null) ? (StepinWarnings)objValue : StepinWarnings.None;
            }
            set
            {
                ViewState[FIELD_WARNING] = value;
            }
        }

        /// Picklist history, get
        protected HistoryStack GetStoredHistory()
        {
            object objValue = ViewState[FIELD_HISTORY];
            if (objValue is ArrayList)
            {
                HistoryStack stack = new HistoryStack((ArrayList)objValue);
                return stack;
            }
         return new HistoryStack();
        }

      /// Picklist history, set
        protected void SetStoredHistory(HistoryStack value)
        {
            ViewState[FIELD_HISTORY] = value;
        }


        /** Support classes: Picklist history **/


        /// <summary>
        /// Helper class: stack of all the search picklists we've stepped through
        /// Implemented using an ArrayList so we can enumerate forwards through them for display,
        /// the 'bottom' of the stack is element 0, the 'top' is element Count - 1, where items are pushed and popped
        /// </summary>
        [Serializable()]
        protected class HistoryStack : ArrayList
        {
            /// Default constructor
            public HistoryStack()
            {
            }

            /// Construct from an ArrayList
            public HistoryStack(ArrayList vValue)
            {
                foreach (object obj in vValue)
                {
                    base.Add((HistoryItem)obj);
                }
            }

            /// Inserts an object at the top of the stack: prevents duplicates
            public void Add(HistoryItem item)
            {
                if (Count == 0 || !Peek().Moniker.Equals(item.Moniker))
                {
                    base.Add(item);
                }
            }

            /// Returns the object at the top of the stack without removing it
            public HistoryItem Peek()
            {
                return (HistoryItem)this[Count - 1];
            }

            /// Removes and returns the object at the top of the stack
            public HistoryItem Pop()
            {
                HistoryItem tail = Peek();
                RemoveAt(Count - 1);
                return tail;
            }

            /// Inserts an object at the top of the stack
            public void Push(string sMoniker, string sText, string sPostcode, string sScore, string sRefine)
            {
                HistoryItem item = new HistoryItem(sMoniker, sText, sPostcode, sScore, sRefine);
                Add(item);
            }

            /// Inserts an object at the top of the stack
            public void Push(PicklistItem item)
            {
                Push(item.Moniker, item.Text, item.Postcode, item.ScoreAsString, "");
            }

            /// Truncate the stack down to a certain size
            public void Truncate(int iCount)
            {
                if (Count > iCount)
                {
                    RemoveRange(iCount, Count - iCount);
                }
            }

            /// Gets or sets the element at the specified index
            public new HistoryItem this[int iIndex]
            {
                get
                {
                    return (HistoryItem)base[iIndex];
                }
                set
                {
                    base[iIndex] = value;
                }
            }
        }


        /// <summary>
        /// Helper class: stores details of search picklists visited
        /// </summary>
        [Serializable()]
        protected class HistoryItem
        {
            public string Moniker = "";
            public string Text = "";
            public string Postcode = "";
            public string Score = "";
            public string Refine = "";

            public HistoryItem (string sMonikerIn, string sTextIn, string sPostcodeIn, string sScoreIn, string sRefineIn)
            {
                Moniker = sMonikerIn;
                Text = sTextIn;
                Postcode = sPostcodeIn;
                Score = sScoreIn;
                Refine = sRefineIn;
            }
        }
    }
}
