<%@ Page Language="c#" CodeFile="WikiPage.aspx.cs" AutoEventWireup="true" EnableEventValidation="false"
    Inherits="Sitecore.Modules.Wiki.UI.WikiPage" %>

<%@ Register TagPrefix="wiki" TagName="WikiEditor" Src="WikiEditor.ascx" %>
<%@ OutputCache Location="None" VaryByParam="none" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Sitecore Wiki</title>
    <link href="/default.css" rel="stylesheet" />
    <style type="text/css">
        .MenuItem {
    	    FONT-WEIGHT: bold; FONT-SIZE: 12px; COLOR: black; FONT-FAMILY: arial; TEXT-DECORATION: none
        }
        .MenuItem:hover {
	        COLOR: black; FONT-FAMILY: arial; TEXT-DECORATION: underline
        }
        .MainTable {
	        VERTICAL-ALIGN: top	        
        }
    </style>

    <script type="text/javascript" language="javascript">
      <!--
         function ProcessTitle()
         {
            if((window.document.getElementById('textBoxTitle') != null) && (trimAll(window.document.getElementById('textBoxTitle').value).length == 0))
            {
               alert("Title cannot be empty");
               window.document.getElementById('textBoxTitle').focus();
               return false;
            }
            return true;
         }
         
         function trimAll(sString) 
         {
            while (sString.substring(0,1) == ' ')
            {
               sString = sString.substring(1, sString.length);
            }
            while (sString.substring(sString.length-1, sString.length) == ' ')
            {
               sString = sString.substring(0,sString.length-1);
            }
            return sString;
         }

         
         function OnTextboxClick()
			{
				if ((event.keyCode==13) && ProcessTitle() && (window.document.getElementById('textBoxTitle') != null))
				{
					__doPostBack('btnTitleSave', 'btnTitleSave:Click');
				}
				return true;
			}
			
			function OnLoad()
			{
			   if(window.document.getElementById('textBoxTitle') != null)
			   {
			      window.document.getElementById('textBoxTitle').focus();
			   }
			   return true;
			}
      -->
    </script>

</head>
<body style="padding-right: 0px; padding-left: 0px; padding-bottom: 0px; margin: 0px;
    padding-top: 9px; background-color: #cccccc" onload="OnLoad();">
    <form id="mainform" method="post" runat="server">
        <table class="MainTable" style="padding-right: 0px; padding-left: 0px; padding-bottom: 0px;
            margin: 0px; padding-top: 0px; height: 100%" width="100%" border="0">
            <tbody>
                <tr>
                    <td style="background-color: #eeeeee; width: 10%" valign="top">
                        <div>
                            <table>
                                <tbody>
                                    <tr>
                                        <td class="MenuItem">
                                            <asp:LinkButton ID="btnMainPage" TabIndex="1" runat="server" CssClass="MenuItem"
                                                OnClick="btnMainPage_Click">Main Page</asp:LinkButton></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:LinkButton ID="btnCreatePage" TabIndex="1" runat="server" CssClass="MenuItem"
                                                OnClick="btnCreatePage_Click">Create Article</asp:LinkButton></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:LinkButton ID="btnPageList" TabIndex="1" runat="server" CssClass="MenuItem"
                                                OnClick="btnPageList_Click">Article List</asp:LinkButton></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:LinkButton ID="RandomArticle" TabIndex="1" runat="server" CssClass="MenuItem"
                                                OnClick="RandomArticle_Click">Random Article</asp:LinkButton></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:LinkButton ID="EditArticle" TabIndex="1" runat="server" CssClass="MenuItem"
                                                OnClick="EditArticle_Click">Edit Article</asp:LinkButton></td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </td>
                    <td valign="top" align="center">
                        <table width="100%">
                            <tbody>
                                <tr>
                                    <td align="center">
                                        <div id="contentHeader" runat="server" style="FONT-WEIGHT: bold; FONT-SIZE: 20px; FONT-FAMILY: arial">
                                           <asp:Literal ID="lblHeader" runat="server"></asp:Literal>                                          
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td valign="top" align="center">
                                        <asp:Label ID="lblArticeTitle" runat="server">Article Title:</asp:Label>
                                        <asp:TextBox ID="textBoxTitle" onkeydown="OnTextboxClick();" runat="server" MaxLength="150"></asp:TextBox>
                                        <asp:Button ID="btnTitleSave" runat="server" Text="Create" OnClick="btnTitleSave_Click">
                                        </asp:Button>
                                        <div id="ContentDiv" runat="server" align="left" style="max-width : 500px;">
                                            <asp:Literal ID="Content" runat="server"></asp:Literal></div>
                                        <wiki:WikiEditor ID="WikiEditor" runat="server"></wiki:WikiEditor>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
            </tbody>
        </table>
    </form>
</body>
</html>
