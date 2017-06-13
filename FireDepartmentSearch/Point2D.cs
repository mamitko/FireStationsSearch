using System;
using System.Collections.Generic;
using System.Linq;

namespace FireDepartmentSearch
{
    public struct Point2D
    {
        public double X { get; }
        public double Y { get; }

        public Point2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static Point2D Parse(string coordinates, char separator)
        {
            var asNumbers = coordinates.Split(separator);
            return new Point2D(double.Parse(asNumbers[0].Trim()), double.Parse(asNumbers[1].Trim()));
        }

        public double SqrDistance(Point2D other)
        {
            var dx = this.X - other.X;
            var dy = this.Y - other.Y;

            return dx*dx + dy*dy;
        }

        public static IEnumerable<Point2D> GetRandomSeries(Rectangle area, int count, Random random = null)
        {
            random = random ?? new Random();
            
            for (var i = 0; i < count; i++)
            {
                yield return new Point2D(
                    area.Left + random.NextDouble()*area.Width, 
                    area.Top - random.NextDouble()*area.Height);
            }
        }
    }

    public static class Point2DExtension
    {
        public static IEnumerable<Point2D> RoundCoordinated(this IEnumerable<Point2D> points)
        {
            return points.Select(pt => new Point2D(Math.Round(pt.X), Math.Round(pt.Y)));
        }
    }
}