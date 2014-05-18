<%@ Page language="c#" Inherits="com.qas.prowebintegration.KeyPrompt" enableViewState="False" CodeFile="KeyPrompt.aspx.cs" %>
<%@ Import namespace="com.qas.prowebintegration" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<HTML>
	<HEAD>
		<title>QuickAddress Pro Web - Key Search</title>
		<link href="style.css" type="text/css" rel="stylesheet">
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script type="text/javascript">
			/* Set the focus to the first input line */
			function init()
			{
				document.frmKeyPrompt.<%= Constants.FIELD_INPUT_LINES %>.focus();
			}
			
			/* Ensure at least one address line has been entered */
			function validate()
			{
				var aUserInput = document.frmKeyPrompt.<%= Constants.FIELD_INPUT_LINES %>;

				if (aUserInput.value != "")
				{
					return true;
				}
				
				document.frmKeyPrompt.<%= Constants.FIELD_INPUT_LINES %>.focus();
				alert("Please enter a search.");
				return false;
			}
		</script>
	</HEAD>
	<body onload="init();">
		<form id="frmKeyPrompt" method="post" runat="server">
			<table width="800" class="nomargin">
				<tr>
					<th class="banner">
						<a href="index.htm"><h1>QuickAddress Pro Web</h1><h2>C# .NET Samples</h2></a>
					</th>
				</tr>
				<tr>
					<td class="nomargin">
						<h1>Key Search</h1>
						<p>Enter the key to search on below.</p>
						<br />

						<table>
						
							<%
							string[] asInputLines = GetInputLines();
                            string sValue = "";
                            if (asInputLines != null && asInputLines.Length > 0)
                            {
                                sValue = HttpUtility.HtmlEncode(asInputLines[0]);
                            }
							%>
							<tr>
								<td>
									Enter Search
								</td>
								<td width="10"></td>
								<td>
								    <input type="text" size="16" value="<%= sValue %>" name="<%= Constants.FIELD_INPUT_LINES %>" >
								</td>
							</tr>
							<tr>
							    <td>&nbsp;</td>
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
						We will then find an address from the information you have entered above.
						<br>
						<br>
						<%
						RenderRequestString(Constants.FIELD_DATA_ID);
						RenderRequestString(Constants.FIELD_COUNTRY_NAME);%>
						<input id="HiddenPromptSet" type="hidden" runat="server" />
						<input id="ButtonBack" type="button" value="   < Back   " runat="server" onserverclick="ButtonBack_ServerClick" />
						<input id="ButtonNext" type="submit" value="    Next >   " runat="server" onclick="return validate();" onserverclick="ButtonNext_ServerClick" />
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

