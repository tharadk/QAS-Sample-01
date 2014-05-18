/// QAS Pro On Demand > (c) QAS Ltd > www.qas.com
/// Web > Verification > VerifyInput
/// Verify the address entered > Initial page - select country, enter address


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
    public partial class VerifyInput : VerifyBase
    {
        /* Members and constants */

        protected Dataset[] m_atDatasets;
        protected string m_asError;

        /* Methods */


        /// <summary>
        /// Pick up values transfered from other pages
        /// </summary>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                m_atDatasets = theQuickAddress.GetAllDatasets();
            }
            catch ( Exception x )
            {
                m_asError = x.Message;
            }
        }
    }
}
