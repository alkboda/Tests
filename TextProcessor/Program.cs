using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TextProcessor.Data.CSV;
using TextProcessor.Processing;
using TextProcessor.Processing.Processors;

namespace TextProcessor
{
    class Program
    {
        #region const
        private const string POPULATION_FILE_PATH = "Cities_By_Population.txt";
        private const string INTERSTATES_FILE_PATH = "Interstates_By_City.txt";
        private const string DEGREES_FILE_PATH = "Degrees_From_Chicago.txt";
        #endregion const

        static void Main(string[] args)
        {
            string fileName = String.Join(" ", args); // in case of setting path parameter w/o double quotes
            IProcessor processor = null;
            switch (Path.GetExtension(fileName))
            {
                case ".txt":
                case ".csv":
                    processor = new CsvProcessor();
                    break;
                default:
                    throw new KeyNotFoundException($"There isn't any processor for '{Path.GetExtension(fileName)}' file type.");
            }

            var cities = processor.Process<City>(fileName).ToList();
            Option1(cities);
            Option2(cities);
            Console.WriteLine("Done.");
        }

        #region Option 1
        static void Option1(IEnumerable<City> cities)
        {
            if (!(cities?.Any() ?? false))
            {
                return;
            }

            Option1_Population(cities);
            Option1_Interstates(cities);
        }

        private static void Option1_Population(IEnumerable<City> cities)
        {
            using (var stream = File.Open(POPULATION_FILE_PATH, FileMode.Create))
            {
                using (TextWriter writer = new StreamWriter(stream))
                {
                    var groups = cities.GroupBy(_ => _.Population).OrderByDescending(_ => _.Key);

                    foreach (var group in groups)
                    {
                        writer.WriteLine(group.Key); // population
                        writer.WriteLine();

                        foreach (var city in group.OrderBy(_ => _.State).ThenBy(_ => _.Name))
                        {
                            writer.WriteLine($"{city.Name}, {city.State}");

                            var interstates = String.Join(", ", city.InterstateNumbers.OrderBy(_ => _).Select(_ => $"{city.InterstatePrefix}{_}"));
                            writer.WriteLine($"Interstates: {interstates}");
                            writer.WriteLine();
                        }
                    }
                }
            }
        }

        private static void Option1_Interstates(IEnumerable<City> cities)
        {
            var interstates = cities
                .SelectMany(city => city.InterstateNumbers, (city, number) => new { prefix = city.InterstatePrefix, number = number })
                .Distinct().OrderBy(_ => _.number);
            var interCities = interstates.ToDictionary(
                _ => $"{_.prefix}{_.number}",
                _ => cities.Count(
                    city => city.InterstateNumbers.Any(number => number == _.number)
                )
            );

            using (var stream = File.Open(INTERSTATES_FILE_PATH, FileMode.Create))
            {
                using (TextWriter writer = new StreamWriter(stream))
                {
                    foreach (var interCity in interCities)
                    {
                        writer.WriteLine($"{interCity.Key} {interCity.Value}");
                    }
                }
            }
        }
        #endregion Option 1

        #region Option 2
        static void Option2(IEnumerable<City> cities, String dcName = "Chicago")
        {
            if (!(cities?.Any() ?? false))
            {
                return;
            }

            // Final result dictionary
            //
            Dictionary<int, IEnumerable<City>> degreeGroups = new Dictionary<int, IEnumerable<City>>();

            // Func for taking first degree cities
            //
            Func<City, IEnumerable<City>, bool> isNear
                = (_dc, _cities) => _cities.Any(_city => !_city.Equals(_dc) && _city.InterstateNumbers.Intersect(_dc.InterstateNumbers).Any());

            // Getting DC
            //
            int degree = 0;
            var dc = cities.Single(_ => _.Name.Equals(dcName, StringComparison.InvariantCultureIgnoreCase));
            IEnumerable<City> generation = new [] { dc };
            cities = cities.Except(generation); // drop out DC
            degreeGroups.Add(degree++, generation);

            // Getting generations
            //
            do
            {
                generation = cities.Where(_ => isNear(_, generation)).ToArray(); // enumerating instantly because generation will recur otherwise
                cities = cities.Except(generation);

                degreeGroups.Add(degree++, generation);
            }
            while (cities.Any() && generation.Any());

            // Cities cut from DC
            //
            if (cities.Any())
            {
                degreeGroups.Add(-1, cities);
            }

            using (var stream = File.Open(DEGREES_FILE_PATH, FileMode.Create))
            {
                using (TextWriter writer = new StreamWriter(stream))
                {
                    foreach (var gen in degreeGroups.OrderByDescending(_ => _.Key))
                    {
                        cities = gen.Value.OrderBy(_ => _.Name).ThenBy(_ => _.State);
                        foreach (var city in cities)
                        {
                            writer.WriteLine($"{gen.Key} {city.Name}, {city.State}");
                        }
                    }
                }
            }
        }
        #endregion Option 2
    }
}
