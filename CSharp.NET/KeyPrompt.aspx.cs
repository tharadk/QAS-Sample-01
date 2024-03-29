/// QuickAddress Pro Web integration code
/// (C) QAS Ltd, www.qas.com


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
    /// Scenario "Key Search on the Web" - flattened picklists
    /// Prompt the user for specific address terms, then take them to the picklist results page
    /// This page is based on FlatBasePage, which provides functionality common to the scenario
    /// </summary>
    public partial class KeyPrompt : KeyBasePage
    {

        protected void Page_Load(object sender, System.EventArgs e)
        {
            // leave it to the event handlers (Next, Back, Alternate Prompts)
            CheckSearch();

            if (!IsPostBack)
            {
                RequestTag.Text = GetRequestTag();
            }
        }


        /** Operations **/


        /// <summary>
        /// Check if searching is available for this country/layout
        /// </summary>
        void CheckSearch()
        {
            bool bPreSearchFailed = false;
            string sErrorMessage = "";

            try
            {
                QuickAddress searchService = Global.NewQuickAddress();
                searchService.Engine = QuickAddress.EngineTypes.Keyfinder;
                searchService.Flatten = true;

                CanSearch tCanSearch = searchService.CanSearch( GetDataID(), GetLayout() );

                // Is automatic key search available for this country & layout
                if ( !tCanSearch.IsOk )
                {
                    // Keyfinder is not available for this country
                    bPreSearchFailed = true;
                    sErrorMessage = tCanSearch.ErrorMessage; 
                }
            }
            catch (Exception x)
            {
                GoErrorPage(x);
            }

            if (bPreSearchFailed)
            {
                GoErrorPage(Constants.Routes.PreSearchFailed, sErrorMessage );
            }
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

        /// <summary>
        /// 'Back' button clicked: either redisplay with standard prompt set, or go back to the first page
        /// </summary>
        protected void ButtonBack_ServerClick(object sender, System.EventArgs e)
        {
            GoFirstPage();
        }

        /// <summary>
        /// 'Next' button clicked: move on to the search page
        /// </summary>
        protected void ButtonNext_ServerClick(object sender, System.EventArgs e)
        {
            GoSearchPage();
        }
      
    }
}

