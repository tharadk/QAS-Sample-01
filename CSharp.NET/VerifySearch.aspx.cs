/// QAS Pro On Demand > (c) QAS Ltd > www.qas.com
/// Web > Verification > VerifySearch
/// Verify the address entered > Perfect match, best match, picklist of matches


using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using com.qas.proweb;							// QuickAddress services


namespace com.qas.prowebintegration
{
    /// <summary>
    /// Scenario "Address Verification on the Web" - verification with interaction
    /// Verify the address, go straight to final address page for verified matches, otherwise
    /// re-display entered address and best match or list of matches (rendered directly in page)
    ///
    /// Main arrival routes:
    ///   - Initial verification: arriving from input page
    ///   - Re-verification: manual address is re-submitted: re-verify then go to final page
    ///   - Picklist selection: retrieve address from moniker, then go to final page
    ///
    /// This page is based on VerifyBasePage, which provides functionality common to the scenario
    /// </summary>
    public partial class VerifySearch : VerifyBase
    {
        /* Members and constants */


        // Search result (contains either picklist or matched address or nothing)
        protected SearchResult m_SearchResult = null;
        protected Picklist m_Picklist = null;
        // Picklist support JavaScript strings: moniker values; refinement values
        protected System.Text.StringBuilder m_sMonikers = new System.Text.StringBuilder();
        protected System.Text.StringBuilder m_sExtras = new System.Text.StringBuilder();
        // Picklist display: explanation message - refinement prompt
        protected String m_sPicklistMessage;

        // Field name - original input address lines - to ensure we don't re-verify the same address
        protected const string FIELD_ORIGINAL_INPUT = "OriginalLine";
        // Field name - does this picklist item require additional information (refinement)
        protected const string FIELD_IS_REFINE = "IsRefine";


        /* Methods */


        /// <summary>
        /// Pick up values transfered from other pages
        /// </summary>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            //if (!IsPostBack)						// server forms are not used on this page

            if (IsInitialSearch)
            {
                // Should we jump past the interaction page, even when the match isn't perfect?
                bool bAvoidInteraction = false;
                // Copy input lines through to output by default
                SetAddressResult(GetInputAddress);

                // Check to see if they have interacted already
                if (OriginalInputAddress != null)
                {
                    // If they have re-accepted their entered address (input lines unchanged), don't re-verify it
                    if (Equals(OriginalInputAddress, GetInputAddress))
                    {
                        SetAddressInfo("address accepted unchanged, so the entered address has been used");
                        GoFinalPage();
                    }

                    // Whatever happens, go to final address page after verifying the (updated) entered address
                    bAvoidInteraction = true;
                }

                bool bGoToFinalPage = VerifyAddress(bAvoidInteraction);

                if (bGoToFinalPage)
                {
                    GoFinalPage();
                }
                else if (m_SearchResult.Picklist != null && m_SearchResult.Picklist.Total > 0 )
                {
                    // Pre-process picklist
                    m_Picklist = m_SearchResult.Picklist;
                    PreparePicklist();
                    // Display picklist of matches for user-interaction: done by page
                }
                else if (m_SearchResult.Address != null)
                {
                    // Display address match for user-interaction: done by page
                }
            }
            else if (bool.Parse(Request[FIELD_IS_REFINE]))
            {
                GoRefinePage();
            }
            else
            {
                // Format final address
                GoFinalPage(GetMoniker());
            }
        }


        /// <summary>
        /// Perform a verification search on the input lines
        /// </summary>
        /// <param name="tSearchResult">To populate with search results</param>
        /// <param name="bGoFinalPage">Set true to avoid user interaction & always move to final page </param>
        /// <returns>Move on to final address page?</returns>
        protected bool VerifyAddress(bool bGoFinalPage)
        {
            // Results
            CanSearch canSearch = null;
            try
            {
                // Verify the address
                theQuickAddress.Engine = QuickAddress.EngineTypes.Verification;
                theQuickAddress.Flatten = true;

                /// Get the layout
                string sLayout = GetLayout();

                canSearch = theQuickAddress.CanSearch(DataID, sLayout);
                if (canSearch.IsOk)
                {
                    m_SearchResult = theQuickAddress.Search(DataID, GetInputAddress, PromptSet.Types.Default, sLayout, GetRequestTag());
                    SetAddressInfo("address verification level was " + m_SearchResult.VerifyLevel.ToString());

                    if (m_SearchResult.VerifyLevel == SearchResult.VerificationLevels.Verified ||
                        (m_SearchResult.VerifyLevel == SearchResult.VerificationLevels.VerifiedPlace) ||
                        (m_SearchResult.VerifyLevel == SearchResult.VerificationLevels.VerifiedStreet))
                    {
                        // Copy found address through to output
                        SetAddressResult(m_SearchResult.Address);
                        SetDPVInfo((Constants.DPVStatus)Enum.Parse(typeof(Constants.DPVStatus), m_SearchResult.Address.DPVStatus.ToString()));   
                        bGoFinalPage = true;
                    }
                    else if (bGoFinalPage)
                    {
                        // Second time round - use input address and explain why
                        SetAddressInfo(GetAddressInfo() + ", so the entered address has been used");
                    }
                    else if (m_SearchResult.Address != null)
                    {
                        // We're going to offer the found address, so display warnings
                        AddAddressWarnings(m_SearchResult.Address);
                        SetDPVInfo((Constants.DPVStatus)Enum.Parse(typeof(Constants.DPVStatus), m_SearchResult.Address.DPVStatus.ToString()));   
                    }
                }
            }
            catch (Exception x)
            {
                GoErrorPage(x);
            }

            if (!canSearch.IsOk)
            {
                GoErrorPage(Constants.Routes.PreSearchFailed, canSearch.ErrorMessage);
            }

            return bGoFinalPage;
        }


