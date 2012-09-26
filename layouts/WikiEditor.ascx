<%@ Control Language="C#" AutoEventWireup="true" CodeFile="WikiEditor.ascx.cs" Inherits="Sitecore.Modules.Wiki.UI.WikiEditor" %>
<%@ Register TagPrefix="sc" Namespace="Sitecore.Modules.Wiki.Domain" Assembly="Sitecore.Wiki" %>    

<table>
    <tr>
        <td align="center">
            <input style="font-weight: bold; width: 32px; font-family: 'Arial Black'; height: 32px"
                onclick="AddBold()" type="button" value="B"/>
            <input style="font-weight: normal; width: 32px; font-style: italic; font-family: 'Arial Black';
                height: 32px" type="button" onclick="AddItalic()" value="I"/>
            <input style="font-weight: normal; width: 32px; font-family: 'Arial Black'; height: 32px;
                text-decoration: underline" type="button" onclick="AddUnderline()" value="U"/>
            <input style="font-weight: normal; width: 32px; font-family: 'Arial Black'; height: 32px;
                text-decoration: line-through" type="button" onclick="AddStrike()" value="S"/>
            <input style="font-weight: normal; font-size: 7pt; width: 32px; font-family: 'Arial Black';
                height: 32px" type="button" onclick="AddSmall()" value="small"/>
            <input style="font-weight: normal; width: 32px; font-family: 'Arial Black'; height: 32px"
                type="button" onclick="AddH2()" value="H2"/>
            <input style="font-weight: normal; width: 32px; font-family: 'Arial Black'; height: 32px"
                type="button" onclick="AddH3()" value="H3"/>&nbsp;
            <button style="font-weight: normal; width: 32px; font-family: 'Arial Black'; height: 32px"
                onclick="AddLink()">&nbsp;
                <img alt="Link" src="/sitecore/images/links.gif"
                    width="32px" height="32px"/></button>
        </td>
    </tr>
    <tr>
        <td align="center">
            <textarea id="MainArea" style="width: 768px; height: 398px" rows="25" cols="93" runat="server"></textarea>
        </td>
    </tr>
    <tr>
        <td align="center">
            <asp:Button ID="btnSave" runat="server" Text="Save" Width="80px" OnClick="btnSave_Click"></asp:Button><asp:Button
                ID="btnPreview" runat="server" Text="Preview" Width="80px" OnClick="btnPreview_Click"></asp:Button></td>
    </tr>
    <tr>
        <td>
            <hr/>
        </td>
    </tr>
    <tr>
        <td align="center" style="color: black; font-family: Arial">
            Preview:
        </td>
    </tr>
    <tr>
        <td>
            <div style="border-right: #666666 solid; border-top: #666666 solid; border-left: #666666 solid;
                border-bottom: #666666 solid">
                <asp:Literal ID="Preview" runat="server"></asp:Literal>
            </div>
        </td>
    </tr>
</table>

<script type="text/javascript" language="javascript">
 function AddBold()
 {
   AddWiki("''' bold text '''");
 }
 
 function AddItalic()
 {
   AddWiki("'' italic text ''");
 }
 
 function AddUnderline()
 {
   AddWiki("<u> underline text </u>");
 }

 function AddStrike()
 {
   AddWiki("<strike> strike text </strike>");
 }
 
  function AddSmall()
 {
   AddWiki("<small> small text </small>");
 }
 
 function AddH2()
 {
   AddWiki("=== Header 2 ===");
 }

 function AddH3()
 {
   AddWiki("==== Header 3 ====");
 }
 
 function AddLink()
 {
   AddWiki("[[http://www.example.com Link]]");
 }


 function AddWiki(s)
 {
   var mainArea = window.document.getElementById("WikiEditor_MainArea");
   mainArea.focus();
   var sel = document.selection.createRange();
   sel.text = s + sel.text;
 }
</script>