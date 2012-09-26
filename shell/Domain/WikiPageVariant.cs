/* *********************************************************************** *
 * File   : WikiPageVariant.cs                            Part of Sitecore *
 * Version: 2.0.0                                         www.sitecore.net *
 *                                                                         *
 *                                                                         *
 * Purpose: WikiPageVariant implementation                                 *
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

using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;

namespace Sitecore.Modules.Wiki.Domain
{
   public class WikiPageVariant : CustomItemBase
   {
      public static readonly ID TemplateID = new ID("{569B31DD-A0E3-4302-9140-8C92BD1ECBF8}");

      public WikiPageVariant(Item item)
         : base(item)
      {
      }

      public string WikiText
      {
         get
         {
            return this.InnerItem["WikiText"];
         }
         set
         {
            using (new SecurityDisabler())
            {
               this.InnerItem.Editing.BeginEdit();
               this.InnerItem["WikiText"] = value;
               this.InnerItem.Editing.EndEdit();
            }
         }
      }

      public DateTime Date
      {
         get
         {
            return DateUtil.IsoDateToDateTime(this.InnerItem["Date"]);
         }
         set
         {
            using (new SecurityDisabler())
            {
               this.InnerItem.Editing.BeginEdit();
               this.InnerItem["Date"] = DateUtil.ToIsoDate(value);
               this.InnerItem.Editing.EndEdit();
            }
         }
      }
   }
}
