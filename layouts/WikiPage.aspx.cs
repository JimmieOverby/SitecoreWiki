/* *********************************************************************** *
 * File   : WikiPage.aspx.cs                              Part of Sitecore *
 * Version: 2.0.0                                         www.sitecore.net *
 *                                                                         *
 *                                                                         *
 * Purpose: Wiki manager page                                              *
 *                                                                         *
 * Bugs   : None known.                                                    *
 *                                                                         *
 * Status : Published.                                                     *
 *                                                                         *
 * Copyright (C) 1999-2007 by Sitecore A/S. All rights reserved.           *
 *                                                                         *
 * This work is the property of:                                           *
 *                                                                         *
 *        Sitecore A/S                                                     *
 *        Meldahlsgade 5, 4.                                               *
 *        1613 Copenhagen V.                                               *
 *        Denmark                                                          *
 *                                                                         *
 * This is a Sitecore published work under Sitecore's                      *
 * shared source license.                                                  *
 *                                                                         *
 * *********************************************************************** */

using System;
using System.Collections;
using System.Web.UI;
using System.Text;

using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Modules.Wiki;
using Sitecore.Modules.Wiki.Domain;
using Sitecore.Publishing;
using Sitecore.Configuration;

namespace Sitecore.Modules.Wiki.UI
{
   public partial class WikiPage : Page
   {
      protected void Page_Load(object sender, EventArgs e)
      {
         this.WikiEditor.SaveButtonClick += new EventHandler<EventArgs>(WikiEditor_SaveButtonClick);
         if (!Page.IsPostBack)
         {
            if (CurrentMasterItem.TemplateID == WikiFolder.TemplateID)
            {
               SetupPageForFolder();
            }
            else
            {
               SetupPageFowWikiPage();
            }
            Domain.WikiFolder folder = CurrentMasterItem.TemplateID == WikiFolder.TemplateID ?
                                       new WikiFolder(CurrentMasterItem) : new WikiFolder(CurrentMasterItem.Parent);
            RandomArticle.Visible = folder.VersionedPages.Count > 0;
         }
         btnTitleSave.Attributes.Add("onclick", "javascript:return ProcessTitle();");
      }

      protected void WikiEditor_SaveButtonClick(object sender, EventArgs e)
      {
         Domain.WikiPage currentPage = new Domain.WikiPage(CurrentMasterItem);
         WikiEditor.WikiText = WikiConvertor.ClearSystemSymbol(WikiEditor.WikiText);
         WikiEditor.WikiText = WikiConvertor.FormatTextWrap(WikiEditor.WikiText, 2 * WikiEditor.WikiText.Length);
         currentPage.AddVariant(WikiEditor.WikiText, true);
         Content.Text = new WikiConvertor(WikiEditor.WikiText).TransformWiki();
         Content.Visible = true;
         WikiEditor.Visible = false;
         EditArticle.Visible = true;
         RandomArticle.Visible = true;
      }

      protected void btnMainPage_Click(object sender, EventArgs e)
      {
         WikiFolder folder = new WikiFolder(this.CurrentMasterItem.Parent);
         if (folder.FirstPageID != Data.ID.Null)
         {
            RedirectToPage(DBMaster.Items[folder.FirstPageID]);
         }
      }

      protected void btnTitleSave_Click(object sender, EventArgs e)
      {
         if (textBoxTitle.Text.Trim().Length > 0)
         {
            textBoxTitle.Text = WikiConvertor.ClearSystemSymbol(textBoxTitle.Text);
            textBoxTitle.Text = WikiConvertor.FormatTextWrap(new WikiConvertor(textBoxTitle.Text).TransformStandart(), textBoxTitle.MaxLength);
            if (CurrentMasterItem.TemplateID == WikiFolder.TemplateID)
            {
               WikiFolder folder = new WikiFolder(CurrentMasterItem);
               Domain.WikiPage newPage = folder.AddPage(textBoxTitle.Text);
               folder.FirstPageID = newPage.ID;
               SetupPageForFolder();
            }
            if (CurrentMasterItem.TemplateID == Domain.WikiPage.TemplateID)
            {
               WikiFolder folder = new WikiFolder(CurrentMasterItem.Parent);
               Domain.WikiPage newPage = folder.AddPage(textBoxTitle.Text);
               RedirectToPage(newPage.InnerItem);
            }
         }
      }

      protected void EditArticle_Click(object sender, EventArgs e)
      {
         Domain.WikiPage currentPage = new Domain.WikiPage(CurrentMasterItem);
         currentPage.CurrentVariant.WikiText = WikiConvertor.ClearSystemSymbol(currentPage.CurrentVariant.WikiText);
         WikiEditor.WikiText = currentPage.CurrentVariant.WikiText.Replace("&shy;", string.Empty);
         SwitchToEditMode();
      }

