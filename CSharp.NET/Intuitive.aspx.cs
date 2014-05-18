using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using com.qas.proweb;

namespace com.qas.prowebintegration
{
    public partial class Intuitive : System.Web.UI.Page
    {
        // Error string
        protected string m_asError;

        // Dataset array for dropdown box
        protected Dataset[] m_atDatasets;

        // Search result (contains either picklist or matched address or nothing)
        protected SearchResult m_SearchResult = null;

        /// <summary>
        /// Page load sets up Javascript event handlers for input elements and deals with postback events
        /// from these functions.
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            InputBox.Attributes.Add("onkeyup", "javascript: InputAddressKeyPressed(event)");

            AddressList.Attributes.Add("onkeypress", "javascript: AddressKeypress(this, event)");
            AddressList.Attributes.Add("onclick",    "javascript: AddressClick(this)");
            AddressList.Attributes.Add("ondblclick", "javascript: AddressDoubleClick(this)");

            if (Request.Params["__EVENTTARGET"] == InputBox.UniqueID && Request.Params["__EVENTARGUMENT"] == "onkeyup")
            {
                InputBox_TextChanged();
            }
            else if (Request.Params["__EVENTTARGET"] == InputBox.UniqueID && Request.Params["__EVENTARGUMENT"] == "onenter")
            {
                SelectButton_Click(null, null);
            }
            else if (Request.Params["__EVENTTARGET"] == InputBox.UniqueID && Request.Params["__EVENTARGUMENT"] == "ondown")
            {
                if (AddressList.Visible)
                {
                    AddressList.Focus();
                    AddressList.SelectedIndex = 0;
                }
            }
            else if (Request.Params["__EVENTTARGET"] == AddressList.UniqueID && Request.Params["__EVENTARGUMENT"] == "onclick")
            {
                InputBox.Focus();
            }
            else
            {
                InputBox.Focus();
            }
        }

        /// <summary>
        /// Looks up a datamapping relating to the selected country
        /// </summary>
        protected string GetLayout()
        {
           string sLayout = "";
           string sDataID = CountryList.SelectedValue;

           if (sDataID != null && sDataID != "")
           {
               // Look for a layout specific to this datamap 
               sLayout = System.Configuration.ConfigurationManager.AppSettings[Constants.KEY_LAYOUT + "." + sDataID];

               if (sLayout == null || sLayout == "")
               {
                   // No layout found specific to this datamap - try the default
                   sLayout = System.Configuration.ConfigurationManager.AppSettings[Constants.KEY_LAYOUT];
               }
           } 
           
           return sLayout;
        }

