namespace EasyGrid.Grid
{
    public class DataColumnOptions
    {
        public DataColumnOptions()
        {
            Sortable = true;
            SortField = "";
            Filterable = true;
            FilterField = "";
            Visible = true;
            DataColumnType = DataColumnTypes.BoundColumn;
            EditorType = DataColumnEditorTypes.Label;
            DataFormat = string.Empty;
        }

        public DataColumnOptions(string fieldName)
        {
            FieldName = fieldName;
            Sortable = true;
            SortField = "";
            Filterable = true;
            FilterField = "";
            Visible = true;
            DataColumnType = DataColumnTypes.BoundColumn;
            EditorType = DataColumnEditorTypes.Label;
            DataFormat = string.Empty;
        }

        public string FieldName { get; set; }

        public string Caption { get; set; }

        public string Format { get; set; }

        public int Width { get; set; }

        public bool Sortable { get; set; }

        public string SortField { get; set; }

        public bool Filterable { get; set; }

        public string FilterField { get; set; }

        public string FilterValue { get; set; }

        public bool Visible { get; set; }

        public DataColumnTypes DataColumnType { get; set; }

        public DataColumnEditorTypes EditorType { get; set; }

        public string DataFormat { get; set; }
    }
}
