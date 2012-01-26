using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace EasyGrid
{
    public static class Reflection
    {
        public static Type GetType(Type type, string propertyName)
        {
            var fields = propertyName.Split('.');
            for (int i = 0; i < fields.Count() - 1; i++)
            {
                var lazyProperty = type.GetProperty(fields[i]);
                if (lazyProperty != null)
                    type = lazyProperty.PropertyType;
            }
            propertyName = fields[fields.Count() - 1];
            if (type != null)
            {
                var prop = type.GetProperty(propertyName);
                return prop != null ? type.GetProperty(propertyName).PropertyType : null;
            }
            return null;
        }

        public static object GetObjectValue(object source, string propertyName)
        {
            var fields = propertyName.Split('.');
            for (int i = 0; i < fields.Count() - 1; i++)
            {
                var lazyProperty = source.GetType().GetProperty(fields[i]);
                if (lazyProperty != null)
                    source = lazyProperty.GetValue(source, null);
            }
            propertyName = fields[fields.Count() - 1];
            if (source != null)
            {
                var property = source.GetType().GetProperty(propertyName);
                return property != null ? property.GetValue(source, null) : string.Empty;
            }
            return string.Empty;
        }

        public static void SetObjectValue(object source, string propertyName, object value)
        {
            var property = source.GetType().GetProperty(propertyName);
            property.SetValue(source, value, null);
        }

        public static string GetDisplayName(Type type, string fieldName)
        {
            var fields = fieldName.Split('.');
            if (fields.Count() > 1)
            {
                for (int i = 0; i < fields.Count() - 1; i++)
                {
                    var lazyProperty = type.GetProperty(fields[i]);
                    if (lazyProperty != null)
                        type = lazyProperty.PropertyType;
                }
                fieldName = fields[fields.Count() - 1];
            }

            var metaAttr = (MetadataTypeAttribute[])type.GetCustomAttributes(typeof(MetadataTypeAttribute), true);
            if (metaAttr.Length > 0)
            {
                foreach (var attr in metaAttr)
                {
                    var t = attr.MetadataClassType;
                    var pi = t.GetProperty(fieldName);
                    if (pi == null)
                        break;
                    foreach (var att in pi.GetCustomAttributes(typeof(DisplayAttribute), false))
                    {
                        var display = att as DisplayAttribute;
                        if (display != null)
                            return display.Name;
                    }
                }
            }
            else
            {
                var pr = type.GetProperty(fieldName);
                if (pr != null)
                    foreach (var att in pr.GetCustomAttributes(false))
                    {
                        var display = att as DisplayAttribute;
                        if (display != null)
                            return display.Name;
                    }
            }

            return fieldName;
        }
    }
}
