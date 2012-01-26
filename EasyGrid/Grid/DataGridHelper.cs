using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using System.IO;
using EasyGrid.Dynamic;
using PagedList;
using PagedList.Mvc;
using HtmlHelper = System.Web.Mvc.HtmlHelper;

namespace EasyGrid.Grid
{
    public static class DataGridHelper
    {
        private static int _colCount;
        private static NameValueCollection _query;

        public static MvcHtmlString DataGrid<T>(this HtmlHelper helper, IEnumerable<T> data, DataColumnOptions[] columns, DataGridOptions option)
            where T : class
        {
            var items = data;
            if (items == null)
            {
                var writer2 = new HtmlTextWriter(new StringWriter());
                writer2.Write(option.EmptyData);
                return new MvcHtmlString(writer2.InnerWriter.ToString());
            }

            _query = helper.ViewContext.HttpContext.Request.QueryString;

            #region FILTER DATA
            var kp = LibGeneral.GetWherePredicate(typeof(T), helper.ViewContext.HttpContext.Request.QueryString);
            items = items.AsQueryable().Where(kp);
            #endregion

            #region SORT DATA

            var currentSort = _query[option.SortKeyword];
            var currentsortDir = _query[option.SortDirKeyword];

            if (string.IsNullOrWhiteSpace(option.SortDefaultFieldName))
                option.SortDefaultFieldName = option.KeyField;
            if (string.IsNullOrWhiteSpace(currentSort))
                currentSort = string.IsNullOrWhiteSpace(option.SortDefaultFieldName) ? option.KeyField : option.SortDefaultFieldName;

            if (string.IsNullOrWhiteSpace(currentsortDir))
                currentsortDir = option.SortDefaultDirection;

            if (option.Sortable && !(data is PagedList<T>) && !(data is StaticPagedList<T>))
            {
                if (!string.IsNullOrWhiteSpace(currentSort))
                    items = items.AsQueryable().OrderBy(currentSort + " " + currentsortDir);
            }
            #endregion

            #region DO PAGING
            if (option.PagerEnabled && !(data is PagedList<T>) && !(data is StaticPagedList<T>))
            {
                var currentPageIndex = _query[option.PagerKeyowrd];
                var pageIndex = 1;
                if (!string.IsNullOrWhiteSpace(currentPageIndex))
                    int.TryParse(currentPageIndex, out pageIndex);
                var currentPageSize = _query[option.PageSizeKeyword];
                var pageSize = 10;
                if (!string.IsNullOrWhiteSpace(currentPageSize))
                    int.TryParse(currentPageSize, out pageSize);
                items = new PagedList<T>(items, pageIndex, pageSize);
            }
            #endregion

            var writer = new HtmlTextWriter(new StringWriter());

            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            writer.AddAttribute("class", option.CssTable);
            writer.RenderBeginTag(HtmlTextWriterTag.Table);

            writer.RenderBeginTag(HtmlTextWriterTag.Thead);
            RenderHeader<T>(helper, writer, columns, option, currentSort, currentsortDir);

            if (option.FilteringEnabled)
                RenderFilterRow<T>(writer, columns);

            writer.RenderEndTag();

            writer.RenderBeginTag(HtmlTextWriterTag.Tbody);
            foreach (var item in items)
                RenderRow(helper, writer, columns, item, option);
            writer.RenderEndTag();

            if (option.PagerEnabled)
            {
                if (data is PagedList<T> || data is StaticPagedList<T>)
                    RenderPager(helper, writer, data, option);
                else
                    RenderPager(helper, writer, items, option);
            }

            if (option.FilteringEnabled && columns.Count(d => !string.IsNullOrWhiteSpace(d.FilterValue)) > 0)
            {
                RenderFilterRowFooter<T>(helper, writer, columns, option);
            }
            writer.RenderEndTag(); //TABLE
            writer.RenderEndTag(); // DIV
            RenderScript(helper, writer, option);

            return new MvcHtmlString(writer.InnerWriter.ToString());
        }

