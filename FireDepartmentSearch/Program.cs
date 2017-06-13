using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace FireDepartmentSearch
{
    class Program
    {
        // For given set of houses and fire stations (integer coordinates on 2D cartesian space) 
        // for each house found 3 nearest stations.
        
        // The big-O complexity is about:
        // O(n*log(n) for bulding the stations spatial index
        // +
        // m*log(n)) for searching nearest stations;
        // where n and m are numbers of stations and houses respectively.

        static void Main(string[] args)
        {
            var housesFile = "houses.txt";
            var stationsFile = "stations.txt";
            var resultFile = "stations for houses.txt";

            // Uncomment to generate sample files. Last parameters corresponds to the size of sample
            //WritePoints(housesFile, Point2D.GetRandomSeries(new Rectangle(-1000, 1000, 2000, -3000), 100000));
            //WritePoints(stationsFile, Point2D.GetRandomSeries(new Rectangle(-1000, 1000, 2000, -3000), 10000));

            Console.WriteLine($"Reading fire stations location data ({stationsFile})...");
            var stationsPositions = ReadPoints(stationsFile).ToList();

            var swTotally = new Stopwatch();
            
            Console.Write("Building fire stations spatial index...");
            swTotally.Start();
            var stations = new QuadTree(stationsPositions);
            Console.WriteLine($" {swTotally.Elapsed} elapsed.");

            Console.Write($"Searching closest fire stations \"{resultFile}\"...");
            var swSeraching = new Stopwatch();
            swSeraching.Start();
            using (var file = new StreamWriter(resultFile))
            {
                file.WriteLine("Coordinates of house: Three closest fire stations (seq.no. and coordinates)");

                foreach (var housePosition in ReadPoints(housesFile))
                {
                    var nearestStations = stations.FindNearest(3).To(housePosition);

                    var stationsFormated = string.Join(", ", nearestStations.Select(pt => $"#{pt.Data} ({pt.Position.X}; {pt.Position.Y})"));
                    file.WriteLine($"({housePosition.X}; {housePosition.Y}): {stationsFormated}");
                }
            }
            Console.WriteLine($" {swSeraching.Elapsed} elapsed.");
            Console.WriteLine($"{swTotally.Elapsed} elapsed totatly.");

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }

        private static IEnumerable<Point2D> ReadPoints(string fileName)
        {
            //todo check line count instead of skipping it's value
            return File.ReadLines(fileName).Skip(1).Select(line => Point2D.Parse(line, ','));
        }

        private static void WritePoints(string fileName, IEnumerable<Point2D> points)
        {
            var list = points.Select(pt => $"{(int)pt.X}, {(int)pt.Y}").ToList();
            list.Insert(0, list.Count.ToString());
            File.WriteAllLines(fileName, list);
        }
    }
}
