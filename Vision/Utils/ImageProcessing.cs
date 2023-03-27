using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Utils
{
    public class ImageProcessing
    {
        public static Mat AdaptativeThreashold(Mat imgInput)
        {
            Mat output = new Mat();
            Cv2.AdaptiveThreshold(imgInput, output, 255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.Binary, 11, 2);
            return output;
        }

        public static Mat Threashold(Mat imgInput, double scale, double max = 255, ThresholdTypes type = ThresholdTypes.Binary)
        {
            Mat output = new Mat();
            Cv2.Threshold(imgInput, output, scale, max, type);
            return output;
        }

        public static Mat GaussianBlur(Mat imgInput)
        {
            Mat output = new Mat();
            Cv2.GaussianBlur(imgInput, output, new Size(11, 11), 0);
            return output;
        }

        public static Mat Blur(Mat imgInput, Size size)
        {
            Mat output = new Mat();
            Cv2.Blur(imgInput, output, size);
            return output;
        }

        public static Mat MedianBlur(Mat imgInput, int size)
        {
            Mat output = new Mat();
            Cv2.MedianBlur(imgInput, output, size);
            return output;
        }

        public static Mat Canny(Mat imgInput, double factor)
        {
            Mat output = new Mat();
            Cv2.Canny(imgInput, output, factor, factor * 3, 3, true);
            return output;
        }

        public static Mat Dilate(Mat imgInput, Size matrixSize, int iterations, BorderTypes border = BorderTypes.Constant)
        {
            Mat output = new Mat();
            Cv2.Dilate(imgInput, output, GetMatrix(matrixSize.Width, matrixSize.Height), iterations: iterations, borderType: border, borderValue: new Scalar(0, 0, 0));
            return output;
        }


        public static Mat Erode(Mat imgInput, Size matrixSize, int iterations)
        {
            Mat output = new Mat();
            Cv2.Erode(imgInput, output, GetMatrix(matrixSize.Width, matrixSize.Height), iterations: iterations, borderValue: new Scalar(0, 0, 0));
            return output;
        }

        public static Mat GetMatrix(int row, int column)
        {
            return new Mat(row, column, MatType.CV_8UC1);
        }

        public static Mat FindCountour(Mat imgInput, out Point[][] contours, RetrievalModes type = RetrievalModes.List)
        {
            Mat output = new Mat();
            Cv2.FindContours(imgInput, out contours, out HierarchyIndex[] hierarchy, type, ContourApproximationModes.ApproxSimple);

            return output;
        }

        public static Mat DrawCountour(Mat imgInput, Mat[] contours)
        {
            Cv2.DrawContours(imgInput, contours, -1, new Scalar(0, 255, 0), thickness: 3);
            return imgInput;
        }

        public static Mat Rogner(Mat input, Vec4i contour)
        {

            Mat output = input.Resize(new Size(input.Width - contour.Item1 - contour.Item3, input.Height - contour.Item0 - contour.Item2));


            for (int y = contour.Item1; y < input.Height - contour.Item2; y++)
            {
                for (int x = contour.Item0; x < input.Width - contour.Item3; x++)
                {
                    Vec3b color = input.Get<Vec3b>(y, x);
                    output.Set(y - contour.Item1, x - contour.Item0, color);
                }
            }
            return output;
        }

        public static Mat RemoveBackground(Vec3b m)
        {
            var loadedImgRGB = Picture.GetImage("Base2", ExtentionType.jpg, ImreadModes.Color);

            int closeValue = 65;

            for (int y = 0; y < loadedImgRGB.Height; y++)
            {
                for (int x = 0; x < loadedImgRGB.Width; x++)
                {

                    Vec3b color = loadedImgRGB.Get<Vec3b>(y, x);

                    if (Math.Abs(color.Item0 - m.Item0) < closeValue &&
                        Math.Abs(color.Item1 - m.Item1) < closeValue &&
                        Math.Abs(color.Item2 - m.Item2) < closeValue)
                    {
                        color.Item0 = 0;
                        color.Item1 = 0;
                        color.Item2 = 0;
                    }
                    else
                    {
                        color.Item0 = 255;
                        color.Item1 = 255;
                        color.Item2 = 255;
                    }


                    loadedImgRGB.Set(y, x, color);
                }
            }
            loadedImgRGB = Blur(loadedImgRGB, new Size(5, 5));
            loadedImgRGB = Erode(loadedImgRGB, new Size(3, 3), 3);
            //loadedImgRGB = ImageProcessing.Canny(loadedImgRGB, 10);
            return loadedImgRGB;
        }

        public static Mat MergeImage(Mat input1, Mat input2, Mat imageFusionnee)
        {
            for (int y = 0; y < input1.Height; y++)
            {
                for (int x = 0; x < input1.Width; x++)
                {
                    Vec3b color = input1.Get<Vec3b>(y, x);
                    Vec3b color1 = input2.Get<Vec3b>(y, x);

                    color.Item0 = color.Item0 > 254 / 2 ? (byte)254 : (byte)0;
                    color.Item1 = color.Item1 > 254 / 2 ? (byte)254 : (byte)0;
                    color.Item2 = color.Item2 > 254 / 2 ? (byte)254 : (byte)0;

                    color1.Item0 = color1.Item0 > 254 / 2 ? (byte)254 : (byte)0;
                    color1.Item1 = color1.Item1 > 254 / 2 ? (byte)254 : (byte)0;
                    color1.Item2 = color1.Item2 > 254 / 2 ? (byte)254 : (byte)0;

                    if (color.Item0 > 15 || color.Item1 > 15 || color.Item2 > 15 || color1.Item0 > 15 || color1.Item1 > 15 || color1.Item2 > 15)
                        imageFusionnee.Set(y, x, new Vec3b(255, 255, 255));
                    else
                        imageFusionnee.Set(y, x, new Vec3b(0, 0, 0));

                    input1.Set(y, x, color);
                    input2.Set(y, x, color1);

                }
            }

            return imageFusionnee;
        }

        public static Mat FillWithPropagation(Mat input)
        {

            // Initialiser une file d'attente avec les pixels de bord de l'image
            Queue<Point> queue = new Queue<Point>();
            for (int y = 0; y < input.Rows; y++)
            {
                if (input.At<byte>(y, 0) == 0)
                {
                    queue.Enqueue(new Point(0, y));
                }
                if (input.At<byte>(y, input.Cols - 1) == 0)
                {
                    queue.Enqueue(new Point(input.Cols - 1, y));
                }
            }
            for (int x = 0; x < input.Cols; x++)
            {
                if (input.At<byte>(0, x) == 0)
                {
                    queue.Enqueue(new Point(x, 0));
                }
                if (input.At<byte>(input.Rows - 1, x) == 0)
                {
                    queue.Enqueue(new Point(x, input.Rows - 1));
                }
            }

            // Appliquer le remplissage par propagation en utilisant la file d'attente
            while (queue.Count > 0)
            {
                Point p = queue.Dequeue();
                if (input.At<byte>(p.Y, p.X) == 0)
                {
                    input.Set<byte>(p.Y, p.X, 255);
                    if (p.X > 0) queue.Enqueue(new Point(p.X - 1, p.Y));
                    if (p.X < input.Cols - 1) queue.Enqueue(new Point(p.X + 1, p.Y));
                    if (p.Y > 0) queue.Enqueue(new Point(p.X, p.Y - 1));
                    if (p.Y < input.Rows - 1) queue.Enqueue(new Point(p.X, p.Y + 1));
                }
            }

            for (int y = 0; y < input.Height; y++)
            {
                for (int x = 0; x < input.Width; x++)
                {
                    Vec3b color = input.Get<Vec3b>(y, x);

                    if (color.Item0 == 255 && color.Item1 == 0 && color.Item2 == 0)
                        input.Set(y, x, new Vec3b(0, 0, 0));
                    else if (color.Item0 == 0 && color.Item1 == 0 && color.Item2 == 0)
                        input.Set(y, x, new Vec3b(255, 255, 255));

                }
            }

            return input;
        }

        public static List<Point[]> GetContourWithMinimumSize(int minSize, Point[][] points)
        {
            List<Point[]> filteredContours = new List<Point[]>();

            foreach (var contour in points)
            {
                Debug.WriteLine(Cv2.ContourArea(contour));

                if (Cv2.ContourArea(contour) > minSize)
                {
                    filteredContours.Add(contour);
                }
            }

            return filteredContours;
        }

        public static Mat ReplaceColor(Mat img)
        {
            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    Vec3b color = img.Get<Vec3b>(y, x);

                    if (color.Item0 == 0 && color.Item1 == 255 && color.Item2 == 0)
                        img.Set(y, x, new Vec3b(255, 255, 255));
                    else if (color.Item0 == 255 && color.Item1 == 255 && color.Item2 == 255)
                        img.Set(y, x, new Vec3b(0, 0, 0));

                }
            }

            return img;
        }


    }
}
