<%@ Page language="c#" Inherits="com.qas.prowebintegration.HierSearch" contentType="text/html" EnableEventValidation="false" CodeFile="HierSearch.aspx.cs"%>
<%@ Import namespace="com.qas.proweb" %>
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
		<style type="text/css">
			TABLE.pickle {
				BORDER-COLLAPSE: collapse
			}
			TD.pickle {
				PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 0px; PADDING-TOP: 0px
			}
		</style>
		<script type="text/javascript">
			// Set the focus to the prompt/refinement text box
			function onLoadPage()
			{
				document.frmHierSearch.TextBoxRefine.focus();
				document.frmHierSearch.TextBoxRefine.select();
			}

			// Fire the event for Back button clicked
			function goBack()
			{
				<%= GetPostBackEventReference(ButtonBack) %>;
			}

			// Pick up item's details from the hidden form and submit Format command
			function <%= Commands.Format.ToString() %>(iIndex)
			{
				document.frmHierSearch.<%= HiddenMoniker.ID %>.value = document.getElementsByName("<%= FIELD_MONIKER %>")[iIndex].value;
				document.frmHierSearch.<%= HiddenWarning.ID %>.value = document.getElementsByName("<%= FIELD_WARNING %>")[iIndex].value;
				<%= GetPostBackEventReference(ButtonFormat) %>;
			}

			// Pick up item's details from the hidden form and submit StepIn command
			function <%= Commands.StepIn.ToString() %>(iIndex)
			{
				document.frmHierSearch.<%= HiddenMoniker.ID %>.value = document.getElementsByName("<%= FIELD_MONIKER %>")[iIndex].value;
				document.frmHierSearch.<%= HiddenText.ID %>.value = document.getElementsByName("<%= FIELD_PICKTEXT %>")[iIndex].value;
				document.frmHierSearch.<%= HiddenPostcode.ID %>.value = document.getElementsByName("<%= FIELD_POSTCODE %>")[iIndex].value;
				document.frmHierSearch.<%= HiddenScore.ID %>.value = document.getElementsByName("<%= FIELD_SCORE %>")[iIndex].value;
				document.frmHierSearch.<%= HiddenWarning.ID %>.value = document.getElementsByName("<%= FIELD_WARNING %>")[iIndex].value;
				<%= GetPostBackEventReference(ButtonStepIn) %>;
			}

			// Explain why incomplete address item cannot be stepped into
			function <%= Commands.HaltIncomplete.ToString() %>(iIndex)
			{
				document.frmHierSearch.TextBoxRefine.focus();
				// display some on-screen warning
				alert ("Please enter the premise details.");
			}
			
			// Explain why unresolvable range item cannot be stepped into
			function <%= Commands.HaltRange.ToString() %>(iIndex)
			{
				document.frmHierSearch.TextBoxRefine.focus();
				// display some on-screen warning
				alert ("Please enter a value within the range.");
			}
		</script>
		<style>
		    em { text-decoration: underline; font-style: normal; }
		</style>
	</HEAD>
	<body onload="onLoadPage();">
		<form id="frmHierSearch" method="post" runat="server">
			<table width="800" class="nomargin">
				<tr>
					<th class="banner">
						<a href="index.htm"><h1>QAS Pro On Demand</h1><h2>C# .NET Samples</h2></a>
					</th>
				</tr>
				<tr>
					<td class="nomargin">
						<h1>Rapid Addressing &#8211; Single Line</h1>
						<h3>Select from Address Picklist</h3>
						<P>Enter text to refine your search, or select one of the addresses below.</P>
						<P>
							<TABLE>
								<TR>
									<TD><button id="ButtonNew" accessKey="N" runat="server" onserverclick="ButtonNew_ServerClick"><EM>N</EM>ew</button></TD>
									<TD><button id="ButtonBack" accessKey="B" runat="server" onserverclick="ButtonBack_ServerClick"><EM>B</EM>ack</button></TD>
									<TD>&nbsp;<label id="LabelPrompt" for="TextBoxRefine" runat="server">Prompt</label>:</TD>
									<TD><asp:textbox id="TextBoxRefine" runat="server" EnableViewState="False"></asp:textbox></TD>
									<TD>&nbsp;<asp:button id="ButtonRefine" runat="server" EnableViewState="False" Text="Search" onclick="ButtonRefine_Click"></asp:button></TD>
								</TR>
							</TABLE>
						</P>
						<P>
							<TABLE class="pickle">
								<asp:placeholder id="PlaceHolderWarning" runat="server" EnableViewState="False" Visible="False">
									<TR>
										<TD noWrap colSpan="5"><IMG class="icon" src="img/warning.gif">
											<B><asp:Literal id="LiteralWarning" runat="server" EnableViewState="False"></asp:Literal></B><BR>&nbsp;
										</TD>
									</TR>
								</asp:placeholder>
								<% RenderHistory(); %>
								<tr>
									<td colSpan="5">
										<hr>
									</td>
								</tr>
								<% RenderPicklist(); %>
							    <tr>
							        <td colspan="5">&nbsp;</td>
							    </tr>
							    <tr>
							        <td colspan="5">&nbsp;</td>
							    </tr>
							    <tr>
							        <td>Request Tag
							        </td>
								    <td width="10"></td>
							        <td colspan="3">
							            <asp:TextBox ID="RequestTag" runat="server" />&nbsp;&nbsp;<i>(optional)</i>
							        </td>
							    </tr>
							</TABLE>
						</P>
						<P></P>
						<P><INPUT id="HiddenMoniker" type="hidden" runat="server"> <INPUT id="HiddenText" type="hidden" runat="server">
							<INPUT id="HiddenPostcode" type="hidden" runat="server"> <INPUT id="HiddenScore" type="hidden" runat="server">
							<INPUT id="HiddenWarning" type="hidden" runat="server">
							<asp:button id="ButtonFormat" runat="server" EnableViewState="False" Text="Format" Visible="False" onclick="ButtonFormat_Click"></asp:button><asp:button id="ButtonStepIn" runat="server" EnableViewState="False" Text="StepIn" Visible="False" onclick="ButtonStepIn_Click"></asp:button></P>
						<P></P>
						<P><HR width="100%" SIZE="1"></P>
						<P></P>
						<P><A href="index.htm">Click here to return to the C# .NET Samples home page.</A></P>
					</TD>
				</TR>
			</TABLE>
		</form>
		<form name="PrivateData">
			<% RenderPrivateData(); %>
		</form>
	</body>
</HTML>
