using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FireDepartmentSearch
{
    //https://en.wikipedia.org/wiki/Quadtree

    public partial class QuadTree<TData>
    {
        private const int MaxPointsPerLeafNode = 5;

        //todo consider implementing MaxDepth parameter

        private readonly Node _root;

        public QuadTree(IList<Point2D> points, IList<TData> data)
        {
            if (points.Count != data.Count)
                throw new ArgumentException("Number of points and data items count must be equal.");

            _root = new Node(null, Rectangle.BoundingBoxOf(points));

            for (var i = 0; i < points.Count; i++)
            {
                _root.Insert(points[i], data[i]);
            }
        }

        public static QuadTree<TData> For(IEnumerable<TData> data, Func<TData, Point2D> dataItemPosition)
        {
            var dataList = data.ToList();
            var positions = dataList.Select(dataItemPosition).ToList();

            return new QuadTree<TData>(positions, dataList);
        }

        public IEnumerable<Point> FindNearest(Point2D point, int count)
        {
            var search = new NearestSearch(point, count);
            search.Do(_root);

            return search.Result;
        }

        public FluentApiSearchAgent FindNearest(int count)
        {
            return new FluentApiSearchAgent(this, count);
        }

        public class Point
        {
            public Point(Point2D position, TData data)
            {
                Position = position;
                Data = data;
            }

            public TData Data { get; }
            public Point2D Position { get; }
        }

        private class Node
        {
            public Node(Node parent, Rectangle bounds)
            {
                Parent = parent;
                Bounds = bounds;
                _points = new List<Point>();
            }

            private Rectangle Bounds { get; }
            public Node[] Children { get; private set; }
            private List<Point> _points;

            public IEnumerable<Point> PointsFlatten => _points ?? Children.SelectMany(ch => ch.PointsFlatten);
            public Node Parent { get; }

            public void Insert(Point2D position, TData data)
            {
                if (!Bounds.Contains(position))
                    throw new ArgumentException("Given point does not lay inside the spatial bounds of node.");

                Insert(new Point(position, data));
            }

            private void Insert(Point point)
            {
                Debug.Assert(Bounds.Contains(point.Position));

                if (Children != null)
                {
                    InsertToPropperChild(point);
                }
                else
                {
                    _points.Add(point);
                    if (_points.Count > MaxPointsPerLeafNode)
                        Split();
                }
            }
            
            private void InsertToPropperChild(Point pt)
            {
                var center = Bounds.Center;

                var index = 0;
                if (pt.Position.X >= center.X)
                    index = index + 1;
                if (pt.Position.Y <= center.Y)
                    index = index + 2;

                if (!Children[index].Bounds.Contains(pt.Position))
                    throw new InvalidOperationException("Algorithm implemetation exception");

                Children[index].Insert(pt);
            }

            private void Split()
            {
                Debug.Assert(Children == null, "Trying to split already splitted node"); // it's private method so think it can be an assertion, not an InvalidOperationException

                Children = new Node[4];
                Children[0] = new Node(this, new Rectangle(Bounds.Left, Bounds.Top, Bounds.Center.X, Bounds.Center.Y));
                Children[1] = new Node(this, new Rectangle(Bounds.Center.X, Bounds.Top, Bounds.Right, Bounds.Center.Y));
                Children[2] = new Node(this, new Rectangle(Bounds.Left, Bounds.Center.Y, Bounds.Center.X, Bounds.Bottom));
                Children[3] = new Node(this, new Rectangle(Bounds.Center.X, Bounds.Center.Y, Bounds.Right, Bounds.Bottom));

                foreach (var pt in _points)
                {
                    InsertToPropperChild(pt);
                }
                
                Debug.Assert(_points.Count == Children.Sum(child => child.PointsFlatten.Count()));
                _points = null;
            }

            public Node NearestLeafTo(Point2D point)
            {
                if (Children == null || Children.Length == 0)
                    return this;

                return Children.WhereMin(n => n.Bounds.SqrMinDistanceTo(point)).NearestLeafTo(point);
            }

            public IEnumerable<Point> PointsWithinRange(Point2D point, double sqrRange)
            {
                if (Bounds.SqrMinDistanceTo(point) > sqrRange)
                    return Enumerable.Empty<Point>();

                if (_points != null)
                    return _points.Where(pt => pt.Position.SqrDistance(point) <= sqrRange);

                // Perhaps it's not the best in terms or perfomance implementation. Should be examined with profiler.
                return Children.Where(ch => ch.Bounds.SqrMinDistanceTo(point) <= sqrRange)
                    .SelectMany(n => n.PointsWithinRange(point, sqrRange));
            }
        }
    }

    public class QuadTree : QuadTree<int>
    {
        public QuadTree(IList<Point2D> points) : base(points, Enumerable.Range(0, points.Count).ToList())
        {
        }
    }
}