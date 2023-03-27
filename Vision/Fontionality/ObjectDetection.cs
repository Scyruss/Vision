using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Utils;

namespace WpfApp1.Fontionality
{
    public class ObjectDetection
    {
        public Mat ComputeFrame(Mat colorFrame, Mat grayFrame, Mat lastgrayFrame,ObjectDetectionProfil profil)
        {
            Mat frameDiff = new Mat();
            Cv2.Absdiff(lastgrayFrame, grayFrame, frameDiff);


            Mat kernel = ImageProcessing.GetMatrix(profil.MatrixSize,profil.MatrixSize);
            kernel.SetTo(new Scalar(255, 255, 255));

            Cv2.Dilate(frameDiff, frameDiff, kernel);

            Cv2.Threshold(frameDiff,frameDiff,profil.Thresh,profil.MaxVal, ThresholdTypes.Binary);

            Cv2.FindContours(frameDiff,out Mat[] contours,new Mat(),RetrievalModes.External,ContourApproximationModes.ApproxSimple);

            if(profil.ShowObjects){
                foreach (var contour in contours)
                {
                    if (Cv2.ContourArea(contour) < profil.MinSize)
                        continue;

                    Rect rect = Cv2.BoundingRect(contour);
                    Cv2.Rectangle(colorFrame, rect, profil.RectangleColor);
                }
            } else if (profil.ShowMovements)
                return frameDiff;
            

            return colorFrame;
        }
    }

    public class ObjectDetectionProfil
    {
        public ObjectDetectionProfil(double thresh, double maxVal, double minSize, int matrixSize, Scalar rectangleColor, bool showMovements, bool showObjects)
        {
            Thresh = thresh;
            MaxVal = maxVal;
            MinSize = minSize;
            MatrixSize = matrixSize;
            RectangleColor = rectangleColor;
            ShowMovements = showMovements;
            ShowObjects = showObjects;
        }

        public double Thresh { get; }
        public double MaxVal { get; }
        public double MinSize { get; }
        public int MatrixSize { get; }
        public Scalar RectangleColor { get; }
        public bool ShowMovements { get; }
        public bool ShowObjects { get; }
    }
}
