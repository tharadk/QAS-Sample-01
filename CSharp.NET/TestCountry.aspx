<%@ Page language="c#" AutoEventWireup="false" %>
<%@ Import namespace="com.qas.proweb" %>
<%@ Import namespace="com.qas.prowebintegration" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<HTML>
	<HEAD>
		<title>QAS Pro On Demand - Country Diagnostics</title>
		<link rel="stylesheet" href="style.css" type="text/css">
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
<%
	// Retrieve server URL from web.config
	string sServerURL = ConfigurationSettings.AppSettings["com.qas.proweb.serverURL"];

	// Instance of the Pro Web service
	QuickAddress searchService = null;

	// Retrieve DataID parameter (set by test overview page): the Dataset to display
	string sDataId = Request.QueryString["DataID"];
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
				<td class="nomargin">
					<h1>Country Diagnostics</h1>
					<h2>Dataset: <%= sDataId %></h2>
					<h3>Prompt Sets</h3>

					<table class="indent" border="1" cellspacing="1">
						<tr>
<%	try
	{
        searchService = Global.NewQuickAddress();
		
		// Array of search prompts to display
		PromptSet.Types[] aPromptTypes = new PromptSet.Types[]
		{
			PromptSet.Types.Optimal,
			PromptSet.Types.Alternate,
			PromptSet.Types.Generic
		};
		for (int i=0; i < aPromptTypes.GetLength(0); i++)
		{
%>							<th><%= aPromptTypes[i].ToString() %></th>
<%		}
%>						</tr>
						<tr>
<%		for (int i=0; i < aPromptTypes.GetLength(0); i++)
		{
			PromptSet tPrompt = searchService.GetPromptSet(sDataId, aPromptTypes[i]);
			if (tPrompt == null || tPrompt.Lines == null)
			{
				Response.Write("\n<td>&nbsp;</td>");
			}
			else
			{
%>							<td>
								<table cellpadding="2">
<%				PromptLine[] aLines = tPrompt.Lines;
				for (int j = 0; j < aLines.GetLength(0); j++)
				{
%>									<tr>
										<td><%= aLines[j].Prompt %></td>
<%					if (!aLines[j].Example.Equals(""))
					{
						Response.Write("<td>(e.g. " + aLines[j].Example + ")</td>");
					}
%>									</tr>
<%				}
%>								</table>
							</td>
<%			}
		}
%>						</tr>
					</table>

					<h3>Layouts</h3>
					<table class="indent" border="1" cellspacing="1">
						<tr>
<%		Layout[] aLayouts = searchService.GetAllLayouts(sDataId);
		for (int i = 0; i < aLayouts.GetLength(0); i++)
		{
%>							<th><%= HttpUtility.HtmlEncode(aLayouts[i].Name) %></th>
<%		}
%>						</tr>
						<tr>
<%		for (int i=0; i < aLayouts.GetLength(0); i++)
		{
			ExampleAddress[] aExamples = searchService.GetExampleAddresses(sDataId, aLayouts[i].Name);
			// Take the labels from the first address
			AddressLine[] aAddLines = aExamples[0].AddressLines;
%>							<td>
								<table>
<%			for (int j = 0; j < aAddLines.GetLength(0); j++)
			{
				string sLine = aAddLines[j].Label;
%>									<tr>
										<td><%= sLine.Equals("") ? "<i>No&nbsp;label</i>" : sLine %></td>
									</tr>
<%			}
%>								</table>
							</td>
<%		}
%>						</tr>
					</table>
<%	}
	catch (Exception x)
	{
%>						</tr>
					</table>
					
					
                    <h3 class="error">Error:</h3>
					<p class="error"><%=x.Message %></p>
					<h3 class="error">Trace:</h3>
					<p class="error"><%=x.StackTrace.Replace(System.Environment.NewLine, "<br/>") %></p>
					
		    <% if (x.InnerException != null)
               { %>
					<h3 class="error">Source:</h3>
					<p class="error"><%=x.InnerException.ToString().Replace( System.Environment.NewLine, "<br/>") %></p>
			<% } %>
<%	}
%>
					<br /><br /><hr /><br />
					<a href="test.aspx">Click here to return to the Diagnostics Overview page.</a>
					<br /><br />
					<a href="index.htm">Click here to return to the C# .NET Samples home page.</a>
				</td>
			</tr>
		</table>
	</body>
</html>
