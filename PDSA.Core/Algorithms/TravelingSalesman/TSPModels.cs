using System;
using System.Collections.Generic;

namespace PDSA.Core.Algorithms.TravelingSalesman
{
    public class City
    {
        public char Name { get; set; }
        public int Index { get; set; }

        public City(char name, int index)
        {
            Name = name;
            Index = index;
        }

        public override string ToString() => Name.ToString();
    }

    public class TSPRoute
    {
        public List<City> Cities { get; set; } = new List<City>();
        public int TotalDistance { get; set; }
        public List<int> Path { get; set; } = new List<int>();

        public override string ToString()
        {
            return $"Route: {string.Join(" -> ", Cities)} -> {Cities[0]}, Distance: {TotalDistance}km";
        }
    }

    public class TSPGameRound
    {
        public int[,] DistanceMatrix { get; set; } = new int[10, 10];
        public City HomeCity { get; set; }
        public List<City> SelectedCities { get; set; } = new List<City>();
        public TSPRoute OptimalRoute { get; set; }
        public long BruteForceTime { get; set; }
        public long DynamicProgrammingTime { get; set; }
        public long NearestNeighborTime { get; set; }
    }

    public class TSPAlgorithmResult
    {
        public TSPRoute Route { get; set; }
        public long ExecutionTimeMs { get; set; }
        public string AlgorithmName { get; set; }
        public int Complexity { get; set; }
    }

    public static class TSPCities
    {
        public static readonly City[] AllCities = {
            new City('A', 0), new City('B', 1), new City('C', 2), 
            new City('D', 3), new City('E', 4), new City('F', 5), 
            new City('G', 6), new City('H', 7), new City('I', 8), 
            new City('J', 9)
        };

        public static City GetCityByName(char name)
        {
            return Array.Find(AllCities, c => c.Name == name);
        }

        public static City GetCityByIndex(int index)
        {
            return AllCities[index];
        }
    }
}