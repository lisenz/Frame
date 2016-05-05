using System;

namespace Frame.Core.Utility
{
    public static class UriParsingHelper
    {
        public static string GetQuery(Uri uri)
        {
            return EnsureAbsolute(uri).Query;
        }

        public static string GetAbsolutePath(Uri uri)
        {
            return EnsureAbsolute(uri).AbsolutePath;
        }

        public static UriQuery ParseQuery(Uri uri)
        {
            string query = GetQuery(uri);

            return new UriQuery(query);
        }

        private static Uri EnsureAbsolute(Uri uri)
        {
            if (uri.IsAbsoluteUri)
                return uri;

            if (null != uri && uri.OriginalString.StartsWith("/", StringComparison.Ordinal))
                return new Uri(string.Format("http://localhost/{0}",uri), UriKind.Absolute);

            return new Uri(string.Format("http://localhost{0}", uri), UriKind.Absolute);
        }

    }
}
