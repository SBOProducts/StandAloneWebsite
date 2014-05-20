using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Customer.Helpers
{
    public static class ActionExtensions
    {
        public static MvcHtmlString ActionButton(this HtmlHelper html, string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes)
        {
            // <a href="/controller/action/id?attr=val">text</a>
            string link = html.ActionLink(linkText, actionName, controllerName, routeValues, htmlAttributes).ToString();

            // get the onclick attribute
            Regex reg = new Regex("href=\"[^\"]+\"");
            string orig = reg.Match(link).Value;
            string href = reg.Match(link).Value;
            href = href.Replace("href", "onclick='javascript:location");
            href = href + ";' ";

            // get the attributes string
            StringBuilder attributes = new StringBuilder();
            if (htmlAttributes != null)
            {
                foreach (var prop in htmlAttributes.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    attributes.AppendFormat("{0}=\"{1}\" ", prop.Name, prop.GetValue(htmlAttributes, null));
                }
            }
            
            // build the input button
            StringBuilder sb = new StringBuilder();
            sb.Append("<input type=\"button\" ");
            sb.AppendFormat("value=\"{0}\" ", linkText);
            sb.Append(attributes.ToString());
            sb.Append(href);
            sb.Append(" />");
         
            return new MvcHtmlString(sb.ToString());
        }

        public static MvcHtmlString GetVirtualPath(this HtmlHelper html, string physicalPath)
        {
            if (string.IsNullOrEmpty(physicalPath))
                return MvcHtmlString.Empty;

            string root = HttpContext.Current.Server.MapPath("~/");
            string prep = physicalPath.Replace(root, "");
            return new MvcHtmlString(prep.Replace('\\', '/'));
        }

        public static string CropString(this HtmlHelper html, string longText, int maxLength)
        {
            if (string.IsNullOrEmpty(longText) || longText.Length <= maxLength)
                return longText;

            string sub = longText.Substring(0, maxLength - 3);
            return sub + "...";

        }
    }
}