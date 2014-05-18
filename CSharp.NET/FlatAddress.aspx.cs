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
	/// Retrieve the final formatted address, display to user for confirmation
	/// This page is based on FlatBasePage, which provides functionality common to the scenario
	/// </summary>
	public partial class FlatAddress : FlatBasePage
	{
      // Result arrays
      protected string[] m_asAddressLines = null;
		protected string[] m_asAddressLabels = null;
		// Route (how we got here, search status); keep a local copy, for efficiency and so we can change it
		protected Constants.Routes m_eRoute = Constants.Routes.Undefined;


		protected void Page_Load(object sender, System.EventArgs e)
		{
			SetRoute(base.GetRoute());
			if (!IsPostBack)
			{
				FormatAddress(ref m_asAddressLabels, ref m_asAddressLines);
				SetWelcomeMessage(GetRoute());
			}
			// Else leave it to the event handlers (Accept, Back)
		}



		/** Operations **/


		/// <summary>
		/// Retrieve the formatted address from the Moniker, or create a set of blank lines
		/// </summary>
		protected void FormatAddress(ref string[] asLabels, ref string[] asLines)
		{
			if (!(GetRoute().Equals(Constants.Routes.PreSearchFailed) || GetRoute().Equals(Constants.Routes.Failed)))
			{
				try
				{
					QuickAddress searchService = Global.NewQuickAddress();

					AddressLine[] aLines;
					if (GetRoute().Equals(Constants.Routes.Okay))
					{
                        FormattedAddress result = null;
                        // Perform address formatting
                        result = searchService.GetFormattedAddress(GetMoniker(), GetLayout(), GetRequestTag());
                        SetDPVMessage((Constants.DPVStatus)Enum.Parse(typeof(Constants.DPVStatus), result.DPVStatus.ToString()));
						aLines = result.AddressLines;
					}
					else
					{
						// Use first example address to get line labels
						aLines = searchService.GetExampleAddresses(GetDataID(), GetLayout())[0].AddressLines;
					}

					// Build display address arrays
					int iSize = aLines.Length;
					asLabels = new string[iSize];
					asLines = new String[iSize];
					for (int i = 0; i < iSize; i++)
					{
						asLabels[i] = aLines[i].Label;
						asLines[i] = aLines[i].Line;
					}
				}
				catch (Exception x)
				{
					SetRoute(Constants.Routes.Failed);
					SetErrorInfo(x.Message);
				}
			}

			if (asLabels == null || asLines == null)
			{
				// Provide default (empty) address for manual entry
				asLabels = new string[]
				{
					"Address Line 1", "Address Line 2", "Address Line 3",
					"City", "State or Province", "ZIP or Postal Code"
				};
				asLines = new string[]
				{
					"", "", "", "", "", ""
				};
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
			switch (GetRoute())
			{
				case Constants.Routes.NoMatches:
				case Constants.Routes.Timeout:
				case Constants.Routes.TooManyMatches:
					GoInputPage();
					break;
				case Constants.Routes.Okay:
					if (!GetRefineMoniker().Equals(""))
					{
						GoRefinementPage(null);
					}
					else
					{
						GoSearchPage();
					}
					break;
				default:
					GoFirstPage();
					break;
			}
		}

		/// <summary>
		/// 'Accept' button clicked: move out of this scenario
		/// </summary>
		protected void ButtonAccept_ServerClick(object sender, System.EventArgs e)
		{
			GoFinalPage();
		}



		/** Page controls **/


		/// <summary>
		/// Update the page welcome depending on the route we took to get here
		/// </summary>
		private void SetWelcomeMessage(Constants.Routes eRoute)
		{
			switch (eRoute)
			{
				case Constants.Routes.Okay:
					LiteralMessage.Text = "Please confirm your address below.";
					break;
				case Constants.Routes.NoMatches:
				case Constants.Routes.Timeout:
				case Constants.Routes.TooManyMatches:
					LiteralMessage.Text = "Automatic address capture did not succeed.<br /><br />Please search again or enter your address below.";
					break;
				default:
					LiteralMessage.Text = "Automatic address capture is not available.<br /><br />Please enter your address below.";
					break;
			}
		}

        private void SetDPVMessage(Constants.DPVStatus status)
        {
            switch (status)
            {
                case Constants.DPVStatus.DPVConfirmed:
                    DPVMessage.Text = "DPV validated";
                    break;
                case Constants.DPVStatus.DPVNotConfirmed:
                    DPVMessage.Text = "WARNING - DPV not validated";
                    break;
                case Constants.DPVStatus.DPVConfirmedMissingSec:
                    DPVMessage.Text = "DPV validated but secondary number incorrect or missing";
                    break;
                case Constants.DPVStatus.DPVLocked:
                    DPVMessage.Text = "WARNING - DPV validation locked";
                    break;
                case Constants.DPVStatus.DPVSeedHit:
                    DPVMessage.Text = "WARNING - DPV - Seed address hit";
                    break;
                default:
                    DPVMessage.Text = "";
                    break;
            }
            Request.Cookies.Set(new HttpCookie(Constants.FIELD_DPVSTATUS, DPVMessage.Text));
        }
		/// Current search state, how we arrived on the address format page (i.e. too many matches)
		protected new Constants.Routes GetRoute()
		{
			return m_eRoute;
		}
		protected void SetRoute(Constants.Routes eRoute)
		{
			m_eRoute = eRoute;
		}
	}
}
