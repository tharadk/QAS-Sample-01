/// QAS Pro On Demand > (c) QAS Ltd > www.qas.com
/// Web > Verification > VerifyRefine
/// Prompt for additional (premise) address details, and check for suitability


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
	/// Scenario "Address Verification on the Web" - verification with prompting
	/// Prompts for additional (premise) address details, and checks submissions for suitability
	/// This is only called if an unresolvable range, incomplete address or phantom primary point (AUS)
	/// is selected: Refine must be called in order to pass additional info into the final address.
	///
	/// This page is based on VerifyBasePage, which provides functionality common to the scenario
	/// </summary>
	public partial class VerifyRefine : VerifyBase
	{
		/** Memebers **/




		/** Methods **/


		/// <summary>
		/// Pick up values transfered from other pages
		/// </summary>
		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Pre-set event to call when <Enter> is hit (otherwise no event will be raised)
			ClientScript.RegisterHiddenField("__EVENTTARGET", ButtonNext.ClientID);

			if (!IsPostBack)
			{
				// Pick up values from Verify Interaction (picklist) page
				Country = GetCountry();
				Moniker = GetMoniker();
			}

         RefineAddress();
		}


		/// <summary>
		/// Try to refine the current Moniker using the entered refinement text
		/// </summary>
		protected void RefineAddress()
		{
			PicklistItem item = null;
			bool bGoFinalPage = false;		// go to the final address page?

			try
			{
				// Perform the refinement
				Picklist picklist = theQuickAddress.Refine(Moniker, RefinementText);

				// If the refined search produces no results, recreate the picklist and update the message
				if (picklist.Length == 0)
				{
					// No acceptable address match - recreate without using refinement text
					picklist = theQuickAddress.Refine(Moniker, "");
				}

				// Update page content
				item = picklist.Items[0];
				LiteralRefineLine.Text = item.Text;
				LiteralRefineAddress.Text = item.PartialAddress;

				if (RefinementText == "")
				{
					// First time through - just display
					bool bIsPhantom = (item.IsPhantomPrimaryPoint);
					LiteralMessage.Text = bIsPhantom
						? "Your selection requires secondary information. Enter your exact details."
						: "Your selection covers a range of addresses. Enter your exact details.";
				}
				else if (item.IsUnresolvableRange)
				{
					// Redisplay with explanation
					LiteralMessage.Text = "You entered '" + RefinementText + "', but this address is outside of the range. Please try again.";
				}
				else
				{
					// Accept (or force accept in the Phantom case)
					bGoFinalPage = true;
				}
			}
			catch (Exception x)
			{
				GoErrorPage(x);
			}

			if (bGoFinalPage)
			{
				SetCountry(Country);
				GoFinalPage(item.Moniker);
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


		/** Page controls **/

        // Country data identifier (i.e. AUS)
        protected string DataID
        {
            get
            {
                return Request[Constants.FIELD_DATA_ID];
            }
        }


		// Country display name (i.e. Australia)
		protected string Country
		{
			get
			{
				return HiddenCountry.Value;
			}
			set
			{
				HiddenCountry.Value = value;
			}
		}

		// Moniker of the final address
		protected string Moniker
		{
			get
			{
				return HiddenMoniker.Value;
			}
			set
			{
				HiddenMoniker.Value = value;
			}
		}

		// Additional text entered by user
		protected string RefinementText
		{
			get
			{
				return TextRefinement.Value;
			}
			set
			{
				TextRefinement.Value = value;
			}
		}
	}
}