      protected void btnCreatePage_Click(object sender, EventArgs e)
      {
         EditArticle.Visible = false;
         Content.Visible = false;
         WikiEditor.Visible = false;
         lblHeader.Text = "Add article title";
         lblHeader.Visible = true;
         textBoxTitle.Visible = true;
         btnTitleSave.Visible = true;
      }

      protected void btnPageList_Click(object sender, EventArgs e)
      {
         EditArticle.Visible = false;
         btnTitleSave.Visible = false;
         lblArticeTitle.Visible = false;
         textBoxTitle.Visible = false;
         SwitchToContentMode();
         WikiFolder folder = CurrentMasterItem.TemplateID == WikiFolder.TemplateID ?
                             new WikiFolder(CurrentMasterItem) : new WikiFolder(CurrentMasterItem.Parent);
         IList pages = folder.Pages;
         Content.Text = "<ul>";
         lblHeader.Text = "Articles in WIKI";
         foreach (Domain.WikiPage page in pages)
         {
            Content.Text += string.Format("<li><a href=\"?q={1}\">{2}</a></li>", this.Request.Url.AbsolutePath,
               Page.Server.UrlEncode(page.Title), page.Title);
         }
         Content.Text += "</ul>";
      }

      protected void RandomArticle_Click(object sender, System.EventArgs e)
      {
         Domain.WikiFolder folder = CurrentMasterItem.TemplateID == Domain.WikiFolder.TemplateID ?
                                    new WikiFolder(CurrentMasterItem) : new WikiFolder(CurrentMasterItem.Parent);
         IList pages = folder.VersionedPages;
         if (pages.Count > 0)
         {
            int index = new Random().Next(pages.Count);
            Domain.WikiPage page = (Domain.WikiPage)pages[index];
            RedirectToPage(page.InnerItem);
         }
      }

      void SetupPageFowWikiPage()
      {
         btnTitleSave.Visible = false;
         lblArticeTitle.Visible = false;
         textBoxTitle.Visible = false;
         lblHeader.Text = new Domain.WikiPage(CurrentMasterItem).Title;

         string query = Request.QueryString["q"];
         if ((query != null) && (query != string.Empty))
         {
            string pageTitle = query;
            WikiFolder folder = new WikiFolder(CurrentMasterItem.Parent);
            Domain.WikiPage pageToRedirect = folder.GetPageByTitle(pageTitle);
            if (pageToRedirect == null)
            {
               pageToRedirect = folder.AddPage(pageTitle);
               pageToRedirect.Publish();
            }
            RedirectToPage(pageToRedirect.InnerItem);
         }
         Domain.WikiPage currentPage = new Domain.WikiPage(CurrentMasterItem);
         if (currentPage.CurrentVariant == null)
         {
            SwitchToEditMode();
         }
         else
         {
            SwitchToContentMode();
            Content.Text = new WikiConvertor(currentPage.CurrentVariant.WikiText).TransformWiki();
         }
      }

      Item ProcessFirstPage(Domain.WikiFolder folder)
      {
         Item item = DBMaster.Items[folder.FirstPageID];
         if (item == null)
         {
            IList pages = folder.Pages;
            if ((pages != null) && (pages.Count > 0))
            {
               item = ((CustomItemBase)pages[0]).InnerItem;
            }
         }
         else
         {
            folder.FirstPageID = item.ID;
         }
         return item;
      }

      void SetupPageForFolder()
      {
         WikiFolder folder = new WikiFolder(CurrentMasterItem);
         if (folder.FirstPageID != Sitecore.Data.ID.Null)
         {
            Item item = ProcessFirstPage(folder);
            if (item != null)
            {
               RedirectToPage(item);
            }
            else
            {
               btnCreatePage_Click(this, EventArgs.Empty);
            }
         }
         else
         {
            Content.Text = "";
            WikiEditor.Visible = false;
            lblHeader.Text = "Add first article";
         }
      }

      void RedirectToPage(Item item)
      {
         Response.Redirect(string.Format("{0}://{1}{2}.aspx", Page.Request.Url.Scheme,
            this.Request.Url.Host, item.Paths.FullPath));
      }

      void SwitchToEditMode()
      {
         EditArticle.Visible = false;
         WikiEditor.Visible = true;
         Content.Visible = false;
      }

      void SwitchToContentMode()
      {
         WikiEditor.Visible = false;
         Content.Visible = true;
      }

      Item CurrentMasterItem
      {
         get
         {
            Item item = null;
            item = DBMaster.Items[Sitecore.Context.Item.ID];
            if (item == null)
            {
               Server.Transfer(Settings.ItemNotFoundUrl);
            }
            return item;
         }
      }

      Database DBMaster
      {
         get
         {
            return Sitecore.Configuration.Factory.GetDatabase("master");
         }
      }
   }
}
