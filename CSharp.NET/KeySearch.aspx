<%@ Page language="c#" Inherits="com.qas.prowebintegration.KeySearch" enableViewState="False" CodeFile="KeySearch.aspx.cs" %>
<%@ Import namespace="com.qas.prowebintegration" %>
<%@ Import namespace="com.qas.proweb" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<HTML>
	<HEAD>
		<title>QuickAddress Pro Web - Key Search</title>
		<link href="style.css" type="text/css" rel="stylesheet">
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<script type="text/javascript">
			/* Hyperlink clicked: select the appropriate radio button, click the Next button */
			function submitCommand(iIndex)
			{
				document.getElementsByName("<%= Constants.FIELD_MONIKER %>")[iIndex].checked = true;
				document.frmFlatSearch.<%= Constants.FIELD_MONIKER %>.selectedIndex = iIndex;
				document.frmFlatSearch.ButtonNext.click();
			}

			/* Next button clicked: ensure radio button selected, and pick up private data */
			function submitForm()
			{
				var aItems = document.getElementsByName("<%= Constants.FIELD_MONIKER %>");
				for (var i=0; i < aItems.length; i++)
				{
					if (aItems[i].checked == true)
					{
						document.frmFlatSearch.<%= FIELD_MUST_REFINE %>.value = document.PrivateData.getElementsByTagName("input")[i].value;
						return true;
					}
				}
				alert ("Please select an address first.");
				return false;
			}
		</script>
	</HEAD>
	<body>
		<form id="frmFlatSearch" method="post" runat="server">
			<table width="800" class="nomargin">
				<tr>
					<th class="banner">
						<a href="index.htm"><h1>QuickAddress Pro Web</h1><h2>C# .NET Samples</h2></a>
					</th>
				</tr>
				<tr>
					<td class="nomargin">
						<h1>Key Search</h1>
						<h3>Select The Address</h3>
						Select one of the following addresses that matched your search.<br /><br />

						<table>
							<%
							PicklistItem[] atItems = m_Picklist.Items;
							for (int i=0; i < atItems.Length; i++)
							{%>
							<tr>
								<td>
									<input type="radio" name="<%= Constants.FIELD_MONIKER %>" value="<%= atItems[i].Moniker %>" ><a
										href="javascript:submitCommand('<%= i %>');"><%= HttpUtility.HtmlEncode(atItems[i].Text) %></a>
								</td>
								<td width="15"></td>
								<td>
									<%= atItems[i].Postcode %>
								</td>
							</tr>
							<%
							}%>
							<tr>
							    <td colspan="3">&nbsp;</td>
							</tr>
							<tr>
							    <td colspan="3">&nbsp;</td>
							</tr>
							<tr>
							    <td>Request Tag
							    </td>
								<td width="10"></td>
							    <td>
							        <asp:TextBox ID="RequestTag" runat="server" />&nbsp;&nbsp;<i>(optional)</i>
							    </td>
							</tr>
						</table>
						<br>
						The next step is to confirm the address.
						<br>
						<br>
						<%
						// carry through values from earlier pages
						RenderRequestString(Constants.FIELD_DATA_ID);
						RenderRequestString(Constants.FIELD_COUNTRY_NAME);
						RenderRequestArray(Constants.FIELD_INPUT_LINES);
						RenderRequestString(FIELD_PROMPTSET);
						RenderRequestString(FIELD_PICKLIST_MONIKER);
						// hidden field to be populated by client JavaScript, picked out from form PrivateData
						RenderHiddenField(FIELD_MUST_REFINE, null);
						%>
						<input type="button" value="   < Back   " id="ButtonBack" runat="server" onserverclick="ButtonBack_ServerClick">
						<input type="submit" value="    Next >   " id="ButtonNext" runat="server" onclick="return submitForm();" onserverclick="ButtonNext_ServerClick">
						<br>
						<br>
						<br>
						<hr>
						<br>
						<A href="index.htm">Click here to return to the C# .NET Samples home page.</A>
					</TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>

