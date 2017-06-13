using System.Collections.Generic;

namespace FireDepartmentSearch
{
    public partial class QuadTree<TData>
    {
        //Just for fun. Actualy I'm not a an anvid fan of this sort of API.
        public struct FluentApiSearchAgent
        {
            private readonly QuadTree<TData> _tree;
            private readonly int _count;

            public FluentApiSearchAgent(QuadTree<TData> tree, int count)
            {
                _tree = tree;
                _count = count;
            }

            public IEnumerable<Point> To(Point2D point)
            {
                return _tree.FindNearest(point, _count);
            } 
        }
    }
}