        private static void RenderHeader<T>(HtmlHelper helper, HtmlTextWriter writer,
            IEnumerable<DataColumnOptions> columns, DataGridOptions option, string currentSort,
            string currentsortDir)
        {
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);

            if ((option.ButtonDeleteEnabled || option.ButtonEditEnabled || option.Buttons.Count > 0))
            {
                writer.AddAttribute("width", option.ButtonColumnWidth.ToString());
                writer.AddAttribute("nowrap", "nowrap");
                writer.RenderBeginTag(HtmlTextWriterTag.Th);
                writer.Write("<table width='" + option.ButtonColumnWidth.ToString() + "'><tr>");
                if (option.ButtonInsertEnabled)
                {
                    var buttonClass = option.InsertPopup ? option.ButtonInsertPopupCss : option.ButtonInsertCss;
                    writer.Write(string.Format("<td><a href='{0}/{1}' class='{2}'>{3}</a></td>",
                        UrlHelper.GenerateContentUrl("~/" + LibGeneral.GetContentUrl(helper.ViewContext.RouteData), helper.ViewContext.HttpContext),
                        option.ButtonInsertAction, buttonClass, LibGeneral.NewButtonText));
                }
                writer.Write("</tr></table>");
                writer.Write(helper.Encode(" "));
                writer.RenderEndTag();
                _colCount++;
            }

            var q = QueryStringConverter.RemoveFromQueryString(_query, option.SortKeyword);
            q = QueryStringConverter.RemoveFromQueryString(q, option.SortDirKeyword);
            var qstr = QueryStringConverter.RemoveFromQueryString(q);

