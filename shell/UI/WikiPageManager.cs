/* *********************************************************************** *
 * File   : WikiPageManager.cs                            Part of Sitecore *
 * Version: 2.0.0                                         www.sitecore.net *
 *                                                                         *
 *                                                                         *
 * Purpose: WikiPageManager implementation                                 *
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

using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Modules.Wiki.Domain;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.XmlControls;
using Sitecore.Configuration;

namespace Sitecore.Modules.Wiki.UI
{
	public class WikiPageManager : XmlControl
	{
      protected Literal Title;
      protected Literal PageContent;

      protected override void OnLoad(EventArgs e)
      {
         base.OnLoad(e);         
         ID itemId = new ID(Context.Request.QueryString["id"]);
         Database master = Factory.GetDatabase("master");
         Item item = master.Items[itemId];
         if (item != null)
         {
            Domain.WikiPage wikiPage = new Domain.WikiPage(item);
            Title.Text = wikiPage.Title.ToString();

            try
            {
               PageContent.Text = new WikiConvertor(wikiPage.CurrentVariant.WikiText).TransformWiki();
            }
            catch
            {
               PageContent.Text = "no content";
            }
            Title.ServerProperties.Add("ID", itemId);
         }
      }

      protected void OnSetFirstClick()
      {
         ID itemId = (ID)Title.ServerProperties["ID"];
         Database master = Factory.GetDatabase("master");
         Item item = master.Items[itemId];
         WikiFolder folder = new WikiFolder(item.Parent);
         folder.FirstPageID = itemId;
      }
	}
}
