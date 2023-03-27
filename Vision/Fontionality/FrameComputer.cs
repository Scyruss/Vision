using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using OxyPlot;
using System.Windows.Media.Imaging;

namespace WpfApp1.Fontionality
{
    internal class FrameComputer
    {
        public Mat? FrameMatColor { get; private set; }
        public Mat? FrameMatGray { get; private set; }
        public Mat? LastFrameMatGray { get; private set; }

        public Eparse Dence { get; private set; }
        public Dense Eparce { get; private set; }
        public ObjectDetection ObjectDetection { get; private set; }
        public CascadeDetection CascadeDetection { get; private set; }

        public MouvementDetection mouvementDetection { get; private set; }

        public PlotModel colorModel;

        public PlotModel denseModel;
        public BitmapSource GetBitmapImage()
        {
            if (FrameMatColor == null) return new BitmapImage();

            return FrameMatColor.ToBitmapSource();
        }

        public Mouvement mouvement;


        public FrameComputer()
        {
            Dence = new Eparse();
            Eparce = new Dense();
            ObjectDetection = new ObjectDetection();
            CascadeDetection = new CascadeDetection();
            mouvementDetection = new MouvementDetection();
        }


        public void ComputeNewFrame(Mat newFrame,bool enableEparce, bool enableDence, bool enableObjectsDetection, bool enableCascadeDetection,EparseProfil eparseProfil,ObjectDetectionProfil detectionProfil, DenceProfil denceProfil)
        {
            if (newFrame == null) return;

            //colorModel = Eparce.ShowHisto(newFrame);

            InitNewFrame(newFrame);

            mouvement = Mouvement.Travelling;

            if (enableEparce && FrameMatColor != null && FrameMatGray != null && LastFrameMatGray != null)
            {
                FrameMatColor = Dence.ComputeFrame(FrameMatColor, FrameMatGray, LastFrameMatGray, eparseProfil);
            }
                
            if (enableDence && FrameMatGray != null && FrameMatColor != null && LastFrameMatGray != null)
            {
                FrameMatColor = Eparce.ComputeFrame(FrameMatColor, FrameMatGray, LastFrameMatGray, out denseModel, out colorModel, denceProfil);
            }
               
            if (enableObjectsDetection && FrameMatGray != null && FrameMatColor != null && LastFrameMatGray != null)
                FrameMatColor = ObjectDetection.ComputeFrame(FrameMatColor, FrameMatGray, LastFrameMatGray, detectionProfil);

            if (enableCascadeDetection && FrameMatColor != null && FrameMatGray != null)
                FrameMatColor = CascadeDetection.DetectCar(FrameMatColor);

            LastFrameMatGray = FrameMatGray;

            OnImageComputed();

            
        }


        private void InitNewFrame(Mat newFrame)
        {
            FrameMatColor = newFrame;
            FrameMatGray = new Mat();
            Cv2.CvtColor(FrameMatColor, FrameMatGray, ColorConversionCodes.BGR2GRAY);
        }


        public virtual void OnImageComputed()
        {

        }
    }

    public enum Mouvement
    {
        Travelling,
        Zoom,
        Rotation,
        Null,
    }
}
