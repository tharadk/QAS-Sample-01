/// QAS Pro On Demand > (c) QAS Ltd > www.qas.com
/// Intranet > Rapid Addressing > Standard > RapidPicklist
/// Results frame, displays picklist, handles dynamic seaching/refinement


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
	/// Intranet > Rapid Addressing > Standard > Full QuickAddress searching with hierarchical picklists
	/// Perform dynamic searching & refinement, updating the picklist in response
	/// 
	/// Main actions:
	///   - (None): included directly in main parent page (Javascript picks up the picklist from parent)
	///   - Dynamic search/refinement: caused by JavaScript in main parent page
	/// 
	/// This page is based on RapidBasePage, which provides functionality common to the scenario
	/// </summary>
	public partial class RapidPicklist : RapidBasePage
	{
		/** Page members **/


		// Current picklist, to display
		protected Picklist m_Picklist = null;


		/** Methods **/


		/// <summary>
		/// Perform initial dynamic (Typedown) search
		/// </summary>
		protected void InitialDynamicSearch()
		{
			try
			{
				theQuickAddress.Engine = QuickAddress.EngineTypes.Typedown;
				m_Picklist = theQuickAddress.Search(DataID, SearchString, PromptSet.Types.Default).Picklist;
			}
			catch (Exception x)
			{
				GoErrorPage(x);
			}
			// Display results picklist: done by page
		}

		/// <summary>
		/// Perform refinement search on Moniker and SearchString
		/// </summary>
		protected void RefinementSearch()
		{
			try
			{
				m_Picklist = theQuickAddress.Refine(Moniker, SearchString);
			}
			catch (Exception x)
			{
				GoErrorPage(x);
			}

			// Display results picklist: done by page
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
		/// Update event: perform initial or refinement search
		/// </summary>
		protected void ActionUpdate_Click(object sender, System.EventArgs e)
		{
			if (Moniker == "")
			{
				InitialDynamicSearch();
			}
			else
			{
				RefinementSearch();
			}
		}


		/** Page controls **/


		/// Country data identifier (i.e. AUS)
		private string DataID
		{
			get
			{
				return HiddenDataID.Value;
			}
		}

		/// Location of picklist data; determined by whether we are being directly or indirectly requested
		protected string DataSource
		{
			get
			{
				// Direct: by browser javascript - indirect: just due to parent page reference
				return DataID == "" ? "parent." : "";
			}
		}

		/// Picklist depth - visual hint (set by browser Javascript)
		protected string HistoryDepth
		{
			get
			{
				return HiddenHistoryDepth.Value;
			}
		}

		/// Moniker of the picklist item selected (set by browser Javascript)
		private string Moniker
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

		/// Text to search/refine on
		protected string SearchString
		{
			get
			{
				return HiddenSearchText.Value;
			}
		}
	}
}
