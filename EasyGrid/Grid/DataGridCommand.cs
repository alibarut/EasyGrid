namespace EasyGrid.Grid
{
    public class DataGridCommand
    {
        public DataGridCommand()
        {
            Ajax = false;
            HttpMethod = "POST";
        }
        public bool Ajax { get; set; }
        public string HttpMethod { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Confirm { get; set; }
        public string Css { get; set; }
    }
}
