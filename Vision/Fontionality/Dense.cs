using C1.Chart;
using C1.WPF;
using OpenCvSharp;
using OxyPlot.Series;
using OxyPlot;
using OxyPlot.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Diagnostics;
using System.Windows.Documents;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using C1.Util.DX;
using System.Drawing.Imaging;
using Xceed.Wpf.AvalonDock.Layout;

namespace WpfApp1.Fontionality
{
    internal class Dense
    {

        public Dense() 
        {
            
        }

        public Mat ComputeFrame(Mat colorFrame,Mat grayFrame,Mat lastgrayFrame, out PlotModel angleModel, out PlotModel magnitudeModel, DenceProfil denceProfil)
        {
            Mat flow = new Mat();
            angleModel= new PlotModel();
            magnitudeModel = new PlotModel();
            if (denceProfil.ShowArrow || denceProfil.ShowColor)
            {
                angleModel = new PlotModel { Title = "Angle" };
                magnitudeModel = new PlotModel { Title = "Magnitude" };
                flow = GetDense(grayFrame, colorFrame, lastgrayFrame);

                // visualization
                Mat[] flow_parts = new Mat[2];
                Cv2.Split(flow, out flow_parts);
                //Magnétude
                Mat magnitude = new Mat(), angle = new Mat(), magn_norm = new Mat();
                Cv2.CartToPolar(flow_parts[0], flow_parts[1], magnitude, angle, true);
                Cv2.Normalize(magnitude, magn_norm, 0.0, 1.0, NormTypes.MinMax);

                if (denceProfil.ShowArrow)
                {
                    colorFrame = CalculArrow(flow, colorFrame, denceProfil.ArrowColor);
                }

                if(denceProfil.ShowColor) 
                {
                    colorFrame = ShowColor(magnitude, angle, magn_norm);
                
                }

                angleModel.Series.Add(angle.GetHist(360, 0, 360));

                magnitudeModel.Series.Add(magnitude.GetHist(100, 0, 100));

            }






            ////Affichage
            //float factor = (float)((1.0 / 360.0) * (180.0 / 255.0));
            //Mat new_angle = new Mat();
            //Cv2.Multiply(angle, new Scalar(factor), new_angle);

            //Mat[] _hsv = new Mat[3];
            //Mat hsv = new Mat();
            //Mat hsv8 = new Mat();
            //Mat bgr = new Mat();

            //_hsv[0] = new_angle;
            //_hsv[1] = (Mat.Ones(angle.Size(), MatType.CV_32F));
            //_hsv[2] = magn_norm;

            //Cv2.Merge(_hsv, hsv);
            //hsv.ConvertTo(hsv8, MatType.CV_8U, 255.0);
            //Cv2.CvtColor(hsv8, bgr, ColorConversionCodes.HSV2BGR);

            //return bgr;

            return colorFrame;
        }

        private Mat ShowColor(Mat magnitude, Mat angle, Mat magn_norm)
        {
            //Affichage
            float factor = (float)((1.0 / 360.0) * (180.0 / 255.0));
            Mat new_angle = new Mat();
            Cv2.Multiply(angle, new Scalar(factor), new_angle);

            Mat[] _hsv = new Mat[3];
            Mat hsv = new Mat();
            Mat hsv8 = new Mat();
            Mat bgr = new Mat();

            _hsv[0] = new_angle;
            _hsv[1] = (Mat.Ones(angle.Size(), MatType.CV_32F));
            _hsv[2] = magn_norm;

            Cv2.Merge(_hsv, hsv);
            hsv.ConvertTo(hsv8, MatType.CV_8U, 255.0);
            Cv2.CvtColor(hsv8, bgr, ColorConversionCodes.HSV2BGR);

            return bgr;
        }

        private Mat CalculArrow(Mat flow, Mat colorFrame, Scalar color)
        {
            for (int y = 0; y < flow.Rows; y += 23)
            {
                for (int x = 0; x < flow.Cols; x += 23)
                {
                    Point2f a = flow.At<Point2f>(y, x);

                    Cv2.Line(colorFrame, new Point(x, y), new Point(Math.Round(x + a.X), Math.Round(y + a.Y)), color, 2);
                    Cv2.Circle(colorFrame, new Point(x, y), 1, color, -1);
                }
            }

            return colorFrame;
        }

        public PlotModel ShowHisto(Mat colorFrame)
        {
            Mat[] channels = colorFrame.Split();

            PlotModel model = new PlotModel { Title = "Couleur de l'image" };

            for (int i = 0; i < channels.Length; i++)
            {
                var lineSeries = new LineSeries();

                // Calculate histogram
                Mat hist = new Mat();
                int[] hdims = { 256 }; // Histogram taile de chaque dimension
                Rangef[] ranges = { new Rangef(0, 256), }; // min/max 
                Cv2.CalcHist(
                    new Mat[] { channels[i] },
                    new int[] { 0 },
                    null,
                    hist,
                    1,
                    hdims,
                    ranges);

                //Récupération des valeur pour les graphiques
                for (int j = 0; j < hdims[0]; ++j)
                {
                    lineSeries.Points.Add(new OxyPlot.DataPoint(j, hist.Get<float>(j)));
                }
                switch (i)
                {
                    case 0:
                        lineSeries.Color = OxyColors.Blue;
                        break;
                    case 1:
                        lineSeries.Color = OxyColors.Green;
                        break;
                    case 2:
                        lineSeries.Color = OxyColors.Red;
                        break;
                }

                model.Series.Add(lineSeries);
            }

            return model;

        }

        public Mat GetDense(Mat nextFrameGray, Mat nextFrameColor, Mat lastgrayFrame)
        {
            Mat flow = new(lastgrayFrame.Size(), MatType.CV_32FC2);
            Cv2.CalcOpticalFlowFarneback(lastgrayFrame, nextFrameGray, flow, 0.5, 3, 15, 3, 5, 1.2, 0);

            return flow;
        }

    }
}

public class DenceProfil
{
    public DenceProfil(Scalar pointsColor, bool showArrow, bool showColor)
    {
        ArrowColor = pointsColor;
        ShowArrow = showArrow;
        ShowColor = showColor;
    }
    public Scalar ArrowColor { get; private set; }
    public bool ShowArrow { get; private set; }
    public bool ShowColor { get; private set; }
}