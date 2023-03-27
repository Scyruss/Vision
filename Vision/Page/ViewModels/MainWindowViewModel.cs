using Microsoft.Win32;
using OpenCvSharp;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Wpf.Ui.Mvvm.Contracts;
using WpfApp1.Page.Models;
using WpfApp1.Utils;
using Xceed.Wpf.Toolkit;

namespace WpfApp1.Page.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        public MainWindowModel model;

        private PlotModel colorPlotModel;
        public PlotModel ColorPlotModel { get => colorPlotModel; set { colorPlotModel = value; OnPropertyChanged(nameof(ColorPlotModel)); } }

        private PlotModel densePlotModel;
        public PlotModel DensePlotModel { get => densePlotModel; set { densePlotModel = value; OnPropertyChanged(nameof(DensePlotModel)); } }

        private BitmapSource videoFrame;
        public BitmapSource VideoFrame { get => videoFrame; set { videoFrame = value; OnPropertyChanged(nameof(VideoFrame)); } }

        int maxCorners = 23;
        public int MaxCorners { get => maxCorners; set { maxCorners = value; OnPropertyChanged(nameof(MaxCorners)); } }

        double qualityLevel = 0.01;
        public double QualityLevel { get => qualityLevel; set { qualityLevel = value; OnPropertyChanged(nameof(QualityLevel)); } }

        double minDistance = 10;
        public double MinDistance { get => minDistance; set { minDistance = value; OnPropertyChanged(nameof(MinDistance)); } }

        int blockSize = 3;
        public int BlockSize { get => blockSize; set { blockSize = value; OnPropertyChanged(nameof(BlockSize)); } }

        bool useHarrisDetector = false;
        public bool UseHarrisDetector { get => useHarrisDetector; set { useHarrisDetector = value; OnPropertyChanged(nameof(UseHarrisDetector)); } }

        string mouvementType = "";
        public string MouvementType { get => mouvementType; set { mouvementType = value; OnPropertyChanged(nameof(MouvementType)); } }

        bool activateCascadeDetector = false;

        bool showDenseArrow = false;
        public bool ShowDenseArrow 
        { 
            get => showDenseArrow; 
            set 
            { 
                showDenseArrow = value;
                showDenseColor = false;
                OnPropertyChanged(nameof(ShowDenseArrow));
                OnPropertyChanged(nameof(ShowDenseColor));


                VisibilityDenseArrowSettings = (showDenseArrow) ? Visibility.Visible : Visibility.Collapsed;

            } 
        }

        bool showDenseColor = false;
        public bool ShowDenseColor 
        { 
            get => showDenseColor; 
            set 
            { 
                showDenseColor = value; 
                showDenseArrow = false;
                OnPropertyChanged(nameof(ShowDenseColor));
                OnPropertyChanged(nameof(ShowDenseArrow));

                VisibilityDenseArrowSettings = Visibility.Collapsed;
            } 
        }

        Visibility visibilityDenseArrowSettings = Visibility.Collapsed;
        public Visibility VisibilityDenseArrowSettings { get => visibilityDenseArrowSettings; set { visibilityDenseArrowSettings = value; OnPropertyChanged(nameof(VisibilityDenseArrowSettings)); } }

        Color arrowColor = Colors.Green;
        public Color ArrowColor { get => arrowColor; set { arrowColor = value; OnPropertyChanged(nameof(ArrowColor)); } }


        public bool ActivateCascadeDetector 
        { 
            get => activateCascadeDetector; 
            set 
            {
                activateObjectsDetection = false;
                activateCascadeDetector = value; 
                OnPropertyChanged(nameof(ActivateCascadeDetector));
                OnPropertyChanged(nameof(ActivateObjectsDetection));
            }
        }

        double k = 0.04;
        public double K { get => k; set { k = value; OnPropertyChanged(nameof(K)); } }

        Visibility visibilityEparse = Visibility.Collapsed;
        public Visibility VisibilityEparse { get => visibilityEparse; set { visibilityEparse = value; OnPropertyChanged(nameof(VisibilityEparse)); } }

        Visibility visibilityDense = Visibility.Collapsed;
        public Visibility VisibilityDense { get => visibilityDense; set { visibilityDense = value; OnPropertyChanged(nameof(VisibilityDense)); } }

        Visibility visibilityObjectsDetection = Visibility.Collapsed;
        public Visibility VisibilityObjectsDetection { get => visibilityObjectsDetection; set { visibilityObjectsDetection = value; OnPropertyChanged(nameof(VisibilityObjectsDetection)); } }


        bool activateEparse = false;
        public bool ActivateEparse
        {
            get => activateEparse;

            set
            {
                activateDense = false;
                activateEparse = value;
                OnPropertyChanged(nameof(ActivateEparse));
                OnPropertyChanged(nameof(ActivateDense));

                VisibilityDense = Visibility.Collapsed;
                VisibilityEparse = (activateEparse)? Visibility.Visible : Visibility.Collapsed;

            }
        }

        bool activateDense = false;
        public bool ActivateDense
        {
            get => activateDense;

            set
            {
                activateEparse = false;
                activateDense = value;
                OnPropertyChanged(nameof(ActivateDense));
                OnPropertyChanged(nameof(ActivateEparse));

                VisibilityEparse = Visibility.Collapsed;
                VisibilityDense = (activateDense) ? Visibility.Visible : Visibility.Collapsed;

            }
        }

        bool showPoint = false;
        public bool ShowPoint { get => showPoint; set { showPoint = value; OnPropertyChanged(nameof(ShowPoint)); } }

        bool showLine = false;
        public bool ShowLine { get => showLine; set { showLine = value; OnPropertyChanged(nameof(ShowLine)); } }

        Color pointColor = Colors.Red;
        public Color PointColor { get => pointColor; set { pointColor = value; OnPropertyChanged(nameof(PointColor)); } }

        Color lineColor = Colors.Blue;
        public Color LineColor { get => lineColor; set { lineColor = value; OnPropertyChanged(nameof(LineColor)); } }

        public ViewModelCommand Clear { get; }
        public ViewModelCommand MouseDownTime { get; }
        public ViewModelCommand TimeChange { get; }

        public ViewModelCommand OpenVideo { get; }


        double thresh = 20;
        public double Thresh { get => thresh; set { thresh = value; OnPropertyChanged(nameof(Thresh)); } }
        double maxVal = 255;
        public double MaxVal { get => maxVal; set { maxVal = value; OnPropertyChanged(nameof(MaxVal)); } }
        double minSize = 100;
        public double MinSize { get => minSize; set { minSize = value; OnPropertyChanged(nameof(MinSize)); } }
        int matrixSize = 3;
        public int MatrixSize { get => matrixSize; set  { matrixSize = value; OnPropertyChanged(nameof(MatrixSize)); } }
        Color rectangleColor = Colors.Green;
        public Color RectangleColor { get => rectangleColor; set  { rectangleColor = value; OnPropertyChanged(nameof(RectangleColor)); } }
        bool showMovements;
        public bool ShowMovements { get => showMovements; set { showMovements = value; OnPropertyChanged(nameof(ShowMovements)); } }
        bool showObjects;
        public bool ShowObjects { get => showObjects; set { showObjects = value; OnPropertyChanged(nameof(ShowObjects)); } }
        public bool ActivateObjectsDetection 
        { 
            get => activateObjectsDetection; 

            set {

                activateCascadeDetector = false;
                activateObjectsDetection = value;
                OnPropertyChanged(nameof(ActivateObjectsDetection));
                OnPropertyChanged(nameof(ActivateCascadeDetector));

                VisibilityObjectsDetection = (activateObjectsDetection) ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        bool activateObjectsDetection = false;

        public int CurrentTime { get => currentTime; set { currentTime = value; OnPropertyChanged(nameof(CurrentTime)); } }
        public int TotalTime { get => totalTime; set { totalTime = value; OnPropertyChanged(nameof(TotalTime)); } }

        int currentTime = 0;
        int totalTime = 100;

        public string CurrentTimeString { get => currentTimeString; set { currentTimeString = value; OnPropertyChanged(nameof(CurrentTimeString)); } }
        public string TotalTimeString { get => totalTimeString; set { totalTimeString = value; OnPropertyChanged(nameof(TotalTimeString)); } }

        string currentTimeString = "";
        string totalTimeString = "";


        public MainWindowViewModel()
        {
            videoFrame = new BitmapImage();

            model = new MainWindowModel(this);

            Clear = new ViewModelCommand(ClearCommand);
            TimeChange = new ViewModelCommand(TimeChangeCommand);
            MouseDownTime = new ViewModelCommand(MouseDownTimeCommand);
            OpenVideo = new ViewModelCommand(OpenVideoCommand);
        }

        private void OpenVideoCommand(object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.FileName = "Select a video file";
            openFileDialog.Filter = "Text files (*mp4)|*.mp4";
            openFileDialog.Title = "Open video file";

            if ((bool)openFileDialog.ShowDialog())
            {
                ActivateDense = false;
                ActivateEparse = false;
                ActivateObjectsDetection = false;
                ActivateCascadeDetector = false;

                var file = openFileDialog.FileName;

                model.LoadVideo(file,false,true);

                service.Show();
            }     
        }

        SnackbarService service = new SnackbarService();

        private void ClearCommand(object obj)
        {
            model.FrameComputer.Dence.InstantiateLinesFrame(model.FrameComputer.LastFrameMatGray.Size());
        }

        private void TimeChangeCommand(object obj)
        {
        }
        private void MouseDownTimeCommand(object obj)
        {
        }
    }
}
