using System.Drawing;

namespace EasyGrid.Grid
{
    public class ImageColumnOptions : DataColumnOptions
    {
        public ImageColumnOptions()
        {
            EditorType = DataColumnEditorTypes.Image;
            ImageSize = new Size(0, 0);
        }

        /// <summary>
        /// EditorTyope image olan alanlar için resim boyuyunu belirler.
        /// </summary>
        public Size ImageSize { get; set; }

        public string ImageUrlFormat { get; set; }
    }
}
