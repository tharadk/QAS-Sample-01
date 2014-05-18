<%@ Page language="c#" AutoEventWireup="false" contentType="text/html" enableViewState="False"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<HTML>
	<HEAD>
		<title>QAS Pro On Demand - C# .NET Samples</title>
<%
	string[] asAddress = Request.Params.GetValues(com.qas.prowebintegration.Constants.FIELD_ADDRESS_LINES);
	if (asAddress == null)
	{
		asAddress = new string[] { "Error: no address lines passed" };
	}
	string sCountryName = Request[com.qas.prowebintegration.Constants.FIELD_COUNTRY_NAME];
	if (sCountryName == null)
	{
		sCountryName = "";
	}
	string sErrorInfo = Request[com.qas.prowebintegration.Constants.FIELD_ERROR_INFO];
	string sAddressInfo = Request[com.qas.prowebintegration.Constants.FIELD_ADDRESS_INFO];
    string sDPVStatus = Request[com.qas.prowebintegration.Constants.FIELD_DPVSTATUS];
%>
		<link href="style.css" type="text/css" rel="stylesheet">
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	</HEAD>
	<body>
		<table width="800" class="nomargin">
			<tr>
				<th class="banner">
					<a href="index.htm"><h1>QAS Pro On Demand</h1><h2>C# .NET Samples</h2></a>
				</th>
			</tr>
			<tr>
				<td class="nomargin">
					<h1>Final Captured Address</h1>

					<p>
						<table>
							<tr>
								<td>
								    <h5>
<%	for (int i=0; i < asAddress.Length; i++)
	{
                                        string sLine = HttpUtility.HtmlEncode(asAddress[i]).Replace(" ", "&nbsp;");
%>
                                        <%= sLine %><br />
<%	}
%>										<%= HttpUtility.HtmlEncode(sCountryName) %>
								    </h5>
								</td>
							</tr>
<%	if (sErrorInfo != null || sAddressInfo != null || sDPVStatus != null)
	{
%>							<tr class="debug">
								<td class="debug">
									<img src="img/debug.gif" align="left">&nbsp;Integrator information:
<%		if (sAddressInfo != null)
		{
			Response.Write(HttpUtility.HtmlEncode(sAddressInfo)+ "<br />");
		}
		if (sAddressInfo != null && sErrorInfo != null)
		{
			Response.Write("<br />");
		}
		if (sErrorInfo != null)
		{
			Response.Write("&nbsp;" + HttpUtility.HtmlEncode(sErrorInfo).Replace("\n", "<br />"));
		}
        if (sDPVStatus != null)
        {
            Response.Write("&nbsp;" + HttpUtility.HtmlEncode(sDPVStatus).Replace("\n", "<br />"));
        }
%>
								</td>
							</tr>
<%	}
%>						</table>
					</p>
					<p>&nbsp;</p>
					<p><hr /></p>
					<p>This is the end of this sample site.<br />
						<a href="index.htm">Click here to return to the C# .NET Samples home page.</a></p>
				</td>
			</tr>
		</table>
	</body>
</HTML>
