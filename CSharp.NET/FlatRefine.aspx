<%@ Page language="c#" Inherits="com.qas.prowebintegration.FlatRefine" enableViewState="False" CodeFile="FlatRefine.aspx.cs" %>
<%@ Import namespace="com.qas.prowebintegration" %>
<%@ Import namespace="com.qas.proweb" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<HTML>
  <HEAD>
		<title>QAS Pro On Demand - Address Capture</title>
		<link href="style.css" type="text/css" rel="stylesheet">
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<script type="text/javascript">
			/* Set the focus */
			function init()
			{
				document.frmFlatRefine.TextRefinement.focus();
				document.frmFlatRefine.TextRefinement.select();
			}

			/* Ensure something has been entered */
			function validate()
			{
				if (document.frmFlatRefine.TextRefinement.value != "")
				{
					return true;
				}
				document.frmFlatRefine.TextRefinement.focus();
				alert("Please enter exact details.");
				return false;
			}
		</script>
</HEAD>
	<body onload="init();">
		<form id="frmFlatRefine" method="post" runat="server" autocomplete="off">
			<table width="800" class="nomargin">
				<tr>
					<th class="banner">
						<a href="index.htm"><h1>QAS Pro On Demand</h1><h2>C# .NET Samples</h2></a>
					</th>
				</tr>
				<tr>
					<td class="nomargin">
						<h1>Address Capture</h1>
						<h3>Enter Your Address</h3>
						<P><asp:Literal id="LiteralMessage" runat="server" EnableViewState="False"></asp:Literal></P>

						<table>
							<tr>
								<td>Address</td>
								<td width="10"></td>
								<td><input type="text" size="10" id="TextRefinement" runat="server"></td>
								<td>&nbsp;&nbsp;in&nbsp;</td>
								<td><asp:Literal id="LiteralRefineLine" runat="server" EnableViewState="False"></asp:Literal></td>
							</tr>
							<tr>
								<td></td>
								<td></td>
								<td></td>
								<td></td>
								<td><asp:Literal id="LiteralRefineAddress" runat="server" EnableViewState="False"></asp:Literal></td>
							</tr>
							<tr>
							    <td colspan="5">&nbsp;</td>
							</tr>
							<tr>
							    <td colspan="5">&nbsp;</td>
							</tr>
							<tr>
								<td>Request Tag</td>
								<td width="10"></td>
								<td colspan="3">
								    <asp:TextBox ID="RequestTag" runat="server" />&nbsp;&nbsp;<i>(optional)</i>
								</td>
							</tr>
						</table>
						<br>
						The next step is to confirm your address.
						<br>
						<br>
						<%
						// carry through values from earlier pages
						RenderRequestString(Constants.FIELD_DATA_ID);
						RenderRequestString(Constants.FIELD_COUNTRY_NAME);
						RenderRequestArray(Constants.FIELD_INPUT_LINES);
						RenderRequestString(FIELD_PROMPTSET);
						RenderRequestString(FIELD_PICKLIST_MONIKER);
						RenderRequestString(FIELD_REFINE_MONIKER);
						%>
						<input type="button" value="   < Back   " id="ButtonBack" runat="server" onserverclick="ButtonBack_ServerClick">
						<INPUT type="submit" value="    Next >   " id="ButtonNext" runat="server" onclick="return validate();" onserverclick="ButtonNext_ServerClick">
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
