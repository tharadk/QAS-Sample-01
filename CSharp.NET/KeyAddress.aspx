<%@ Page language="c#" Inherits="com.qas.prowebintegration.KeyAddress" enableViewState="False" CodeFile="KeyAddress.aspx.cs" %>
<%@ Import namespace="com.qas.prowebintegration" %>
<%@ Import namespace="com.qas.proweb" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<HTML>
	<HEAD>
		<title>QuickAddress Pro Web - Key Search</title>
		<link href="style.css" type="text/css" rel="stylesheet">
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<table width="800" class="nomargin">
				<tr>
					<th class="banner">
						<a href="index.htm"><h1>QuickAddress Pro Web</h1><h2>C# .NET Samples</h2></a>
					</th>
				</tr>
				<tr>
					<td class="nomargin">
						<h1>Key Search</h1>
						<h3>Confirm The Address</h3>
						<P><asp:Literal id="LiteralMessage" runat="server" EnableViewState="False"></asp:Literal></P>

						<table>
<%	for (int i=0; i < m_asAddressLines.Length; i++)
	{
%>							<tr>
								<td><%= m_asAddressLabels[i] %></td>
								<td width="10"></td>
								<td>
									<input type="text" name="<%= Constants.FIELD_ADDRESS_LINES %>" size="50" value="<%= HttpUtility.HtmlEncode(m_asAddressLines[i]) %>" />
								</td>
							</tr>
<%	}
%>							<tr>
								<td>Country or Datamap</td>
								<td width="10"></td>
								<td>
									<input type="text" name="<%= Constants.FIELD_COUNTRY_NAME %>" size="50" value="<%= GetCountryName() %>" readonly class="readonly" tabindex="-1" >
								</td>
							</tr>
<%	if (!GetRoute().Equals(Constants.Routes.Okay))
	{
%>							<tr class="debug">
								<td colspan="3">
									<p class="debug"><img src="img/debug.gif" align="left">&nbsp;Integrator information: search result was <%= GetRoute().ToString() %>
<%	if (GetErrorInfo() != null)
	{
		Response.Write("<br />&nbsp;" + GetErrorInfo().Replace("\n", "<br />"));
	}
%>
									</p>
								</td>
							</tr>
<%	}
%>						</table>
						<br>
						This completes the key search process.
						<br /><br />
<%	// carry through values from earlier pages
	RenderRequestString(Constants.FIELD_DATA_ID);
	RenderRequestArray(Constants.FIELD_INPUT_LINES);
	RenderRequestString(FIELD_PROMPTSET);
	RenderRequestString(FIELD_PICKLIST_MONIKER);
	RenderRequestString(FIELD_REFINE_MONIKER);
	RenderRequestString(Constants.FIELD_MONIKER);
	RenderHiddenField(Constants.FIELD_ROUTE, GetRoute().ToString());
%>						<input type="button" value="   < Back   " id="ButtonBack" runat="server" onserverclick="ButtonBack_ServerClick">
						<input type="submit" value="   Accept   " id="ButtonAccept" runat="server" onserverclick="ButtonAccept_ServerClick">

						<br /><br /><br /><hr /><br />
						<a href="index.htm">Click here to return to the C# .NET Samples home page.</a>
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>

