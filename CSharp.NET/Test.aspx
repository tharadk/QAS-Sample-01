<%@ Page language="c#" AutoEventWireup="false" %>
<%@ Import namespace="com.qas.proweb" %>
<%@ Import namespace="com.qas.prowebintegration" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<HTML>
	<HEAD>
		<title>QAS Pro On Demand - Diagnostics Overview</title>
		<link rel="stylesheet" href="style.css" type="text/css">
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		
		<script lang="Javascript">
		
		function Toggle(elem)
		{
		    // 'elem' refers to the element containing the + / - graphic
		    // Find the table row element to show
		    
		    var sToggleID = "detail" + elem.id.replace("toggle", "");
		    var elemDetail = document.getElementById(sToggleID);
		   
		    if ( elemDetail.className == "contracted" )
		    {
		        elemDetail.className = "expanded";
		        elem.className = "expanded";
		    }
		    else
		    {
		        elemDetail.className = "contracted";
		        elem.className = "contracted";
		    }
		}
		
		function toggleTab( sElemName )
		{
		    
		    if ( sElemName == "datainfo" )
		    {
		        document.getElementById("datainfo").className = "showtab";
		        document.getElementById("sysinfo").className = "hidetab";
		        document.getElementById("datainfotab").className = "showtab";
		        document.getElementById("sysinfotab").className = "hidetab";
		    }
		    else
		    {
		        document.getElementById("sysinfo").className = "showtab";
		        document.getElementById("datainfo").className = "hidetab";
		        document.getElementById("datainfotab").className = "hidetab";
		        document.getElementById("sysinfotab").className = "showtab";
		    }
		}
		
		</script>
		
		<style>
		
		table.data
		{
		    text-align:left;
		    width: 100%; 
		}
		    
		
		table.detail
		{
		    margin: 0;
		}
		
		table.data td, table.data th
		{
		    padding-bottom: 5px;
		    padding-top: 5px;
		    padding-right: 15px;
		    font-size: 120%;
		    white-space: nowrap;
		}
		
		tr.base td
		{
		    font-weight: bold;
		}
		
		table.detail td, table.detail th
		{
		    font-size: 80%;
		    white-space: normal;
		}
		
		tr.contracted
		{
		    display:none;
		    border:solid 0 black;
		}
		
		tr.expanded
		{
		    display:table-row;
		    border:solid 0 black;
		}
		
		td.contracted, td.expanded
		{		    
		    background-repeat:no-repeat;
		    background-position:center center;
		    cursor:pointer;
		    padding-right: 15px;
		}
		
		td.contracted
		{
		    background-image:url(img/contracted.gif);
		}
		
		td.expanded
		{
		    background-image:url(img/expanded.gif);
		}
		
		table.data th, table.detail th
		{
		    color: #0a246a;
		}
		
		a.showtab, a.hidetab
		{
		    font-size: 140%;
		    padding: 5px;
		}
		
		td.showtab
		{
		    display: table-cell;
		}
		
		td.hidetab
		{
		    display: none;
		}
		
		a.showtab, a.hidetab { border: 0; cursor: pointer; }
		a.showtab:link, a.hidetab:link { text-decoration: none; }
        a.showtab:visited, a.hidetab:visited { text-decoration: none; }
        a.showtab:active, a.hidetab:active { text-decoration: none; }
        a.showtab:hover, a.hidetab:hover { text-decoration: underline; color: #0a246a; }
		
		a.showtab
		{
		    background-color: #c8e3ee;
		}
		
		</style>
		
<%
	// Retrieve server URL from web.config
	string sServerURL = ConfigurationSettings.AppSettings["com.qas.proweb.serverURL"];
	
	// Instance of the Pro Web service
	QuickAddress searchService = null;
%>
	</HEAD>
	<body>
		<table width=800 class="nomargin">
			<tr>
				<th class="banner">
					<a href="index.htm"><h1>QAS Pro On Demand</h1><h2>C# .NET Samples</h2></a>
				</th>
			</tr>
			<tr>
			    <td>
			        <h1>Diagnostics Overview</h1>
			        <br />
				        <a id="datainfotab" class="showtab" href="javascript:toggleTab('datainfo');"><b>Data Information</b></a>
				        &nbsp;&nbsp;
				        <a id="sysinfotab" class="hidetab" href="javascript:toggleTab('sysinfo');"><b>System Information</b></a>
				    <hr />
				</td>
			</tr>
			<tr>
				<td class="showtab" id="datainfo">
					
					
<%	try
	{
        searchService = Global.NewQuickAddress();
		Dataset[] aDatasets = searchService.GetAllDatasets();

		if (aDatasets == null)
		{
%>					<p>No datasets available.</p>
<%		}
		else
		{
%>					<br />
                    <ul>
                        <li>Click on the datamap link for layout and search prompt information.</li>
                        <li>Expand a datamap to view the datasets it contains.</li>
                    </ul>
					<table class="indent data">
						<tr>
						    <th width="18px"></th>
							<th>Datamap ID</th>
							<th>Datamap Name</th>
							<th>Status</th>
						</tr>
<%			for (int i = 0; i < aDatasets.Length; i++)
			{
                Dataset tDatamap = aDatasets[i];
                LicensedSet[] atDatamapDetail = searchService.GetDataMapDetail(aDatasets[i].ID);

                string sDatamapName = tDatamap.Name;
                string sTitle = "";

                if (sDatamapName.Length > 50)
                {
                    sDatamapName = sDatamapName.Substring(0, 50);
                    sTitle = tDatamap.Name;
                }
                
				// Build hyperlink to the country details page
                string sLink = "<a href=\"TestCountry.aspx?DataId=" + tDatamap.ID + "\"";
                if (sTitle.Length > 0) { sLink += " title='" + sTitle + "'"; } 
                sLink += " >";
				string sID = sLink + "<b>" + tDatamap.ID + "</b></a>";
				string sName = sLink + "<b>" + sDatamapName + "</b></a>";
				
				// Build status, if there are any warnings
                // The overall warning level for a datamap is the highest warning level of the datasets it contains
                string sStatus = "";
                LicensedSet.WarningLevels tWarningLevel = LicensedSet.WarningLevels.None;
                
                foreach (LicensedSet lset in atDatamapDetail)
                {
                    if (lset.WarningLevel > tWarningLevel)
                    {
                        tWarningLevel = lset.WarningLevel;
                    }
                }

                if (tWarningLevel != LicensedSet.WarningLevels.None)
                {
                    sStatus = tWarningLevel.ToString();
                }
                else
                {
                    sStatus = "OK";
                }

                string sRowClass = "odd";

                if (i % 2 == 0)
                {
                    sRowClass = "even";
                }

%>						<tr class="<%= sRowClass %>">
							<td class="contracted" onclick="Toggle(this)" id="toggle<%=tDatamap.ID%>"></td>
							<td valign=middle><%= sID %></td>
							<td valign=middle><%= sName %></td>
							<td valign=middle><%= sStatus %></td>
						</tr>
<%			
                // Build the inner 'Datamap details' row ( hidden initially )
%>
                        <tr class="contracted" id="detail<%=tDatamap.ID%>">
                            <td border="0"></td>
                            <td colspan=3>
                                <table class="detail" border="1" cellspacing="1" cellpadding="4">
                                    <tr>
                                        <th>Dataset&nbsp;ID</th>
                                        <th>Name</th>
                                        <th>Version</th>
                                        <th>Copyright</th>
                                        <th>Days&nbsp;Left</th>
                                    </tr>
<%
                string sRowDetailClass = "even";
                
                foreach (LicensedSet lset in atDatamapDetail)
                {
                    string sSetID = lset.ID;
                    string sDetailClass = (lset.ID == lset.BaseCountry) ? "base" : "";
                
%>
                                    <tr class="<%=sDetailClass %>">
                                        <td><%= sSetID %></td>
                                        <td><%= lset.Description %></td>
                                        <td><%= lset.Version %></td>
                                        <td><%= lset.Copyright %></td>
                                        <td><%= lset.DaysLeft %></td>
                                    </tr>                  
<%                   
                }
%>
                    
                            </table>
                        </td>
                    </tr>
<%                             
            }
                        
%>					</table>
<%		}
	}
	catch (Exception x)
	{
%>
                    <h3 class="error">Error:</h3>
					<p class="error"><%=x.Message %></p>
					<h3 class="error">Trace:</h3>
					<p class="error"><%=x.StackTrace.Replace(System.Environment.NewLine, "<br/>") %></p>
					
		    <% if (x.InnerException != null)
               { %>
					<h3 class="error">Source:</h3>
					<p class="error"><%=x.InnerException.ToString().Replace(System.Environment.NewLine, "<br/>") %></p>
			<% } %>
					<br/>
<%	}

%>					
            </td>
        </tr>
        <tr>
            <td class="hidetab" id="sysinfo">
            <br />
<%
	try
	{
		string[] asSysInfo = (searchService != null) ? searchService.GetSystemInfo() : null;
		if (asSysInfo == null)
		{
%>					<p>No information available.</p>
<%		}
		else
		{
%>					<table border="1" cellspacing="1" cellpadding="4" class="indent">
<%			for (int i = 0; i < asSysInfo.GetLength(0); i++)
			{
				// Split each string in two (key:value) on the tab character
				int iTabPos = asSysInfo[i].IndexOf('\t');
				string sKey = (iTabPos > 0) ? HttpUtility.HtmlEncode(asSysInfo[i].Substring(0, iTabPos)) : "&nbsp;";
				string sValue = asSysInfo[i].Substring(iTabPos + 1);
				sValue = (!sValue.Equals("")) ? HttpUtility.HtmlEncode(sValue) : "&nbsp;";
%>						<tr>
							<td><%= sKey %></td>
							<td><%= sValue %></td>
						</tr>
<%			}
%>					</table>
<%		}
	}
	catch (Exception x)
	{
%>                  <h3 class="error">Error:</h3>
  					<p class="error"><%=x.Message %></p>
					<h3 class="error">Trace:</h4>
					<p class="error"><%=x.StackTrace %></p>
					
		    <% if (x.InnerException != null)
               { %>
					<h3 class="error">Source:</h3>
					<p class="error"><%=x.InnerException.ToString()%></p>
			<% } %>
<%	}
%>					
				</td>
			</tr>
			<tr>
			    <td>
			        <br /><br /><hr /><br />
					<a href="index.htm">Click here to return to the C# .NET Samples home page.</a>
			    </td>
			</tr>
		</table>
	</body>
</HTML>
