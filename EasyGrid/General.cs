using System;
using System.Collections.Specialized;
using System.Web.Routing;

namespace EasyGrid
{
    public class LibGeneral
    {
        private static int _pageSize = 10;
        public static int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = value;
            }
        }

        public const string EditButtonText = "Edit";
        public const string DeleteButtonText = "Delete";
        public const string AskDeleteText = " confirm delete?";
        public const string NewButtonText = "New";

        public static string GetWherePredicate(Type type, NameValueCollection queryString)
        {
            var wherePredicate = "1==1";
            foreach (var key in queryString.AllKeys)
            {
                if (key != null && key.StartsWith("s_"))
                {
                    if (string.IsNullOrWhiteSpace(queryString[key]))
                        continue;

                    var field = key.Replace("__", ".").Replace("s_", "");
                    var fieldType = Reflection.GetType(type, field);
                    if (fieldType == typeof(string))
                    {
                        wherePredicate = string.Format("{0}.ToLower().Contains(\"{1}\")",
                         field, queryString[key].ToLower());
                    }
                    else if (fieldType == typeof(int) || fieldType == typeof(int?))
                    {
                        wherePredicate = string.Format("{0} == {1}",
                        field, queryString[key]);
                    }
                }
            }
            return wherePredicate;
        }

        public static string JsAlert(string text)
        {
            return "alert(\"" + text.Replace("\r\n", "") + "\");";
        }

        public static string GetContentUrl(RouteData route)
        {
            var root = string.Empty;
            if (route.DataTokens["area"] != null)
                root = route.DataTokens["area"].ToString();
            if (route.Values["controller"] != null)
            {
                if (!string.IsNullOrWhiteSpace(root))
                    root += "/";
                root += (string) route.Values["controller"];
            }
            return root;
        }

    }
}
