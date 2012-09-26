/* *********************************************************************** *
 * File   : WikiPage.cs                                   Part of Sitecore *
 * Version: 2.0.0                                         www.sitecore.net *
 *                                                                         *
 *                                                                         *
 * Purpose: WikiPage implementation                                        *
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

using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Templates;
using Sitecore.Publishing;
using Sitecore.SecurityModel;

namespace Sitecore.Modules.Wiki.Domain
{
   public class WikiPage : CustomItemBase
   {
      public static readonly ID TemplateID = new ID("{C5FF8BA9-8A59-4C4A-BD7E-D352A9BA766D}");

      public WikiPage(Item item) : base(item)
      {
      }

      public IList Variants
      {
         get
         {
            IList list = new ArrayList();
            using(Sitecore.SecurityModel.SecurityDisabler disabler = new Sitecore.SecurityModel.SecurityDisabler())
            {
               foreach(Item item in this.InnerItem.Children)
               {
                  if (item.TemplateID == WikiPageVariant.TemplateID)
                  {
                     list.Add((new WikiPageVariant(item)));
                  }
               }
            }
            return list;
         }
      }

      public WikiPageVariant CurrentVariant
      {
         get
         {
            string currentStr = this.InnerItem["CurrentVariant"];
            if (currentStr.Length > 0)
            {
               ID itemID = new ID(this.InnerItem["CurrentVariant"]);
               Item item = DBMaster.Items[itemID];
               return item == null ? null : new WikiPageVariant(item);
            }
            return null;
         }
         set
         {
            using(new SecurityDisabler())
            {
               this.InnerItem.Editing.BeginEdit();
               this.InnerItem["CurrentVariant"] = value.ID.ToString();
               this.InnerItem.Editing.EndEdit();
            }
         }
      }

      public string Title
      {
         get
         {
            return this.InnerItem["Title"];
         }
         set
         {
            using(new SecurityDisabler())
            {
               this.InnerItem.Editing.BeginEdit();
               this.InnerItem["Title"] = value;
               this.InnerItem.Editing.EndEdit();
            }
         }
      }

      public void AddVariant(string text, bool isSetcurrent)
      {
         using(new SecurityDisabler())
         {
            TemplateItem template = DBMaster.Templates[WikiPageVariant.TemplateID];
            DateTime created = DateTime.Now;
            Item newItem = this.InnerItem.Add("Variant" + created.Ticks.ToString(), template);
            newItem.Editing.BeginEdit();
            WikiPageVariant variant = new WikiPageVariant(newItem);
            variant.WikiText = text;
            variant.Date = created;
            if (isSetcurrent)
            {
               this.CurrentVariant = variant;
            }
         }
      }

      public void SetCurrentVariant(ID id)
      {
         using(new SecurityDisabler())
         {
            this.InnerItem.Editing.BeginEdit();
            this.InnerItem["CurrentVariant"] = id.ToString();
            this.InnerItem.Editing.EndEdit();
         }
      }

      public void Publish()
      {
          using (new SecurityDisabler())
          {
              PublishOptions options = new PublishOptions(InnerItem.Database, Configuration.Factory.GetDatabase("web"), PublishMode.SingleItem, InnerItem.Language, DateTime.Now);
              options.RootItem = InnerItem;
              options.Deep = false;
              Publisher publisher = new Publisher(options);
              publisher.Publish();
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
