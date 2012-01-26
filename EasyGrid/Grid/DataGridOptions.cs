using System.Collections.Generic;
using System.Drawing;
using System.Web.Mvc;

namespace EasyGrid.Grid
{
    public class DataGridOptions
    {
        public DataGridOptions()
        {
            CssTable = "grid";

            EmptyData = "<div>EMPTY DATA</div>";

            Sortable = true;
            SortDirKeyword = "sortdir";
            SortKeyword = "sort";
            SortDefaultDirection = "ASC";

            PagerAndShortAction = "ListView";
            PagerJsFunction = "LoadListView";
            PagerKeyowrd = "pageindex";
            PageSizeKeyword = "pagesize";
            PagerEnabled = true;

            ButtonDeleteEnabled = true;
            ButtonDeleteAction = "delete";
            ButtonDeleteCss = "button icon remove danger";

            ButtonEditEnabled = true;
            ButtonEditAction = "edit";
            EditPopup = true;
            ButtonEditCss = "button icon edit";
            ButtonEditPopupCss = "button icon edit fancybox iframe";

            ButtonInsertAction = "create";
            ButtonInsertEnabled = true;
            InsertPopup = true;
            ButtonInsertCss = "button icon add";
            ButtonInsertPopupCss = "button icon add fancybox iframe";

            ButtonColumnWidth = 150;
            PopupSize = new Size(700,450);
            Buttons = new List<MvcHtmlString>();

            FilteringEnabled = true;
        }

        public string CssTable{ get; set; }

        public string PagerAndShortAction { get; set; }

        public string PagerJsFunction { get; set; }

        public string PagerKeyowrd { get; set; }

        public string PageSizeKeyword { get; set; }

        public bool PagerEnabled { get; set; }

        public bool Sortable { get; set; }

        public string SortKeyword { get; set; }

        public string SortDirKeyword { get; set; }

        public string SortDefaultFieldName { get; set; }

        public string SortDefaultDirection { get; set; }

        public bool ButtonInsertEnabled { get; set; }

        public string ButtonInsertAction { get; set; }

        public string ButtonInsertCss { get; set; }

        public string ButtonInsertPopupCss { get; set; }

        public bool InsertPopup { get; set; }

        public bool ButtonEditEnabled { get; set; }

        public string ButtonEditAction { get; set; }

        public string ButtonEditCss { get; set; }

        public bool EditPopup { get; set; }

        public string ButtonEditPopupCss { get; set; }

        public bool ButtonDeleteEnabled { get; set; }

        public string ButtonDeleteAction { get; set; }

        public string ButtonDeleteCss { get; set; }

        public int ButtonColumnWidth { get; set; }

        public Size PopupSize { get; set; }

        public List<MvcHtmlString> Buttons { get; set; }

        public string KeyField { get; set; }

        public string TextField { get; set; }

        public bool FilteringEnabled { get; set; }

        public string EmptyData { get; set; }
    }
}
