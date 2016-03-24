namespace EmailMarketing.SalesLogix
{
    using System;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Utility methods for helping with SalesLogix SData connections
    /// </summary>
    public class SdataHelper
    {
        /// <summary>
        /// Append the required segments to the end of an sdata url ("/slx/dynamic/-") if they are missing
        /// </summary>
        /// <param name="sdataUrl">The sdata url to append to</param>
        /// <returns>The 'fixed' sdata url</returns>
        public string AppendRequiredUrlSegments(string sdataUrl)
        {
            Uri parsedUri;
            if (!Uri.TryCreate(sdataUrl, UriKind.Absolute, out parsedUri))
            {
                throw new ArgumentException("Not a valid URL", sdataUrl);
            }

            UriBuilder builder = new UriBuilder(parsedUri);

            string lastSegment = parsedUri.Segments.Last().Trim('/');
            if (lastSegment.Equals("-", StringComparison.OrdinalIgnoreCase))
            {
                // URL is complete
                return sdataUrl;
            }
            else if (lastSegment.Equals("dynamic", StringComparison.OrdinalIgnoreCase))
            {
                // Add last segment
                builder.Path = Path.Combine(builder.Path, "-");
            }
            else if (lastSegment.Equals("slx", StringComparison.OrdinalIgnoreCase))
            {
                // Add last 2 segments
                builder.Path = Path.Combine(builder.Path, "dynamic/-");
            }
            else
            {
                // Add the whole of the missing segments
                builder.Path = Path.Combine(builder.Path, "slx/dynamic/-");
            }

            return builder.ToString();
        }
    }
}