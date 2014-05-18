<%@ Page language="c#" Inherits="com.qas.prowebintegration.HierAddress" contentType="text/html" CodeFile="HierAddress.aspx.cs" %>
<%@ Import namespace="com.qas.prowebintegration" %>
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
			// Set the focus to the first address line
			function onLoadPage()
			{
				document.frmHierAddress.<%= Constants.FIELD_ADDRESS_LINES %>[0].focus();
			}
			
			// Fire the event for Back button clicked
			function goBack()
			{
				<%= GetPostBackEventReference(ButtonBack) %>;
			}
		</script>
	</HEAD>
	<body onload="onLoadPage();">
		<form id="frmHierAddress" method="post" runat="server">
			<table width="800" class="nomargin">
				<tr>
					<th class="banner">
						<a href="index.htm"><h1>QAS Pro On Demand</h1><h2>C# .NET Samples</h2></a>
					</th>
				</tr>
				<tr>
					<td class="nomargin">
						<h1>Rapid Addressing &#8211; Single Line</h1>
						<h3>Confirm Final Address</h3>
						<P><asp:literal id="LiteralMessage" runat="server" EnableViewState="False" Text="Please confirm your address below."></asp:literal></P>
						<asp:placeholder id="PlaceHolderWarning" runat="server" EnableViewState="False" Visible="False">
							<P><IMG class="icon" src="img/warning.gif"> <B>
									<asp:Literal id="LiteralWarning" runat="server" EnableViewState="False"></asp:Literal></B></P>
						</asp:placeholder>
						<P></P>
						<P><asp:table id="TableAddress" runat="server" EnableViewState="False"></asp:table></P>
						<P>This completes the address capture process.</P>
						<P><font color="red"><asp:Literal id="DPVMessage" runat="server" EnableViewState="False"></asp:Literal></font></P>
<% RenderRequestString(Constants.FIELD_DPVSTATUS);
%>						
						<asp:placeholder id="PlaceholderInfo" runat="server" EnableViewState="False" Visible="False">
							<P class="debug"><IMG src="img/debug.gif" align="left"> Integrator information: search result was
								<asp:Literal id="LiteralRoute" runat="server" EnableViewState="False"></asp:Literal>
								<asp:Literal id="LiteralError" runat="server" EnableViewState="False"></asp:Literal></P>
						</asp:placeholder>
						<P><INPUT id="ButtonNew" accessKey="N" type="button" value="   New   " runat="server" onserverclick="ButtonNew_ServerClick">
							<INPUT id="ButtonBack" accessKey="B" type="button" value="  Back  " runat="server" onserverclick="ButtonBack_ServerClick">
							<asp:button id="ButtonAccept" runat="server" EnableViewState="False" Text=" Accept " onclick="ButtonAccept_Click"></asp:button></P>
						<P>&nbsp;</P>
						<P>
							<HR width="100%" SIZE="1">
						<P></P>
						<P><A href="index.htm">Click here to return to the C# .NET Samples home page.</A></P>
					</TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