        /// <summary>
        /// Prepare picklist control and display properties:
        /// - JavaScript string array of monikers, and 'must-refine' booleans
        /// </summary>
        protected void PreparePicklist()
        {
            m_sMonikers.Length = 0;
            m_sMonikers.Append("'");
            m_sExtras.Length = 0;

            for (int i=0; i < m_Picklist.Items.Length; ++i)
            {
                // Build JavaScript string arrays
                m_sMonikers.Append(m_Picklist.Items[i].Moniker);
                m_sMonikers.Append("','");
                bool bRefine = MustRefine(m_Picklist.Items[i]);
                m_sExtras.Append(bRefine ? "true," : "false,");
            }

            // Remove trailing characters
            m_sMonikers.Length -= 2;
            m_sExtras.Length--;
        }


        /** Helpers **/


        /// <summary>
        /// Provide member-wise string array comparison
        /// </summary>
        /// <param name="asLHS">First argument, left-hand side</param>
        /// <param name="asRHS">Second argument, right-hand side</param>
        /// <returns>Are the two string arrays equal, member-by-member</returns>
        protected static bool Equals(string[] asLHS, string[] asRHS)
        {
            if (asLHS.Length != asRHS.Length)
            {
                return false;
            }
            // Same length, so compare them member-by-member
            for (int iIndex = 0; iIndex < asLHS.Length; ++iIndex)
            {
                if (asLHS[iIndex] != asRHS[iIndex])
                {
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        /// Must the picklist item be refined (text added) to form a final address?
        /// </summary>
        protected static bool MustRefine(PicklistItem item)
        {
            return (item.IsIncompleteAddress || item.IsUnresolvableRange || item.IsPhantomPrimaryPoint);
        }


        /** Page events **/


        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }
        #endregion


        /** Page parameters **/


        // Input //


        // Country data identifier (i.e. AUS)
        protected string DataID
        {
            get
            {
                return Request[Constants.FIELD_DATA_ID];
            }
        }

        // Is this the result of the initial search, or a subsequent picklist selection/refinement
        protected bool IsInitialSearch
        {
            get
            {
                return (GetMoniker() == null);
            }
        }

        // Originally submitted address
        private string[] OriginalInputAddress
        {
            get
            {
                return Request.Form.GetValues(FIELD_ORIGINAL_INPUT);
            }
        }


        // Output //
        // Address found
        protected AddressLine[] AddressLines
        {
            get
            {
                return (m_SearchResult != null && m_SearchResult.Address != null)
                    ? m_SearchResult.Address.AddressLines
                    : null;
            }
        }

        // Array of picklist items
        protected PicklistItem[] PicklistItems
        {
            get
            {
                return (m_Picklist != null) ? m_Picklist.Items : null;
            }
        }


        // Picklist refinement overview/description text
        protected string PicklistMessage
        {
            get
            {
                if (m_sPicklistMessage != null)
                {
                    return m_sPicklistMessage;
                }
                else if (m_SearchResult != null)
                {
                    switch (m_SearchResult.VerifyLevel)
                    {
                        case SearchResult.VerificationLevels.PremisesPartial:
                            return "Your address appears to be missing secondary information. " +
                                "Please select from the list below.";
                        case SearchResult.VerificationLevels.StreetPartial:
                            return "Your house information appears to be missing or not recognised. " +
                                "Please select from the list below.";
                        case SearchResult.VerificationLevels.Multiple:
                            return "Your details have matched a number of addresses. " +
                                "Please select from the list below.";
                    }
                }

                return "Your selection covers a range of addresses. Please enter your exact details.";
            }
        }

        // Picklist refinement text box prompt
        protected string PicklistPrompt
        {
            get
            {
                if (m_SearchResult != null)
                {
                    switch (m_SearchResult.VerifyLevel)
                    {
                        case SearchResult.VerificationLevels.PremisesPartial:
                            return "Enter apartment, flat or unit number";
                        case SearchResult.VerificationLevels.StreetPartial:
                            return "Enter house number or organisation";
                    }
                }

                return m_Picklist.Prompt;
            }
        }

        // Moniker for current picklist
        protected string PicklistMoniker
        {
            get
            {
                return m_Picklist.Moniker;
            }
        }

        // JavaScript boolean array of 'must refine?', one for each picklist item
        protected string PicklistExtrasStringArray
        {
            get
            {
                return m_sExtras.ToString();
            }
        }

        // JavaScript string array of monikers, one for each picklist item
        protected string PicklistMonikersStringArray
        {
            get
            {
                return m_sMonikers.ToString();
            }
        }

    }
}
