using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WpfApp1.Fontionality;
using WpfApp1.Page.ViewModels;
using WpfApp1.Utils;
using Xceed.Wpf.AvalonDock.Layout;
using static OpenCvSharp.Stitcher;

namespace WpfApp1.Page.Models
{
    internal class MainWindowModel
    {
        public MainWindowViewModel ViewModel { get; set; }
        public string VideoName;
        public string VideoExtention { get; set; }

        private VideoCapture Video { get; set; }
        private Stopwatch Watch { get; set; }

        public FrameComputer FrameComputer { get; set; }

        private bool stop = false;

        public MainWindowModel(MainWindowViewModel viewModel)
        {
            this.ViewModel = viewModel;

            FrameComputer = new FrameComputer();

            Watch = new Stopwatch();

            LoadVideo();
        }

        #region Main functions

        public void LoadVideo(string name = "Autoroute_1", bool isLocal = true,bool forceStop=false)
        {
            VideoName = name;
            //Video = new VideoCapture(PathUtils.GetVideoPath("slow_traffic_small", "mp4"));
            if (isLocal)
                Video = new VideoCapture(PathUtils.GetVideoPath(name, "mp4"));
            else
                Video = new VideoCapture(name);
            Debug.WriteLine(Video.IsOpened());

            ViewModel.TotalTime = (int)(Video.FrameCount / Video.Fps);

            TimeSpan t = TimeSpan.FromSeconds(ViewModel.TotalTime);

            string v = t.ToString(@"hh\:mm\:ss");


            ViewModel.TotalTimeString = v;

            if (forceStop == false)
            {
                Playing();               
            }
                
        }

        public async void Playing()
        {
            int delayPerFrame = (int)(1000 / Video.Fps);
            long elapsed = 0;
            bool start = true;


            Watch.Start();

            Mat m = new Mat();

            while (Video.Read(m))
            {
                if (start)
                {
                    start = false;
                    FrameComputer.Dence.InstantiateLinesFrame(m.Size());
                }
                
                PreCompute(m);

                ViewModel.CurrentTime = (int)((Video.PosFrames) / Video.Fps);

                TimeSpan t = TimeSpan.FromSeconds(ViewModel.CurrentTime);

                string v = t.ToString(@"hh\:mm\:ss");

                ViewModel.CurrentTimeString = v;

                long timeElapsedLastFrame = (Watch.ElapsedMilliseconds) - elapsed;
                int timeToWait = (int)(delayPerFrame - timeElapsedLastFrame);
                timeToWait = (timeToWait < 0) ? 1 : timeToWait;

                Debug.WriteLine("Time elapsed last frame {0} Delay per frame {1} Time to wait {2}", timeElapsedLastFrame, delayPerFrame, timeToWait);

                await Task.Delay(timeToWait);

                elapsed = Watch.ElapsedMilliseconds;

                PostCompute(m);

                ViewModel.ColorPlotModel = FrameComputer.colorModel;
                ViewModel.DensePlotModel = FrameComputer.denseModel;
            }

            //Video.Open(PathUtils.GetVideoPath("slow_traffic_small", "mp4"));
            //Video.Open(PathUtils.GetVideoPath("Autoroute_1", "mp4"));
            //Video.Open(VideoName);

            if(VideoName != "Autoroute_1")
            {
                LoadVideo(VideoName, false, false);
            }
            else
            {
                LoadVideo(VideoName);
            }
            
        }

        private void PreCompute(Mat nextFrameColor)
        {
            FrameComputer.ComputeNewFrame(nextFrameColor, ViewModel.ActivateEparse, ViewModel.ActivateDense,ViewModel.ActivateObjectsDetection, ViewModel.ActivateCascadeDetector,GetCurrentEparseProfil(), GetCurrentObjectDetectionProfil(), GetCurrentDenceProfil());

            ViewModel.VideoFrame = FrameComputer.GetBitmapImage();//nextFrameColor.ToBitmapSource();
            ViewModel.MouvementType = FrameComputer.mouvement.ToString();
        }

        private void PostCompute(Mat lastFrame)
        {
            
        }


        public EparseProfil GetCurrentEparseProfil()
        {
            return new EparseProfil(ViewModel.MaxCorners, ViewModel.QualityLevel, ViewModel.MinDistance, ViewModel.BlockSize,
                ViewModel.UseHarrisDetector, ViewModel.K, new Scalar(ViewModel.LineColor.B, ViewModel.LineColor.G, ViewModel.LineColor.R),
                new Scalar(ViewModel.PointColor.B, ViewModel.PointColor.G, ViewModel.PointColor.R), ViewModel.ShowPoint, ViewModel.ShowLine);
        }

        public ObjectDetectionProfil GetCurrentObjectDetectionProfil()
        {
            return new ObjectDetectionProfil(ViewModel.Thresh,ViewModel.MaxVal,ViewModel.MinSize,
                ViewModel.MatrixSize,new Scalar(ViewModel.RectangleColor.B, ViewModel.RectangleColor.G, ViewModel.RectangleColor.R),ViewModel.ShowMovements,ViewModel.ShowObjects);
        }

        public DenceProfil GetCurrentDenceProfil()
        {
            return new DenceProfil(new Scalar(ViewModel.ArrowColor.B, ViewModel.ArrowColor.G, ViewModel.ArrowColor.R), ViewModel.ShowDenseArrow, ViewModel.ShowDenseColor);
        }

        #endregion
    }
}