        /// <summary>
        /// The main search routine which is called when input box text changes. Tests the connection with a
        /// CanSearch operation then if server is available performs Intuitive Search and populates and 
        /// address list with the resulting picklist elements. The moniker is stored upon a successful search
        /// to be used by formatting to replay the search operation.
        /// </summary>
        protected void InputBox_TextChanged()
        {
            Session["useTypedAddress"] = true;
            Session.Contents.Remove("inputMoniker");

            // Using com.qas.proweb
            CanSearch canSearch = null;
            
            ResultBox.Text = "";

            try
            {
                // Create new search connection
                QuickAddress searchService = Global.NewQuickAddress();
                searchService.Engine = QuickAddress.EngineTypes.Intuitive;
                searchService.Flatten = true;

                // Get the layout
                string sLayout = GetLayout();

                // Check that searching with this engine and layout is available
                canSearch = searchService.CanSearch(CountryList.SelectedValue, sLayout);
                
                if (canSearch.IsOk) 
                {
                    // Search on the address
                    m_SearchResult = searchService.Search(CountryList.SelectedValue, 
                                                          InputBox.Text, 
                                                          PromptSet.Types.Default, 
                                                          sLayout, RequestTag.Text);

                    // Clear address list
                    AddressList.Items.Clear();

                    if ( m_SearchResult.Picklist != null &&
                         m_SearchResult.Picklist.Total > 0)
                    {
                        // Store picklist moniker for replaying the search
                        Session["inputMoniker"] = m_SearchResult.Picklist.Moniker;

                        // Too many matches so return prompt to address list
                        if (m_SearchResult.Picklist.IsMaxMatches) 
                        {
                            AddressList.Enabled = false;
                            PicklistItem entry = m_SearchResult.Picklist.Items[0];
                            AddressList.Items.Add(new ListItem("Continue typing (too many matches) ", entry.Moniker));
                        } 
                        else 
                        {
                            // Get pick list entries
                            AddressList.Enabled = true;
                            
                            // Write the results to the address list. Also store the SPMs for use in formatting
                            foreach (PicklistItem entry in m_SearchResult.Picklist.Items)
                            {
                                if (entry.PartialAddress != "")
                                {
                                    AddressList.Items.Add(new ListItem(entry.PartialAddress, entry.Moniker));
                                }
                            }
                        }  
                    }

                    // Address list can grow from 3 to 10 items to aid display of larger picklists
                    if (AddressList.Items.Count >= 1)
                    {
                        int RowsMin = 3;
                        int RowsMax = 10;
                        
                        AddressList.Visible = true;
                        
                        if (AddressList.Items.Count > RowsMin)
                        {
                            if (AddressList.Items.Count < RowsMax)
                            {
                                AddressList.Rows = AddressList.Items.Count;
                            } 
                            else 
                            {
                                AddressList.Rows = RowsMax;
                            }
                        } 
                        else 
                        {
                            AddressList.Rows = RowsMin;
                        }
                    }
                    // Address list should disappear if empty
                    else
                    {
                        AddressList.Visible = false;
                        ResultBox.Text = "";
                    }
                }
                else if (canSearch != null)
                {
                    m_asError = canSearch.ErrorMessage;
                }
            }
            catch (System.Exception e)
            {
               m_asError = e.Message;
            }

            if (m_asError != null)
            {
                UpdatePanelLogging.Update();
            }
        }

        /// <summary>
        /// The main formatting routine which is called when an address is selected and the formatting button 
        /// pressed.
        /// 
        /// There are tree modes of operation:
        ///     1. Address Selected from address list
        ///         In this case the stored moniker of the address element is used to format the 
        ///         pre-classified (cleaned) address.
        ///     2. Formatting button selected with no prior moniker
        ///         Here no prior search moniker has been retrieved from the Intuitive search. We therefore
        ///         need to search first before we can call the Standardise engine. Once a SPM has been 
        ///         obtained it will be used in the call to Standardise.
        ///     3. Formatting button selected with known moniker
        ///         As in #2 but without the requirement for a pre-formatting search.
        /// 
        /// </summary>
        protected void SelectButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Ensure that we don't search on blank strings
                Regex rNonWhitespace = new Regex(@"\S");
                if (!rNonWhitespace.IsMatch(InputBox.Text))
                {
                    return;
                }

