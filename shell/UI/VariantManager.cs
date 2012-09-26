/* *********************************************************************** *
 * File   : VariantManager.cs                             Part of Sitecore *
 * Version: 2.0.0                                         www.sitecore.net *
 *                                                                         *
 *                                                                         *
 * Purpose: VariantManager implementation                                  *
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
	public class VariantManager : XmlControl
	{
      protected Literal VariantDate;
      protected Literal VariantContent;

      protected override void OnLoad(EventArgs e)
      {
         base.OnLoad(e);
         if (!Sitecore.Context.ClientPage.IsEvent)
         {
            ID itemId = new ID(Context.Request.QueryString["id"]);
            Database master = Factory.GetDatabase("master");
            Item item = master.Items[itemId];
            if (item != null)
            {
               WikiPageVariant var = new WikiPageVariant(item);
               VariantDate.Text = var.Date.ToString();
               VariantContent.Text = new WikiConvertor(var.WikiText).TransformWiki();
               VariantDate.ServerProperties.Add("ID", itemId);
            }
         }
      }

      protected void OnSetCurrentClick()
      {
         ID itemId = (ID)VariantDate.ServerProperties["ID"];
         Database master = Factory.GetDatabase("master");
         Item item = master.Items[itemId];
         Domain.WikiPage page = new Domain.WikiPage(item.Parent);
         page.SetCurrentVariant(itemId);
      }
	}
}
