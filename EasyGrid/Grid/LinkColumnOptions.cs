namespace EasyGrid.Grid
{
    public class LinkColumnOptions : DataColumnOptions
    {
        public LinkColumnOptions()
        {
            EditorType = DataColumnEditorTypes.Link;
            Target = string.Empty;
        }
        public string Target { get; set; }

        public string NavigateUrlFormat { get; set; }
    }
}
