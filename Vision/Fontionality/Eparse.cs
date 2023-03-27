using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Wpf.Ui.Mvvm.Interfaces;

namespace WpfApp1.Fontionality
{
    internal class Eparse
    {
        public Point2f[] LastCorners { get; private set; }
        public Mat LinesFrame { get; private set; }

        public Mat ComputeFrame(Mat colorFrame, Mat grayFrame, Mat lastGrayFrame, EparseProfil profil)
        {
            Point2f[] corners = Cv2.GoodFeaturesToTrack(grayFrame, profil.MaxCorners, profil.QualityLevel, profil.MinDistance, null, profil.BlockSize, profil.UseHarrisDetector, profil.K);

            if (profil.ShowPoints)
                foreach (Point2f corner in corners)
                    Cv2.Circle(colorFrame, (OpenCvSharp.Point)corner, 3, profil.PointsColor, -1);

            if (profil.ShowLines)
            {
                if (LastCorners != null)
                    ComputeAndApplyPointsToLinesFrame(grayFrame, lastGrayFrame, LastCorners);

                colorFrame = ApplyMaskToMainFrame(colorFrame,profil);
            }

            LastCorners = corners;

            return colorFrame;
        }

        public void ComputeAndApplyPointsToLinesFrame(Mat frameGray, Mat lastFrameGray, Point2f[] lastPoints)
        {
            Point2f[] newPoints = new Point2f[lastPoints.Length];
            // Calculer le mouvement des points
            Cv2.CalcOpticalFlowPyrLK(lastFrameGray, frameGray, lastPoints, ref newPoints, out byte[] status, out float[] error, winSize: new Size(15, 15), maxLevel: 2);

            // Dessiner les points et les lignes de mouvement sur l'image
            for (int i = 0; i < lastPoints.Length; i++)
            {
                Cv2.Line(LinesFrame, (Point)lastPoints[i], (Point)newPoints[i], Scalar.White, 2);
            }
        }

        public Mat ApplyMaskToMainFrame(Mat colorFrame,EparseProfil profil)
        {
            var OneColorFrame = new Mat();
            colorFrame.CopyTo(OneColorFrame);
            OneColorFrame = OneColorFrame.SetTo(profil.LinesColor);
            Debug.WriteLine(colorFrame.Type() + ";" + OneColorFrame.Type() + ";" + LinesFrame.Type()); ;

            Cv2.BitwiseAnd(colorFrame, OneColorFrame, colorFrame, LinesFrame);

            return colorFrame;
        }

        public void InstantiateLinesFrame(Size size)
        {
            LinesFrame = new Mat(size, MatType.CV_8UC1);
            LinesFrame.SetTo(new Scalar(0));
        }
    }

    public class EparseProfil
    {
        public EparseProfil(int maxCorners, double qualityLevel, double minDistance, int blockSize, bool useHarrisDetector, double k, Scalar linesColor, Scalar pointsColor, bool showPoints, bool showLines)
        {
            MaxCorners = maxCorners;
            QualityLevel = qualityLevel;
            MinDistance = minDistance;
            BlockSize = blockSize;
            UseHarrisDetector = useHarrisDetector;
            K = k;
            PointsColor = pointsColor;
            LinesColor = linesColor;
            ShowPoints = showPoints;
            ShowLines = showLines;
        }

        public bool ShowPoints { get; }
        public bool ShowLines { get; }
        public int MaxCorners { get; }
        public double QualityLevel { get; }
        public double MinDistance { get; }
        public int BlockSize { get; }
        public bool UseHarrisDetector { get; }
        public double K { get; }
        public Scalar LinesColor { get; }
        public Scalar PointsColor { get; }
    }
}
