using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WpfApp1.Utils
{
    public class PathUtils
    {

        public static string GetImagePath(string imageName, ExtentionType extention)
        {
            return string.Format("{0}\\Photos\\" + imageName + ".{1}", AppDomain.CurrentDomain.BaseDirectory, extention.ToString());
        }

        public static string GetVideoPath(string videoName, string extention)
        {
            Debug.WriteLine(string.Format("{0}Video\\" + videoName + ".{1}", AppDomain.CurrentDomain.BaseDirectory, extention.ToString()));
            return string.Format("{0}Video\\" + videoName + ".{1}", AppDomain.CurrentDomain.BaseDirectory, extention.ToString());
        }

        public static string GetXMLPath(string xmlName)
        {
            Debug.WriteLine(string.Format("{0}Classifier\\" + xmlName + ".xml", AppDomain.CurrentDomain.BaseDirectory));
            return string.Format("{0}Classifier\\" + xmlName + ".xml", AppDomain.CurrentDomain.BaseDirectory);
        }
    }
    public enum ExtentionType
    {
        jpg,
        png
    }
}
