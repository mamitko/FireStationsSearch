using System.Collections.Generic;

namespace FireDepartmentSearch
{
    /// <summary>
    /// It's a sort of dirty trick. Think should not be used in production code
    /// </summary>
    internal class NeverEqualDoubleComparer : IComparer<double>
    {
        public int Compare(double x, double y)
        {
            return x < y ? -1 : 1;
        }
    }
}