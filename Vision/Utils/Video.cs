using OpenCvSharp;
using System.Diagnostics;

namespace WpfApp1.Utils
{
    internal class Video
    {
        public const string imagePath = "";
        public static VideoCapture GetVideo(string name, string extention)
        {
            string f = PathUtils.GetVideoPath(name, extention);
            var v = new VideoCapture(f);


            return v;
        }

    }
}