using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp3
{
    public class LissajousModel
    {
        public List<Point> GeneratePoints(
            double amplitudeX,
            double amplitudeY,
            double frequencyX,
            double frequencyY,
            double phaseXDegrees,
            double phaseYDegrees,
            int pointsCount)
        {
            var points = new List<Point>();

            double phaseX = phaseXDegrees * Math.PI / 180.0;
            double phaseY = phaseYDegrees * Math.PI / 180.0;

            double periodX = 1.0 / frequencyX;
            double periodY = 1.0 / frequencyY;

            double totalTime = 20 * Math.Max(periodX, periodY);

            double dt = totalTime / pointsCount;

            for (int i = 0; i <= pointsCount; i++)
            {
                double t = i * dt;

                double x = amplitudeX *
                    Math.Sin(2 * Math.PI * frequencyX * t + phaseX);

                double y = amplitudeY *
                    Math.Sin(2 * Math.PI * frequencyY * t + phaseY);

                points.Add(new Point(x, y));
            }
            

            return points;
        }
    }
}
