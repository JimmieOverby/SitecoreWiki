/* *********************************************************************** *
 * File   : WikiEditor.aspx.cs                            Part of Sitecore *
 * Version: 2.0.0                                         www.sitecore.net *
 *                                                                         *
 *                                                                         *
 * Purpose: Editor for Wiki                                                *
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
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Sitecore.Modules.Wiki;
using Sitecore.Modules.Wiki.Domain;

namespace Sitecore.Modules.Wiki.UI
{
   public partial class WikiEditor : UserControl
   {
      public event EventHandler<EventArgs> SaveButtonClick;

      protected void btnPreview_Click(object sender, System.EventArgs e)
      {
         Preview.Text =
            WikiConvertor.FormatTextWrap(new WikiConvertor(WikiConvertor.ClearSystemSymbol(MainArea.InnerText)).TransformWiki(), 2 * MainArea.InnerText.Length);
      }

      protected void btnSave_Click(object sender, System.EventArgs e)
      {
         if (SaveButtonClick != null)
         {
            SaveButtonClick(sender, e);
         }
      }

      public string WikiText
      {
         get
         {
            return MainArea.InnerText;
         }
         set
         {
            MainArea.InnerText = value;
         }
      }
   }
}
