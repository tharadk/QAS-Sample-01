/// QAS Pro On Demand integration code
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
	/// Scenario "Address Capture on the Web" - flattened picklists
	/// Display the address requiring extra information, then do the refinement and take them to the final address page
	/// This page is based on FlatBasePage, which provides functionality common to the scenario
	/// </summary>
	public partial class FlatRefine : FlatBasePage
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Pre-set event to call when <Enter> is hit (otherwise no event will be raised)
			ClientScript.RegisterHiddenField("__EVENTTARGET", ButtonNext.ClientID);

			if (!IsPostBack)
			{
				RequestTag.Text = GetRequestTag();
				RefineAddress();
			}
			// Else leave it to the event handlers (Next, Back)
		}



		/** Search operations **/


		/// <summary>
		/// Try to refine the current Moniker using the entered refinement text
		/// </summary>
		protected void RefineAddress()
		{
			PicklistItem item = null;
			bool bFinalAddress = false;		// jump straight to the final address page?

			try
			{
				// Perform the refinement
                QuickAddress searchService = Global.NewQuickAddress();
				Picklist picklist = searchService.Refine(GetRefineMoniker(), RefinementText, RequestTag.Text);

				// If the refined search produces no results, recreate the picklist and update the message

				if (picklist.Length == 0)
				{
					// No acceptable address match - recreate without using refinement text
					picklist = searchService.Refine(GetRefineMoniker(), "");
				}

				item = picklist.Items[0];
				// Update page content
				LiteralRefineLine.Text = item.Text;
				LiteralRefineAddress.Text = item.PartialAddress;

				if (RefinementText.Equals(""))
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
					LiteralMessage.Text = "You entered '" + RefinementText + "', but this address is outside of the range. "
						+ "Please try again or click Back and select the correct range.";
				}
				else
				{
					// Accept (or force accept in the Phantom case)
					bFinalAddress = true;
				}
			}
			catch (Exception x)
			{
            GoErrorPage(x);
			}

			if (bFinalAddress)
			{
				GoFormatPage(item.Moniker);
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
		/// 'Back' button clicked
		/// </summary>
		protected void ButtonBack_ServerClick(object sender, System.EventArgs e)
		{
			GoSearchPage();
		}

		/// <summary>
		/// 'Next' button clicked: verify entered text, and go forward if possible, else display error
		/// </summary>
		protected void ButtonNext_ServerClick(object sender, System.EventArgs e)
		{
			RefineAddress();
		}



		/** Page controls **/


		private string RefinementText
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
