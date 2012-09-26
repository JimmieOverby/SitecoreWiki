/* *********************************************************************** *
 * File   : WikiConvertor.cs                              Part of Sitecore *
 * Version: 2.0.0                                         www.sitecore.net *
 *                                                                         *
 *                                                                         *
 * Purpose: WikiConvertor implementation                                   *
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
using System.Security.Policy;
using System.Text.RegularExpressions;

namespace Sitecore.Modules.Wiki.Domain
{
   public class WikiConvertor
   {
      string wikiText = string.Empty;
      string baseUrl = string.Empty;
      Stack activeTags = new Stack();
      static int lineCount = 10;
      static readonly char systemHyphen = (char)173;
      static readonly string[] separateStartChars = new string[] { " ", "<", "&", "&lt;", "<", "&lt;" };
      static readonly string[] separateEndChars = new string[] { " ", ">", ";", "&gt;", "&gt;", ">" };
      static readonly int[] limitsForLiteral = new int[] { 2 * lineCount, 2 * lineCount, 6 };

      static int IndexOfFirstSeparateChars(string text, int startpos, int count)
      {
         if (startpos > 0)
         {
            int countprev = lineCount;

            if (startpos - countprev < 0)
            {
               countprev = startpos;
            }

            for (int i = 0; i < separateStartChars.Length; ++i)
            {
               int prev = text.LastIndexOf(separateStartChars[i], startpos, countprev);

               prev = prev < text.LastIndexOf(separateEndChars[i], startpos, countprev) &&
                     text.LastIndexOf(separateEndChars[i], startpos, countprev) > -1 ? -1 : prev;
               int next = text.IndexOf(separateEndChars[i], startpos, count);
               next = next > text.IndexOf(separateStartChars[i], startpos, count) &&
                      text.IndexOf(separateStartChars[i], startpos, count) > -1 ? -1 : next;
               if (prev >= 0 && next >= 0)
               {
                  return next;
               }
            }
         }
         return -1;
      }


      static public string ClearSystemSymbol(string text)
      {
         int pos = 0;

         while (text.IndexOf(systemHyphen, pos) > 0)
         {
            pos = text.IndexOf(systemHyphen, pos);
            text = text.Remove(pos, 1);
         }

         while (text.IndexOf("&shy;", pos) > 0)
         {
            pos = text.IndexOf("&shy;", pos);
            text = text.Remove(pos, "&shy;".Length);
         }

         return text;
      }

      static public string FormatTextWrap(string text, int maxLength)
      {
         if (text.Length > 0)
         {
            int pos = 0;
            int count = 0;
            while (pos < maxLength && pos < text.Length)
            {
               count = text.Length - pos > lineCount ? lineCount : text.Length - pos - 2;
               if (pos + count >= text.Length)
               {
                  count = text.Length - pos;
               }
               if (count > 0)
               {
                  int posSeparator = IndexOfFirstSeparateChars(text, pos, count);
                  if (posSeparator < 0)
                  {
                     text = text.Insert(pos, "&shy;");
                     pos += count + 5;
                     maxLength += 5;
                  }
                  else
                  {
                     pos += posSeparator;
                  }
               }
               ++pos;
            }
         }
         return text;
      }

      public WikiConvertor(string wikiText)
      {
         this.wikiText = wikiText;
      }

      public string TransformStandart()
      {
         return FormatStandard(wikiText);
      }

      public string TransformWiki()
      {
         return FormatWikipedia(wikiText);
      }

      protected string WikipediaLink(Match m) 
      {
         string result = m.Groups[1].Value;

         Match url = Regex.Match(result, "^(http|news|ftp|mailto)\\:(\\/\\/)?((?:\\S(?!\\.gif|\\.jpg|\\.jpeg))+)\\s", RegexOptions.IgnoreCase);

         if (url.Success) 
         {
            string href = url.Groups[1].Value + ":" + url.Groups[2].Value + url.Groups[3].Value;
            string text = result.Substring(href.Length + 1);

            result = "<a href=\"" + href + "\">" + text + "</a>";
         }
         else if (result.IndexOf("|") >= 0) 
         {
            string[] parts = result.Split('|');
            result = "<a href=\"" + baseUrl + "?q=" + parts[0] + "\">" + parts[1] + "</a>";
         }
         else 
         {
            result = "<a href=\"" + baseUrl + "?q=" + result + "\">" + result + "</a>";
         }

         return result;
      }

      protected string Url(Match m) 
      {
         string result = m.Groups[1].Value + ":" + m.Groups[2].Value + m.Groups[3].Value;
         result = result.Replace("<a:wiki>", "").Replace("</a:wiki>", "");
         return "<a href=\"" + result + "\">" + result + "</a> ";
      }

      protected string Image(Match m) 
      {
         string result = m.Groups[1].Value + ":" + m.Groups[2].Value + m.Groups[3].Value + m.Groups[4].Value;
         result = result.Replace("<a:wiki>", "").Replace("</a:wiki>", "");
         return "<img src=\"" + result + "\" alt=\"\" border=\"0\"/>";
      }

      protected string WikiWord(Match m) 
      {
         return "<a href=\"" + baseUrl + "?q=" + m.Groups[1].Value + "\">" + m.Groups[1].Value + "</a>";
      }

      protected string FormatStandard(string text) 
      {
         // replace tabs
         text = Regex.Replace(text, "<", "&lt;", RegexOptions.IgnoreCase);
         text = Regex.Replace(text, ">", "&gt;", RegexOptions.IgnoreCase);

         // replace tabs
         text = Regex.Replace(text, " {3,8}", "\t", RegexOptions.IgnoreCase);

         // outline
         text = Regex.Replace(text, "^\\t[0-9]\\s*(.*)(?:\\n?|$)", "<li>$1</li>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
         text = Regex.Replace(text, "(?<!</li>)((?:<li>.*</li>)+)(?!<li>)", "<ol>$1</ol>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
         text = Regex.Replace(text, "^\\t\\*\\s*(.*)(?:\\n?|$)", "<li>$1</li>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
         text = Regex.Replace(text, "(?<!</li>|<ol>)((?:<li>.*</li>)+)(!<li>|</ol>)", "<ul>$1</ul>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
         text = Regex.Replace(text, "\\<li\\>\\.", "<li>", RegexOptions.IgnoreCase);

         // horizontal line
         text = Regex.Replace(text, "-{4,}", "<hr>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
      
         // fixed font
         text = Regex.Replace(text, "^([ \\t]+.*)\\n", "<pre>$1</pre>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
         text = Regex.Replace(text, "</pre><pre>", "", RegexOptions.IgnoreCase);

         // WikiWord marker
         text = Regex.Replace(text, "([^A-Za-z])([A-Z][a-z]+[A-Z][A-Za-z]*)([^A-Za-z])", "$1<a:wiki>$2</a:wiki>$3");

         // URL
         text = Regex.Replace(text, "(http|news|ftp|mailto)\\:(\\/\\/)?((?:\\S(?!\\.gif|\\.jpg|\\.jpeg))+)\\s", new MatchEvaluator(Url), RegexOptions.IgnoreCase);
         text = Regex.Replace(text, "(?<!href=\")(http|news|ftp|mailto)\\:(\\/\\/)?((?:\\S(?!\\.gif|\\.jpg|\\.jpeg|\\.png))+\\S)(\\.gif|\\.jpg|\\.jpeg|\\.png)", new MatchEvaluator(Image), RegexOptions.IgnoreCase);

         // WikiWord 
         text = Regex.Replace(text, "<a:wiki>([A-Z][a-z]+[A-Z][A-Za-z]*)</a:wiki>", new MatchEvaluator(WikiWord), RegexOptions.IgnoreCase);

         // emphasis ''' -> <b>
         text = Regex.Replace(text, "(''')(.*?)\\1", "<b>$2</b>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

         // emphasis '' -> <i>
         text = Regex.Replace(text, "('')(.*?)\\1", "<i>$2</i>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

         // line breaks
         text = text.Replace("\n", "<br/>");
         text = Regex.Replace(text, "<br/><pre>", "<pre>", RegexOptions.IgnoreCase);

         // replace tabs
         text = Regex.Replace(text, "\\t", "        ", RegexOptions.IgnoreCase);

         return text;
      }

      protected string FormatWikipedia(string text) 
      {
         // replace tabs
         text = Regex.Replace(text, "<", "&lt;", RegexOptions.IgnoreCase);
         text = Regex.Replace(text, ">", "&gt;", RegexOptions.IgnoreCase);

         // fixed font
         text = Regex.Replace(text, "^([ \\t]+.*)\\n", "<pre>$1</pre>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
         text = Regex.Replace(text, "</pre><pre>", "", RegexOptions.IgnoreCase);

         // heading 3
         text = Regex.Replace(text, @"^====(.*)====", "<h3>$1</h3>", RegexOptions.IgnoreCase | RegexOptions.Multiline);

         // heading 2
         text = Regex.Replace(text, @"^===(.*)===", "<h2>$1</h2>", RegexOptions.IgnoreCase | RegexOptions.Multiline);

         // heading 1
         text = Regex.Replace(text, @"^==(.*)==", "<h1>$1</h1>", RegexOptions.IgnoreCase | RegexOptions.Multiline);

         // outline
         text = Regex.Replace(text, "^\\#+\\s*(.*)(?:\\n?|$)", "<li1>$1</li1>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
         text = Regex.Replace(text, "(?<!</li1>)((?:<li1>.*</li1>)+)(?!<li1>)", "<ol>$1</ol>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
         text = Regex.Replace(text, "^\\*+\\s*(.*)(?:\\n?|$)", "<li2>$1</li2>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
         text = Regex.Replace(text, "(?<!</li2>)((?:<li2>.*</li2>)+)(?!<li2>)", "<ul>$1</ul>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
         text = Regex.Replace(text, "<li1>", "<li>", RegexOptions.IgnoreCase);
         text = Regex.Replace(text, "</li1>", "</li>", RegexOptions.IgnoreCase);
         text = Regex.Replace(text, "<li2>", "<li>", RegexOptions.IgnoreCase);
         text = Regex.Replace(text, "</li2>", "</li>", RegexOptions.IgnoreCase);

         text = Regex.Replace(text, "\\<li\\>\\.", "<li>", RegexOptions.IgnoreCase);

         // indent
         text = Regex.Replace(text, "^;(.*)", "<div style=\"padding-left:24px\">$1</div>", RegexOptions.IgnoreCase | RegexOptions.Multiline);

         // indent
         text = Regex.Replace(text, "^:(.*)", "<div style=\"padding-left:24px\">$1</div>", RegexOptions.IgnoreCase | RegexOptions.Multiline);

         // horizontal line
         text = Regex.Replace(text, "-{4,}", "<hr>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
      
         // URL
         text = Regex.Replace(text, "[^\\[](http|news|ftp|mailto)\\:(\\/\\/)?((?:\\S(?!\\.gif|\\.jpg|\\.jpeg))+)\\s", new MatchEvaluator(Url), RegexOptions.IgnoreCase);
         text = Regex.Replace(text, "(?<!href=\")(http|news|ftp|mailto)\\:(\\/\\/)?((?:\\S(?!\\.gif|\\.jpg|\\.jpeg|\\.png))+\\S)(\\.gif|\\.jpg|\\.jpeg|\\.png)", new MatchEvaluator(Image), RegexOptions.IgnoreCase);

         // links
         text = Regex.Replace(text, "\\[\\[(.*?)\\]\\]", new MatchEvaluator(WikipediaLink), RegexOptions.IgnoreCase | RegexOptions.Singleline);


         text = Regex.Replace(text, "(''''')(.*?)\\1", "<i><b>$2</b></i>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
         // emphasis ''' -> <b>
         text = Regex.Replace(text, "(''')(.*?)\\1", "<b>$2</b>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

         // emphasis '' -> <i>
         text = Regex.Replace(text, "('')(.*?)\\1", "<i>$2</i>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

         // line breaks
         text = text.Replace("\n", "<br/>");
         text = Regex.Replace(text, "<br/><pre>", "<pre>", RegexOptions.IgnoreCase);

         // small
         text = Regex.Replace(text, "&lt;small&gt;(.*?)&lt;/small&gt;", "<font size=\"1\">$1</font>", RegexOptions.IgnoreCase  | RegexOptions.Singleline);

         // replace HTML tags
         text = Regex.Replace(text, "&lt;br&gt;", "<br/>", RegexOptions.IgnoreCase);
         text = Regex.Replace(text, "&lt;br/&gt;", "<br/>", RegexOptions.IgnoreCase);
         text = Regex.Replace(text, "&lt;b&gt;", "<b>", RegexOptions.IgnoreCase);
         text = Regex.Replace(text, "&lt;/b&gt;", "</b>", RegexOptions.IgnoreCase);
         text = Regex.Replace(text, "&lt;u&gt;", "<u>", RegexOptions.IgnoreCase);
         text = Regex.Replace(text, "&lt;/u&gt;", "</u>", RegexOptions.IgnoreCase);
         text = Regex.Replace(text, "&lt;i&gt;", "<i>", RegexOptions.IgnoreCase);
         text = Regex.Replace(text, "&lt;/i&gt;", "</i>", RegexOptions.IgnoreCase);
         text = Regex.Replace(text, "&lt;center&gt;", "<center>", RegexOptions.IgnoreCase);
         text = Regex.Replace(text, "&lt;/center&gt;", "</center>", RegexOptions.IgnoreCase);
         text = Regex.Replace(text, "&lt;sub&gt;", "<sub>", RegexOptions.IgnoreCase);
         text = Regex.Replace(text, "&lt;/sub&gt;", "</sub>", RegexOptions.IgnoreCase);
         text = Regex.Replace(text, "&lt;sup&gt;", "<sup>", RegexOptions.IgnoreCase);
         text = Regex.Replace(text, "&lt;/sup&gt;", "</sup>", RegexOptions.IgnoreCase);
         text = Regex.Replace(text, "&lt;tt&gt;", "<tt>", RegexOptions.IgnoreCase);
         text = Regex.Replace(text, "&lt;/tt&gt;", "</tt>", RegexOptions.IgnoreCase);
         text = Regex.Replace(text, "&lt;strike&gt;", "<strike>", RegexOptions.IgnoreCase);
         text = Regex.Replace(text, "&lt;/strike&gt;", "</strike>", RegexOptions.IgnoreCase);

         return text;
      }
   }
}
