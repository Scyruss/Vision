using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Utils;

namespace WpfApp1.Fontionality
{
    internal class CascadeDetection
    {
        public Mat DetectCar(Mat image)
        {
            //CascadeClassifier cascadeClassifier = new CascadeClassifier("C:\\Users\\romai\\Documents\\GitHub\\ProjetPuzzle\\WpfApp1\\Classifier\\cars.xml");
            CascadeClassifier cascadeClassifier = new CascadeClassifier(PathUtils.GetXMLPath("cars"));

            Mat newMat = new Mat();
            image.CopyTo(newMat);

            if(!newMat.Empty())
            {
                Rect[] cars = cascadeClassifier.DetectMultiScale(newMat, minSize: new Size(20,20));

                // Afficher le nombre de voitures détectées
                //Console.WriteLine("Voitures détectées: " + cars.Length);

                // Afficher une boîte autour de chaque voiture détectée
                foreach (Rect car in cars)
                {
                    Cv2.Rectangle(newMat, car, Scalar.Red, 2);
                }
            }
                
            return newMat;
        }



    }
}
