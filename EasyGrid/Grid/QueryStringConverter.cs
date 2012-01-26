using System.Collections.Specialized;
using System.Linq;

namespace EasyGrid.Grid
{
    class QueryStringConverter
    {
        public static NameValueCollection RemoveFromQueryString(NameValueCollection q, string removeKey)
        {
            if (string.IsNullOrEmpty(removeKey))
                return q;

            var r = new NameValueCollection();
            foreach (var i in q.AllKeys)
            {
                if (i != null && i.ToLowerInvariant() != removeKey.ToLowerInvariant())
                    r.Add(i, q[i]);
            }
            return r;
        }

        public static string RemoveFromQueryString(NameValueCollection q)
        {
            var s = string.Empty;
            foreach (var i in q.AllKeys)
            {
                if (!string.IsNullOrWhiteSpace(s))
                    s += "&";
                s += i + "=" + q[i];
            }
            return s;
        }

        public static string RemoveFromQueryStringWhere(NameValueCollection q)
        {
            var s = string.Empty;
            foreach (var i in q.AllKeys.Where(d => !d.StartsWith("s_")))
            {
                if (!string.IsNullOrWhiteSpace(s))
                    s += "&";
                s += i + "=" + q[i];
            }
            return s;
        }
    }
}