            foreach (var field in columns.Where(d => d.Visible))
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Th);
                if (field.DataColumnType == DataColumnTypes.BoundColumn)
                {
                    if (string.IsNullOrWhiteSpace(field.Caption))
                        field.Caption = Reflection.GetDisplayName(typeof(T), field.FieldName);
                    writer.Write("<table><tr>");
                    if (option.Sortable && field.Sortable)
                    {
                        var sortDir = "ASC";
                        var sortImage = string.Empty;
                        if (currentSort == field.FieldName)
                        {
                            sortImage = "<img src='" + UrlHelper.GenerateContentUrl(@"~/Content/EasyGrid/images/sort-asc.png", helper.ViewContext.HttpContext) + "' />";
                            if (currentsortDir == "ASC")
                            {
                                sortDir = "DESC";
                                sortImage = "<img src='" + UrlHelper.GenerateContentUrl(@"~/Content/EasyGrid/images/sort-desc.png", helper.ViewContext.HttpContext) + "' />";
                            }
                        }
                        if ((currentSort == field.FieldName || currentSort == field.SortField) && currentsortDir == "ASC") sortDir = "DESC";

                        var url = UrlHelper.GenerateContentUrl(string.Format(@"~/{0}/{1}?{2}={3}&{4}={5}&{6}",
                                LibGeneral.GetContentUrl(helper.ViewContext.RouteData),
                                option.PagerAndShortAction, option.SortKeyword,
                                string.IsNullOrEmpty(field.SortField) ? field.FieldName : field.SortField,
                                option.SortDirKeyword, sortDir, qstr),
                            helper.ViewContext.HttpContext);

                        writer.Write(string.Format("<td><a href=\"javascript:{0}('{1}');\">{2}</a></td>",
                                option.PagerJsFunction, url, field.Caption));
                        if (!string.IsNullOrWhiteSpace(sortImage))
                            writer.Write(string.Format("<td>{0}</td>", sortImage));
                    }
                    else
                        writer.Write(string.Format("<td>{0}</td>", field.Caption));
                    if (option.FilteringEnabled && field.Filterable)
                    {
                        var fReplace = string.IsNullOrWhiteSpace(field.FilterField) ? field.FieldName.Replace(".", "__") : field.FilterField.Replace(".", "__");
                        writer.Write(string.Format("<td><a href='#p_{0}' class='fancybox'><img src='" +
                            UrlHelper.GenerateContentUrl(@"~/Content/EasyGrid/images/filter.png", helper.ViewContext.HttpContext)
                            + "' /></a></td>", fReplace));
                    }
                    writer.Write("</tr></table>");
                }
                else writer.Write(field.Caption);
                _colCount++;
                writer.RenderEndTag();
            }
            writer.RenderEndTag();
        }

        private static void RenderFilterRow<T>(HtmlTextWriter writer, IEnumerable<DataColumnOptions> columns)
        {
            foreach (var field in columns.Where(d => d.Visible))
            {
                var fieldType = string.IsNullOrWhiteSpace(field.FilterField) ? Reflection.GetType(typeof(T), field.FieldName) : Reflection.GetType(typeof(T), field.FilterField);
                if (fieldType == typeof(string) || fieldType == typeof(int))
                {
                    var fReplace = string.IsNullOrWhiteSpace(field.FilterField) ? field.FieldName.Replace(".", "__") : field.FilterField.Replace(".", "__");
                    field.FilterValue = _query["s_" + fReplace];
                    writer.Write(string.Format(@"<div class='popupHide'><div id='p_{0}'>{3}<br />
<input type='search' id='s_{0}' class='txtSearch {2}' value='{1}' /> 
<a id='b_{0}' class='button icon search btnSearch'>Ara</a>
</div></div>",
                        fReplace, field.FilterValue, fieldType == typeof(int) ? "mask-int" : "", field.Caption));
                }
            }
        }

        private static void RenderFilterRowFooter<T>(HtmlHelper helper, HtmlTextWriter writer, IEnumerable<DataColumnOptions> columns, DataGridOptions option)
        {
            var param = QueryStringConverter.RemoveFromQueryStringWhere(QueryStringConverter.RemoveFromQueryString(_query, option.PagerKeyowrd));
            var loader = string.Format("javascript:{0}('/{1}/{2}?{3}');",
                option.PagerJsFunction, LibGeneral.GetContentUrl(helper.ViewContext.RouteData),
                option.PagerAndShortAction, param);

            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            writer.AddAttribute("colspan", _colCount.ToString());
            writer.AddAttribute("class", "filterRow");
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write(string.Format(@"<table width='100%'><tr>
				<td width='20px'><a href=""{0}""><img src='" + UrlHelper.GenerateContentUrl(@"~/Content/EasyGrid/images/clear.png", helper.ViewContext.HttpContext)
                + "' alt='Clear Filter' /></a></td><td>Filter: ", loader));
            var ilk = true;
            foreach (var field in columns.Where(d => !string.IsNullOrWhiteSpace(d.FilterValue)))
            {
                if (!ilk)
                    writer.Write(", ");
                writer.Write(string.Format("{0}='{1}'", field.Caption, field.FilterValue));
                ilk = false;
            }
            writer.Write("</td></tr></table>");
            writer.RenderEndTag();
            writer.RenderEndTag();
        }

        private static void RenderRowButtons(HtmlHelper helper, HtmlTextWriter write, object item, DataGridOptions option)
        {
            if (!option.ButtonDeleteEnabled && !option.ButtonEditEnabled && option.Buttons.Count <= 0) return;
            write.AddAttribute("class", "button-group");
            write.RenderBeginTag(HtmlTextWriterTag.Td);

            var keyFieldValue = ValueConverter.GetFieldValue(item, option.KeyField);
            var keyText = ValueConverter.GetFieldValue(item, option.TextField);

            foreach (var button in option.Buttons)
                write.Write(button.ToHtmlString().Replace("KEYFIELD", keyFieldValue));

            if (option.ButtonEditEnabled)
            {
                var buttonClass = option.EditPopup ? option.ButtonEditPopupCss : option.ButtonEditCss;
                var url = UrlHelper.GenerateContentUrl(string.Format(@"~/{0}/{1}/{2}",
                    LibGeneral.GetContentUrl(helper.ViewContext.RouteData), option.ButtonEditAction, keyFieldValue),
                    helper.ViewContext.HttpContext);
                write.Write(string.Format("<a href='{0}' class='{1}'>{2}</a>", url, buttonClass, LibGeneral.EditButtonText));
            }

            if (option.ButtonDeleteEnabled)
            {
                var url = UrlHelper.GenerateContentUrl(string.Format(@"~/{0}/{1}/{2}",
                    LibGeneral.GetContentUrl(helper.ViewContext.RouteData), option.ButtonDeleteAction, keyFieldValue),
                    helper.ViewContext.HttpContext);
                write.Write(string.Format("<a href='{0}' class='{1}' data-ajax='true' data-ajax-method='Post' data-ajax-confirm='{2}'>{3}</a>",
                        url, option.ButtonDeleteCss, keyText + " " + LibGeneral.AskDeleteText, LibGeneral.DeleteButtonText));
            }
            write.RenderEndTag();
        }

        private static void RenderRowButtons(HtmlTextWriter write, IEnumerable<DataGridCommand> commands)
        {
            if (commands == null || commands.Count() == 0)
            {
                write.AddAttribute("class", "button-group");
                write.RenderBeginTag(HtmlTextWriterTag.Td);
                write.RenderEndTag();
                return;
            }

            write.AddAttribute("class", "button-group");
            write.RenderBeginTag(HtmlTextWriterTag.Td);

            foreach (var command in commands)
            {
                if (command.Ajax)
                    write.Write(string.Format("<a href='{0}' class='{1}' data-ajax='true' data-ajax-method='{2}' data-ajax-confirm='{3}'>{4}</a>",
                                      command.Url, command.Css, command.HttpMethod, command.Confirm, command.Title));
                else
                    write.Write(string.Format("<a href='{0}' class='{1}'>{2}</a>",
                                      command.Url, command.Css, command.Title));
            }
            write.RenderEndTag();
        }

        private static void RenderRow(HtmlHelper helper, HtmlTextWriter write, DataColumnOptions[] columns, object item, DataGridOptions option)
        {
            write.RenderBeginTag(HtmlTextWriterTag.Tr);

            RenderRowButtons(helper, write, item, option);
            foreach (var column in columns.Where(d => d.Visible && d.DataColumnType == DataColumnTypes.CommandColumn))
            {
                var val = Reflection.GetObjectValue(item, column.FieldName);
                if (val is List<DataGridCommand>)
                    RenderRowButtons(write, val as List<DataGridCommand>);
            }

            foreach (var field in columns.Where(d => d.Visible && d.DataColumnType == DataColumnTypes.BoundColumn))
            {
                if (field.Width > 0)
                    write.AddAttribute("width", field.Width.ToString());
                write.RenderBeginTag(HtmlTextWriterTag.Td);
                write.Write(ValueConverter.GetValue(helper, item, field));
                write.RenderEndTag();
            }
            write.RenderEndTag();
        }

        private static void RenderPager<T>(HtmlHelper helper, HtmlTextWriter writer, IEnumerable<T> items, DataGridOptions option)
            where T : class
        {
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            writer.AddAttribute("colspan", _colCount.ToString());
            writer.RenderBeginTag(HtmlTextWriterTag.Td);

            var pagedList = (IPagedList<T>)items;
            var currentPageSize = _query[option.PageSizeKeyword];
            int pageSize = 10;
            if (!string.IsNullOrWhiteSpace(currentPageSize))
                int.TryParse(currentPageSize, out pageSize);
            var q = QueryStringConverter.RemoveFromQueryString(QueryStringConverter.RemoveFromQueryString(_query, option.PagerKeyowrd));
            if (pagedList != null)
            {
                writer.Write(PagedList.Mvc.HtmlHelper.PagedListPager(helper, pagedList,
                    page => string.Format("javascript:{0}('{1}');", option.PagerJsFunction,
                        UrlHelper.GenerateContentUrl(string.Format(@"~/{0}/{1}?{2}={3}&{4}",
                        LibGeneral.GetContentUrl(helper.ViewContext.RouteData),
                        option.PagerAndShortAction, option.PagerKeyowrd, page, q), helper.ViewContext.HttpContext)),
                    new PagedListRenderOptions { DisplayItemSliceAndTotal = true /*, PageSize = pageSize*/ }));
            }
            writer.RenderEndTag();
            writer.RenderEndTag();
        }

        private static void RenderScript(HtmlHelper helper, HtmlTextWriter write, DataGridOptions option)
        {
            var q = QueryStringConverter.RemoveFromQueryString(QueryStringConverter.RemoveFromQueryString(_query, option.PageSizeKeyword));
            var scriptFilter = @"
		$('.btnSearch').click(function () {
				var id = $(this).attr('id').replace('b_', 's_');
				var val = $('#' + id).val();
				##QUERY##
				$.fancybox.close();
			});
		$('.txtSearch').keypress(function (e) {
			if ((e.which && e.which == 13) || (e.keyCode && e.keyCode == 13)) {
				var id = $(this).attr('id');
				var val = $('#' + id).val();
				##QUERY##
				$.fancybox.close();
				return false;
			} else return true;
		});";
            var scriptPager = @"
		$(function () {
			$('#ddPageSize').change(function(){
				##PAGELOADER##
			});
		});";
            var scriptPopup = @"
		$('.fancybox').fancybox({
			##POPUP_SIZE##          
		});";
            var refreshScript = @"
		function refreshPage(){
			##REFRESH_URL##
		};

								";
            var script = @"<script type='text/javascript'>";

            var queryStr = helper.ViewContext.HttpContext.Request.QueryString;

            refreshScript = refreshScript.Replace("##REFRESH_URL##", string.Format("{0}('{1}');", option.PagerJsFunction,
                        UrlHelper.GenerateContentUrl(string.Format(@"~/{0}/{1}?{2}",
                        LibGeneral.GetContentUrl(helper.ViewContext.RouteData),
                        option.PagerAndShortAction, queryStr),
                        helper.ViewContext.HttpContext)));
            script += refreshScript;

            if (option.PagerEnabled)
            {
                scriptPager = scriptPager.Replace("##PAGELOADER##",
                    string.Format("{0}('{1}');", option.PagerJsFunction,
                        UrlHelper.GenerateContentUrl(string.Format(@"~/{0}/{1}?{2}={3}&{4}",
                        LibGeneral.GetContentUrl(helper.ViewContext.RouteData),
                        option.PagerAndShortAction, option.PageSizeKeyword, "'+$(this).val()+'", q),
                        helper.ViewContext.HttpContext)));
                script += scriptPager;
            }
            if (option.EditPopup || option.InsertPopup || option.FilteringEnabled)
            {
                scriptPopup = scriptPopup.Replace("##POPUP_SIZE##",
                                        @"height: " + option.PopupSize.Height.ToString() +
                                        @",width: " + option.PopupSize.Width.ToString() +
                                        @",onComplete: function () {
														$(""#fancybox-content input:first"").focus();
														}");
                script += scriptPopup;
            }
            if (option.FilteringEnabled)
            {
                var param = QueryStringConverter.RemoveFromQueryStringWhere(QueryStringConverter.RemoveFromQueryString(_query, option.PagerKeyowrd));
                scriptFilter = scriptFilter.Replace("##QUERY##",
                   string.Format("{0}('{1});", option.PagerJsFunction,
                       UrlHelper.GenerateContentUrl(string.Format(@"~/{0}/{1}?{2}&'+id+'='+val",
                       LibGeneral.GetContentUrl(helper.ViewContext.RouteData),
                       option.PagerAndShortAction, param),
                       helper.ViewContext.HttpContext)));
                script += scriptFilter;
            }
            script += @"</script>";
            write.Write(script);
        }
    }
}