<%@ Page language="c#" Inherits="com.qas.prowebintegration.FlatPrompt" enableViewState="False" CodeFile="FlatPrompt.aspx.cs" %>
<%@ Import namespace="com.qas.prowebintegration" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<HTML>
	<HEAD>
		<title>QAS Pro On Demand - Address Capture</title>
		<link href="style.css" type="text/css" rel="stylesheet">
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script type="text/javascript">
			/* Set the focus to the first input line */
			function init()
			{
				document.frmFlatPrompt.<%= Constants.FIELD_INPUT_LINES %>[0].focus();
			}
			
			/* Ensure at least one address line has been entered */
			function validate()
			{
				var aUserInput = document.frmFlatPrompt.<%= Constants.FIELD_INPUT_LINES %>;
				for (var i=0; i < aUserInput.length; i++)
				{
					if (aUserInput[i].value != "")
					{
						return true;
					}
				}
				document.frmFlatPrompt.<%= Constants.FIELD_INPUT_LINES %>[0].focus();
				alert("Please enter some address details.");
				return false;
			}
		</script>
	</HEAD>
	<body onload="init();">
		<form id="frmFlatPrompt" method="post" runat="server">
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
						Enter the address elements requested below.<br /><br />

						<table>
							<%
							string[] asInputLines = GetInputLines();
							for (int i=0; i < m_atPromptLines.Length; i++)
							{
								string sValue = "";
								if (i < asInputLines.Length)
								{
									sValue = HttpUtility.HtmlEncode(asInputLines[i]);
								}%>
							<tr>
								<td>
									<%= m_atPromptLines[i].Prompt %>
								</td>
								<td width="10"></td>
								<td><input type="text" size="<%= m_atPromptLines[i].SuggestedInputLength %>" value="<%= sValue %>" name="<%= Constants.FIELD_INPUT_LINES %>" >
									&nbsp;&nbsp;<i>(e.g. <%= HttpUtility.HtmlEncode(m_atPromptLines[i].Example) %>)</i>
								</td>
							</tr>
							<%
							}%>
							<tr>
								<td colSpan="3">
									<P><br>
										<asp:linkbutton id="HyperlinkAlternate" runat="server" EnableViewState="False" onclick="HyperlinkAlternate_Click">If you do not know the postal/ZIP code then click here.</asp:linkbutton></P>
								</td>
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
						<br>
						We will now find your address from the information you have entered above.
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
