using System;
using System.Linq;
using FireDepartmentSearch;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestFireDepartmentSearch
{
    [TestClass]
    public class TestQuadTree
    {
        [TestMethod]
        public void TestWorksInGeneral()
        {
            var positions = new[] {new Point2D(1, 1), new Point2D(-1, -1), new Point2D(10, 10)};

            var tree = new QuadTree<int>(positions, Enumerable.Range(0, positions.Length).ToList());

            var result = tree.FindNearest(new Point2D(-0.5, -0.5), 2).ToArray();

            Assert.AreEqual(2, result.Length);
            Assert.IsTrue(result[0].Data == 1);
            Assert.IsTrue(result[1].Data == 0);
        }

        [TestMethod]
        public void TestSearchHugeRandom()
        {
            var rnd = new Random(3);
            const int numberOfStations = 1000;
            const int numberOfHouses = 1000;

            var fireStations = Point2D.GetRandomSeries(new Rectangle(-1000, 1000, 2000, -3000), numberOfStations, new Random(1)).RoundCoordinated().ToList();
            
            var tree = new QuadTree(fireStations);
            var houses = Point2D.GetRandomSeries(new Rectangle(-1500, 1500, 2500, -3500), numberOfHouses, new Random(2)).RoundCoordinated().ToList();

            foreach (var house in houses)
            {
                var cnt = rnd.Next(100)+1;

                var usingTree = tree.FindNearest(house, cnt).Select(pt => pt.Position.SqrDistance(house)).OrderBy(d => d);;

                var plainSortSearch = fireStations.Select(pt => pt.SqrDistance(house)).OrderBy(d => d).Take(cnt);

                Assert.IsTrue(usingTree.SequenceEqual(plainSortSearch));
            }
        }

        //todo write some tests for boundary conditions, e.g. empty point set, coinsiding points etc.
    }
}
