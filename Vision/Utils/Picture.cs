using OpenCvSharp;


namespace WpfApp1.Utils
{
    internal class Picture
    {
        public const string imagePath = "";
        public static Mat GetImage(string name, ExtentionType extention, ImreadModes type = ImreadModes.Grayscale)
        {
            return Cv2.ImRead(PathUtils.GetImagePath(name, extention), type);
        }

        public static void ShowImage(string name, ExtentionType extention, bool waitKey = false)
        {
            ShowImage(GetImage(name, extention), name, waitKey);
            if (waitKey)
                Cv2.WaitKey(0);
        }


        public static void ShowImage(Mat img, string title, bool waitKey = false)
        {
            Cv2.ImShow(title, img);
            if (waitKey)
                Cv2.WaitKey(0);
        }

    }
}