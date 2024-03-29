<%@ Page language="c#" Inherits="com.qas.prowebintegration.VerifySearch" enableViewState="False" CodeFile="VerifySearch.aspx.cs" %>
<%@ Import namespace="com.qas.prowebintegration" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
    <head>
        <title>QAS Pro On Demand - Address Verification</title>
        <%-- QAS Pro On Demand > (c) QAS Ltd > www.qas.com --%>
        <%-- Web > Verification > VerifySearch --%>
        <%-- Interaction page > Display original address along with found address or picklist of addresses --%>
        <link href="style.css" type="text/css" rel="stylesheet">
        <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
        <meta content="C#" name="CODE_LANGUAGE">
        <meta content="JavaScript" name="vs_defaultClientScript">
        <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
        <script type="text/javascript">
            /* Set the focus */
            function init()
            {
                document.formUserInput.<%= Constants.FIELD_INPUT_LINES %>[0].focus();
            }

            /* picklist item hyperlink has been clicked: pick item, command and submit */
            function doPicklist(iIndex)
            {
                document.formPicklist.<%= Constants.FIELD_MONIKER %>.value = g_asMonikers[iIndex];
                document.formPicklist.<%= FIELD_IS_REFINE %>.value = g_abRefine[iIndex];
                document.formPicklist.submit();
            }

            /* hide the link and show the picklist */
            function showPicklist()
            {
                toggleVisibility('formPicklist', 'showPicklist');
                document.getElementById('picklistToggled').value = true;
            }

            /* toggle visibility of elements, passed through arguments */
            function toggleVisibility()
            {
                for (var i = 0, id; id = arguments[i]; i++)
                {
                    var elem = document.getElementById(id);
                    if (elem) elem.style.display = (elem.style.display != 'none') ? 'none' : '';
                }
            }
        </script>
    </head>
    <body onload="init();">
        <table width="800" class="nomargin">
            <tr>
                <th class="banner">
                    <a href="index.htm" tabindex="-1"><h1>QAS Pro On Demand</h1><h2>C# .NET Samples</h2></a>
                </th>
            </tr>
            <tr>
                <td class="nomargin">
                    <h1>Address Verification</h1>
                    <form name="formUserInput" method="post" action="<%= Constants.PAGE_VERIFY_SEARCH %>">
                        <h3>Confirm the Address Entered</h3>
                        <p>
                            <table>
<%	/* Tab index for lines and (the primary purpose) to get the Accept buttons in the right tab order */
    int iTab = 0;

    for (int i = 0; i < GetInputAddress.Length; i++)
    {
%>								<tr>
                                    <td></td>
                                    <td width="10"></td>
                                    <td><input type="text" name="<%= Constants.FIELD_INPUT_LINES %>" size="50" tabIndex="<%= ++iTab %>" value="<%= HttpUtility.HtmlEncode(GetInputAddress[i]) %>" ></td>
<%		if (i == 0)
        {
            /* Put the submit button next to the first line */
%>									<td width="20"></td>
                                    <td><input type="submit" value="   Accept   " tabIndex="<%= iTab + GetInputAddress.Length %>" ></td>
<%		}
%>								</tr>
<%	}
    /* Bump up tab index as we used one for the button */
    iTab++;
%>								<tr>
                                    <td>Country or Datamap</td>
                                    <td width="10"></td>
                                    <td><input type="text" name="<%= Constants.FIELD_COUNTRY_NAME %>" size="50" tabIndex="-1" class="readonly" readonly value="<%= GetCountry() %>" ></td>
                                </tr>
                            </table>
                        </p>
                        <p><input type="hidden" name="<%= Constants.FIELD_DATA_ID %>" value="<%= DataID %>" ></p>
