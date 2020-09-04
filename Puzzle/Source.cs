using System.Windows.Media.Imaging;

namespace Puzzle
{
    public class Source
    {
        public int OriginalRow { get; set; }
        public int OriginalCol { get; set; }
        public CroppedBitmap Img { get; set; }
        public bool Taken { get; set; }
    }
}