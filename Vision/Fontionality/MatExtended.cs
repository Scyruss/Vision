using OpenCvSharp;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Fontionality
{
    public static class MatExtended
    {
        /// <summary>
        /// Retourne une série de point pour construire un histogramme
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="dimSize">Taille de chaque dimmension</param>
        /// <param name="min">valeur minimum</param>
        /// <param name="max">valeur maximum</param>
        /// <returns></returns>
        public static LineSeries GetHist(this Mat mat, int dimSize, int min, int max)
        {
            var lineSeries = new LineSeries();
            Mat hist = new Mat();
            int[] hdims = { dimSize }; // Histogram taile de chaque dimension
            Rangef[] ranges = { new Rangef(min, max), }; // min/max 

            Cv2.CalcHist(
                    new Mat[] { mat },
                    new int[] { 0 },
                    null,
                    hist,
                    1,
                    hdims,
                    ranges);

            for (int j = 0; j < hdims[0]; ++j)
            {
                lineSeries.Points.Add(new OxyPlot.DataPoint(j, hist.Get<float>(j)));
            }

            return lineSeries;
        }



    }
}
