<%@ Page language="c#" Inherits="com.qas.prowebintegration.HierInput" enableViewState="False" CodeFile="HierInput.aspx.cs" %>
<%@ Import namespace="com.qas.prowebintegration" %>
<%@ Import namespace="com.qas.proweb" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>QAS Pro On Demand - Rapid Addressing - Single Line</title>
		<LINK href="style.css" type="text/css" rel="stylesheet">
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script type="text/javascript">
			// Set the focus to the country dropdown
			function onLoadPage()
			{
				document.frmHierInput.country.focus();
			}
			
			// Validate the country and search term fields before submission
			function validate(theForm)
			{
				// Validate country selection
				var iCountrySelection = theForm.country.selectedIndex;
				if (!iCountrySelection || !theForm.country.options[iCountrySelection].value)
				{
					theForm.country.focus();
					alert ("Please select a country or datamap.");
					return false;
				}

				// Validate search term
				var sUserInput = theForm.TextBoxSearch.value;
				if (!sUserInput)
				{
					theForm.TextBoxSearch.focus();
					alert ('Please enter a search.');
					return false;
				}

				return true;
			}
		</script>
		<style>
		    .heading
		    {
		        background-color:#c5e6f0;
		    }
		</style>
	</HEAD>
	<body onload="onLoadPage();">
		<form id="frmHierInput" method="post" runat="server" onsubmit="return validate(this);">
			<table width="800" class="nomargin">
				<tr>
					<th class="banner">
						<a href="index.htm"><h1>QAS Pro On Demand</h1><h2>C# .NET Samples</h2></a>
					</th>
				</tr>
				<tr>
					<td class="nomargin">
						<h1>Rapid Addressing &#8211; Single Line</h1>
						<h3>Enter Your Search</h3>
						<p>Please select a country or datamap from the list below, and enter your search terms.</p>
						<p>
							<table>
								<tr>
									<td><label for="country">Country or Datamap</label></td>
									<td width="10"></td>
									<td><asp:dropdownlist id="country" runat="server"></asp:dropdownlist></td>
								</tr>
								<tr>
									<td><label for="TextBoxSearch">Search</label></td>
									<td></td>
									<td><asp:textbox id="TextBoxSearch" runat="server" Width="20em" EnableViewState="False"></asp:textbox>&nbsp;
										<asp:button id="ButtonSearch" runat="server" Text="Search" EnableViewState="False" onclick="ButtonSearch_Click"></asp:button></td>
								</tr>
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
						</p>
						<p>The next step is to drill-down to the right address.</p>
						<p class="debug">
						    <%
                                if (m_asError != null)
                                {
                                    %><%="<img src='img/debug.gif' align='left'>The On Demand server is not available</br>" + m_asError %><%
                                }
                                else
                                {
                                    %><%="&nbsp" %><%
                                }
						    %>
						</p>
						<p>
							<hr width="100%" SIZE="1">
						<P></P>
						<p></p>
						<p><a href="index.htm">Click here to return to the C# .NET Samples home page.</a></p>
					</TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
