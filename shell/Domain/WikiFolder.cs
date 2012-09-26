/* *********************************************************************** *
 * File   : WikiFolder.cs                                 Part of Sitecore *
 * Version: 2.0.0                                         www.sitecore.net *
 *                                                                         *
 *                                                                         *
 * Purpose: WikiFolder implementation                                      *
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

using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;

namespace Sitecore.Modules.Wiki.Domain
{
	public class WikiFolder : CustomItemBase
	{
      public static readonly ID TemplateID = new ID("{01E527A3-A919-427E-8C6D-DB01059A8EFB}");
      
		public WikiFolder(Item item) : base(item)
		{
		}

      public ID FirstPageID
      {
         get
         {
            return this.InnerItem["FirstPage"].Length == 0 ? ID.Null : new ID(this.InnerItem["FirstPage"]);
         }
         set
         {
            using(new SecurityDisabler())
            {
               this.InnerItem.Editing.BeginEdit();
               this.InnerItem["FirstPage"] = value.ToString();
               this.InnerItem.Editing.EndEdit();
            }
         }
      }

      public WikiPage AddPage(string title)
      {
         WikiPage newPage = this.GetPageByTitle(title);
         if(newPage == null)
         {
            using (new SecurityDisabler())
            {
               TemplateItem pageTemplate = DBMaster.Templates[WikiPage.TemplateID];
               newPage = new WikiPage(this.InnerItem.Add("page" + DateTime.Now.Ticks.ToString(), pageTemplate));
               newPage.Title = title;            
               newPage.Publish();
            }
         }
         return newPage;
      }

      public WikiPage GetPageByTitle(string title)
      {
         using (new SecurityDisabler())
         {
            foreach (Item item in this.InnerItem.Children)
            {
               if (item.TemplateID == WikiPage.TemplateID)
               {
                  WikiPage page = new WikiPage(item);
                  if (page.Title == title)
                  {
                     return page;
                  }
               }
            }
         }
         return null;
      }

      public WikiPage GetPageByID(ID id)
      {
         using (new SecurityDisabler())
         {
         
            foreach (Item item in this.InnerItem.Children)
            {
               if ((item.TemplateID == WikiPage.TemplateID) && (item.ID == id))
               {
                  WikiPage page = new WikiPage(item);
                  return page;
               }
            }
         }
         return null;
      }

      public IList Pages
      {
         get
         {
            IList list = new ArrayList();
            using(Sitecore.SecurityModel.SecurityDisabler disabler = new Sitecore.SecurityModel.SecurityDisabler())
            {
               foreach(Item item in this.InnerItem.Children)
               {
                  if (item.TemplateID == WikiPage.TemplateID)
                  {
                     list.Add(new WikiPage(item));
                  }
               }
            }
            return list;
         }
      }

      public IList VersionedPages
      {
         get
         {
            IList list = this.Pages;
            for(int i = list.Count - 1; i >= 0; i--)
            {
               if(((WikiPage)list[i]).CurrentVariant == null)
               {
                  list.RemoveAt(i);
               }
            }
            return list;
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
