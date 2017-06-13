using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FireDepartmentSearch
{
    public partial class QuadTree<TData>
    {
        private class NearestSearch
        {
            private readonly int _pointsToFind;
            private readonly Point2D _targetPoint;

            // The values are a Points and the keys are distances^2 from each Point to _targetPoint
            private SortedList<double, Point> _candidates = new SortedList<double, Point>(new NeverEqualDoubleComparer());
            private readonly HashSet<Point> _checkedPts = new HashSet<Point>();

            public IEnumerable<Point> Result => _candidates.Values;

            public NearestSearch(Point2D targetPoint, int nearestPointsToFind)
            {
                if (nearestPointsToFind <= 0) throw new ArgumentOutOfRangeException(nameof(nearestPointsToFind));

                _pointsToFind = nearestPointsToFind;
                _targetPoint = targetPoint;
            }

            private void UseAsCandidate(Point point)
            {
                if (_checkedPts.Contains(point))
                    return;

                _candidates.Add(point.Position.SqrDistance(_targetPoint), point);
                _checkedPts.Add(point);
            }

            private void ShrinkResult()
            {
                while (_candidates.Count > _pointsToFind)
                {
                    _candidates.RemoveAt(_candidates.Count - 1);
                }
            }

            public void Do(Node topNode)
            {
                // Find a leaf node of the tree, laying as close to target point as possible (usually containg the point)
                var node = topNode.NearestLeafTo(_targetPoint);
                if (node == null)
                    return;

                // use points of found leaf as current result
                node.PointsFlatten.ForAll(UseAsCandidate);

                // while insufficient number of points found, go up the nodes hierarchy and take points of met parents
                while (_candidates.Count < _pointsToFind && node.Parent != null)
                {
                    node.Parent.Children.Except(node).ForAll(
                        sibling => sibling.PointsFlatten.ForAll(UseAsCandidate));
                    node = node.Parent;
                }

                ShrinkResult();

                // in case of the whole tree is seen
                if (node.Parent == null)
                    return;

                Debug.Assert(_candidates.Count == _pointsToFind);

                // No we have a sufficient number of points but, there are unseen neighbor nodes in the tree 
                // with, points witch might be closer to targetPoint then already found ones.

                var currentFurtherst = _candidates.Last().Key;
                topNode.PointsWithinRange(_targetPoint, currentFurtherst)
                    .ForAll(UseAsCandidate);

                ShrinkResult();

                // -----------------
                // A way better approach would be to step-by-step enlarge a "concentric square" around initially found leaf node, 
                // but it's not trivial (for job test assignment) in case of "unballanced" tree (i.e. tree with variable depth of branches).
            }
        }
    }
}