                // If an address has been manually typed into the input box, standardise and format it
                // using the Intuitive Search engine
                if (AddressList.SelectedValue == "")
                {
                    QuickAddress searchService = Global.NewQuickAddress();
                    searchService.Engine = QuickAddress.EngineTypes.Intuitive;
                    
                    string sLayout = GetLayout();

                    if ( Session.Contents["inputMoniker"] == null ||
                         Session["inputMoniker"].ToString().Length == 0 )
                    {
                        // Need to do a search first to obtain the moniker
                        searchService.Engine = QuickAddress.EngineTypes.Intuitive;
                        searchService.Flatten = true;

                        // Using com.qas.proweb
                        CanSearch canSearch = searchService.CanSearch(CountryList.SelectedValue, sLayout);

                        if (canSearch.IsOk)
                        {
                            m_SearchResult = searchService.Search(CountryList.SelectedValue,
                                                          InputBox.Text,
                                                          PromptSet.Types.Default,
                                                          sLayout, RequestTag.Text);
                            
                            if (m_SearchResult.Picklist != null)
                            {
                                Session["inputMoniker"] = m_SearchResult.Picklist.Moniker;
                            }
                        }
                    }

                    if (Session["inputMoniker"].ToString().Length != 0)
                    {
                        // Use SPM to format address
                        FormattedAddress canFormat = searchService.GetFormattedAddress(Session["inputMoniker"].ToString(), sLayout, RequestTag.Text);

                        StringBuilder theText = new StringBuilder();

                        if (canFormat.AddressLines != null)
                        {
                            foreach (AddressLine addrline in canFormat.AddressLines)
                            {
                                theText.Append(addrline.Line);
                                theText.Append("\n");
                            
                            }
                        }

                        ResultBox.Text = theText.ToString();
                    } 
                    else 
                    {
                        m_asError = "No input to format - please enter address details";
                    }
                }
                else
                {
                    // If an address has been selected from the picklist, and has not been subsequently edited,
                    // format that instead
                    FormatAddress(AddressList.SelectedValue);
                }
            }
            catch (System.Exception ex)
            {
                m_asError = ex.Message;
            }

            if (m_asError != null)
            {
                UpdatePanelLogging.Update();
            }
        }

        /// <summary>
        /// The onclick event for the Clear button. This is used to clear the search box and any result
        /// fields and reset the input moniker for formatting.
        /// </summary>
        protected void ClearButton_Click(object sender, EventArgs e)
        {
            InputBox.Text = "";
            InputBox.Focus();
            AddressList.Items.Clear();
            AddressList.Visible = false;
            ResultBox.Text = "";

            Session["useTypedAddress"] = false;
            Session.Contents.Remove("inputMoniker");
        }

        /// <summary>
        /// Formats the selected address from the address list, by passing the SPM to the Pro Web server
        /// </summary>
        protected void FormatAddress(String sMoniker)
        {
            try
            {
                // Format the address
                QuickAddress searchService = Global.NewQuickAddress();
                //GetLayout()
                FormattedAddress tAddressResult = searchService.GetFormattedAddress(sMoniker, "MedibankCRM", RequestTag.Text);

                StringBuilder theText = new StringBuilder();

                if (tAddressResult.AddressLines != null)
                {
                    foreach (AddressLine addrline in tAddressResult.AddressLines)
                    {
                        theText.Append(addrline.Line);
                        theText.Append("\n");
                    }
                }

                ResultBox.Text = theText.ToString();
                Session["useTypedAddress"] = false;
                
            }
            catch (Exception x)
            {
                m_asError = x.Message;
            }
        }


        /// <summary>
        /// Set up country list by querying Pro Web server for list of available countries
        /// </summary>
        protected void PopulateDatamaps(object sender, EventArgs e)
        {
            if (m_atDatasets == null)
            {
                // Retrieve a list of all datasets ( maps ) available on the server
                try
                {
                    QuickAddress qas = Global.NewQuickAddress();
                    m_atDatasets = qas.GetAllDatasets();
                }
                catch (Exception x)
                {
                    m_asError = x.Message;
                }
            }

            // Populate drop down list of countries
            if (m_atDatasets != null)
            {
                ListItem itemheader1 = new ListItem("-- Datamaps available --", "");
                itemheader1.Attributes["class"] = "heading";
                CountryList.Items.Add(itemheader1);

                Array.Sort(m_atDatasets);

                foreach (Dataset dset in m_atDatasets)
                {
                    ListItem litem = new ListItem(dset.Name, dset.ID);
                    CountryList.Items.Add(litem);
                }

                ListItem itemheader2 = new ListItem("-- Other --", "");
                itemheader2.Attributes["class"] = "heading";
                CountryList.Items.Add(itemheader2);
            }

            foreach (Dataset dset in Constants.gatAllCountries)
            {
                bool bDuplicate = false;

                if (m_atDatasets != null)
                {
                    foreach (Dataset serverDset in m_atDatasets)
                    {
                        if (serverDset.Name == dset.Name || serverDset.ID == dset.ID)
                        {
                            bDuplicate = true;
                            break;
                        }
                    }
                }

                if (!bDuplicate)
                {
                    ListItem litem = new ListItem(dset.Name, dset.ID);
                    CountryList.Items.Add(litem);
                }
            }
        }
    }
}