<%	/* Need to pass along the original user input to make sure we don't verify the same address twice */
    for (int i = 0; i < GetInputAddress.Length; i++)
    {
%>						<input type="hidden" name="<%= FIELD_ORIGINAL_INPUT %>" value="<%= HttpUtility.HtmlEncode(GetInputAddress[i]) %>" >
<%	}
%>					</form>
<%


    /** Address case **/


    if (AddressLines != null)
    {
%>					<form name="VerifiedAddress" action="<%= Constants.PAGE_FINAL_ADDRESS %>" method="post">
                        <h3>Or Accept the Address Found</h3>
                        <p>
                            <table>
<%		for (int i = 0; i < AddressLines.Length; i++)
        {
%>								<tr>
                                    <td><%= AddressLines[i].Label %></td>
                                    <td width="10"></td>
                                    <td><input type="text" name="<%= Constants.FIELD_ADDRESS_LINES %>" size="50" tabIndex="<%= ++iTab %>" value="<%= HttpUtility.HtmlEncode(AddressLines[i].Line) %>" ></td>
<%			if (i == 0)
            {
                /* Put the submit button next to the first line */
%>									<td width="20"></td>
                                    <td><input type="submit" value="   Accept   " tabIndex="<%= iTab + AddressLines.Length %>" ></td>
<%			}
%>								</tr>
<%		}
        /* Bump up tab index as we used one for the button */
        iTab++;
%>								<tr>
                                    <td>Country or Datamap</td>
                                    <td width="10"></td>
                                    <td><input type="text" name="<%= Constants.FIELD_COUNTRY_NAME %>" value="<%= GetCountry() %>" size="50" tabIndex="-1" class="readonly" readonly="true" ></td>
                                </tr>
                            </table>
                        </p>
                    </form>
<%	}


    /** Integrator information **/
    
    if (GetAddressInfo() != null)
    {
%>					<table>
                        <tr>
                            <td class="debug"><img src="img/debug.gif" align="left">Integrator information: <%= GetAddressInfoHTML() %>
                            </td>
                        </tr>
                    </table>
<%	}


    /** Picklist case **/


    if (PicklistItems != null)
    {
%>					<h3>Or Select the Address from the Picklist</h3>

                    <form name="formPicklist" id="formPicklist" method="post" action="<%= Constants.PAGE_VERIFY_SEARCH %>">
                        <table style="line-height: 1.7em">
<%		for (int i=0; i < PicklistItems.Length; i++)
        {
%>							<tr>
                                <td nowrap>
                                    <a href="javascript:doPicklist(<%= i %>);"><%= HttpUtility.HtmlEncode(PicklistItems[i].Text) %></a>
                                </td>
                                <td width="15"></td>
                                <td nowrap><%= PicklistItems[i].Postcode %></td>
                            </tr>
<%		}
%>						</table>
                        <input type="hidden" name="<%= Constants.FIELD_COUNTRY_NAME %>" value="<%= GetCountry() %>" >
                        <input type="hidden" name="<%= Constants.FIELD_MONIKER %>" />
                        <input type="hidden" name="<%= FIELD_IS_REFINE %>" value="true" />
                        <input type="hidden" name="<%= Constants.FIELD_DATA_ID %>" value="<%= DataID %>" >
<%
        /* Pass along the user input in case the next stage fails */
        for (int i = 0; i < GetInputAddress.Length; i++)
        {
%>						<input type="hidden" name="<%= Constants.FIELD_INPUT_LINES %>" value="<%= HttpUtility.HtmlEncode(GetInputAddress[i]) %>" >
<%		}
        /* Write out Javascript arrays - monikers and whether the item requires refinement */
%>						<script type="text/javascript">
var g_asMonikers = new Array(<%= PicklistMonikersStringArray %>);
var g_abRefine = new Array(<%= PicklistExtrasStringArray %>);
                        </script>
                    </form>
<%	}


%>					<input type="hidden" id="picklistToggled" />
                    <p>&nbsp;</p>
                    <p><hr /></p>
                    <p><a href="index.htm">Click here to return to the C# .NET Samples home page.</a></p>
                </td>
            </tr>
        </table>
    </body>
</HTML>
