using System;
using System.Collections.Generic;
using System.Text;
using RazorEngine.Templating;
using RazorEngine.Text;

namespace MedEnthLogsApi.Razor
{
    // Taken From: https://antaris.github.io/RazorEngine/TemplateBasics.html

    /// <summary>
    /// This allows us to return the raw HTML from a file using Html.Raw()
    /// in our cshtml files.
    /// </summary>
    public class RawHtmlHelper
    {
        public IEncodedString Raw( string rawString )
        {
            return new RawString( rawString );
        }
    }

    /// <summary>
    /// This template allows us to return the raw HTML from a file using Html.Raw()
    /// in our cshtml files.
    /// </summary>
    public class RawHtmlTemplate<T> : TemplateBase<T>
    {
        public RawHtmlTemplate()
        {
            Html = new RawHtmlHelper();
        }

        public RawHtmlHelper Html { get; set; }
    }
}
