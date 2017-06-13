using System;
using System.Collections.Generic;
using System.Linq;
using static System.Math;

namespace FireDepartmentSearch
{
    /// <summary>
    /// Axis aligned rectangle
    /// </summary>
    public struct Rectangle
    {
        /// <summary>
        /// Vertices of polygon clockwise starting from left top one
        /// </summary>
        private readonly Point2D[] _corners;

        public Point2D LeftTop => _corners[0];
        public Point2D RightBottom => _corners[2];
        public Point2D Center => new Point2D(Left + Width/2, Top - Height/2);

        public Rectangle(double left, double top, double right, double bottom)
        {
            _corners = new[] {new Point2D(left, top), new Point2D(right, top), new Point2D(right, bottom), new Point2D(left, bottom)};
        }

        public bool Contains(Point2D point)
        {
            return Left <= point.X && point.X <= Right 
                && Bottom <= point.Y && point.Y <= Top;
        }

        public static Rectangle BoundingBoxOf(IEnumerable<Point2D> points)
        {
            var pts = points as IList<Point2D> ?? points.ToList();
            if (pts.Count == 0)
                return Void;

            // very inefficient but short to type
            return new Rectangle(pts.Min(pt => pt.X), pts.Max(pt => pt.Y), pts.Max(pt => pt.X), pts.Min(pt => pt.Y));
        }

        public static Rectangle Void = new Rectangle(double.NaN, double.NaN, double.NaN, double.NaN);

        /// <summary>
        /// Power of two of the value of Euclidian distance to the closes point of rectange.
        /// </summary>
        public double SqrMinDistanceTo(Point2D point)
        {
            var dy = double.NaN;
            if (Left <= point.X && point.X <= Right) // if point is right "above" or "under" the rectange 
                dy = Min(Abs(point.Y - Top), Abs(point.Y - Bottom));

            var dx = double.NaN;
            if (Bottom <= point.Y && point.Y <= Top)
                dx = Min(Abs(point.X - Left), Abs(point.X - Right));

            if (!double.IsNaN(dy) && !double.IsNaN(dx)) // if point is inside or at any bound of the rectange
                return 0;

            if (!double.IsNaN(dy))
                return dy*dy;

            if (!double.IsNaN(dx))
                return dx*dx;

            // some optimizations possible...
            return _corners.Select(c => c.SqrDistance(point)).Min();
        }

        public double Left => LeftTop.X;
        public double Top => LeftTop.Y;
        public double Right => RightBottom.X;
        public double Bottom => RightBottom.Y;

        public double Width => RightBottom.X - LeftTop.X;
        public double Height => LeftTop.Y - RightBottom.Y;
    }
}