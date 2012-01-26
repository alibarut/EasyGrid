using System;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace EasyGrid.Grid
{
    class ValueConverter
    {
        public static string GetFieldValue(object item, string fieldName)
        {
            var value = Reflection.GetObjectValue(item, fieldName) ?? String.Empty;
            return value.ToString();
        }

        public static object InspectDataFormat(object item, DataColumnOptions field, string dataFormat = null)
        {
            if (string.IsNullOrWhiteSpace(dataFormat))
                dataFormat = field.DataFormat;
            if (string.IsNullOrWhiteSpace(dataFormat))
            {
                var fieldValue = Reflection.GetObjectValue(item, field.FieldName);
                return fieldValue != null ? fieldValue : string.Empty;
            }
            var val = dataFormat;
            const string regex = @"{(\w+.\w+)}";
            var ma = Regex.Matches(dataFormat, regex, RegexOptions.IgnoreCase);
            if (ma.Count > 0)
            {
                foreach (Match m in ma)
                    if (m.Success)
                    {
                        var fieldName = m.Groups[1].Value;
                        var fieldValue = Reflection.GetObjectValue(item, fieldName);
                        var fv = fieldValue != null ? fieldValue.ToString() : string.Empty;
                        val = val.Replace(m.ToString(), fv);
                    }
            }
            return val;
        }

        public static string GetValue(HtmlHelper helper, object item, DataColumnOptions field)
        {
            var r = string.Empty;

            if (field.EditorType == DataColumnEditorTypes.Image)
            {
                var col = field as ImageColumnOptions;

                var valUrl = InspectDataFormat(item, field, col.ImageUrlFormat);
                if (valUrl == null)
                    return r;

                r += "<img src='" + valUrl + "'";
                if (col.ImageSize.Height > 0)
                    r += " height='" + helper.AttributeEncode(col.ImageSize.Height.ToString()) + "'";
                if (col.ImageSize.Width > 0)
                    r += " width='" + helper.AttributeEncode(col.ImageSize.Width.ToString()) + "'";
                r += "/>";
            }
            else if (field.EditorType == DataColumnEditorTypes.Link)
            {
                var col = field as LinkColumnOptions;
                var valUrl = InspectDataFormat(item, field, col.NavigateUrlFormat);
                if (valUrl == null)
                    return r;

                r += "<a href='" + valUrl + "'";
                if (!string.IsNullOrWhiteSpace(col.Target))
                    r += " target='" + col.Target + "'";
                r += ">" + col.Caption + "</a>";
            }
            else // LABEL VE HTMLLABEL
            {
                var val = InspectDataFormat(item, field);
                if (val == null)
                    return r;

                if (val.GetType().FullName == "System.Boolean")
                {
                    bool bVal;
                    if (bool.TryParse(val.ToString(), out bVal))
                    {
                        r += "<input type='checkbox' disabled='disabled' value='" + bVal + "'";
                        if (bVal)
                            r += " checked='checked'";
                        r += "/>";
                    }
                    else r = helper.Encode(val);
                }
                else if (val.GetType().FullName.IndexOf("System.DateTime") > -1)
                {
                    DateTime bVal;
                    r = helper.Encode(DateTime.TryParse(val.ToString(), out bVal) ? bVal.ToString(field.Format) : val);
                }
                else if (val.GetType().FullName.IndexOf("System.Int32") > -1)
                {
                    int bVal;
                    r = helper.Encode(int.TryParse(val.ToString(), out bVal) ? bVal.ToString(field.Format) : val);
                }
                else if (val.GetType().FullName.IndexOf("System.Decimal") > -1)
                {
                    decimal bVal;
                    r = helper.Encode(decimal.TryParse(val.ToString(), out bVal) ? bVal.ToString(field.Format) : val);
                }
                else
                {
                    r = field.EditorType == DataColumnEditorTypes.Label ? helper.Encode(val.ToString()) : val.ToString();
                }
            }
            return r;
        }
    }
}
