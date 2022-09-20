using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbTableComparer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool willStartComparison = true;
            var argValues = string.Join(" ", args).Split('-').Where(a => !string.IsNullOrEmpty(a)).ToList();

            if (argValues.Count() < 3)
            {
                willStartComparison = false;
                Console.WriteLine("Missing argument");
            }

            try
            {
                string firstTableName = argValues[0].Split(' ')[1];
                string secondTableName = argValues[1].Split(' ')[1];
                string pkName = argValues[2].Split(' ')[1];

                if (string.IsNullOrWhiteSpace(firstTableName) ||
                    string.IsNullOrWhiteSpace(secondTableName) ||
                    string.IsNullOrWhiteSpace(pkName))
                {
                    willStartComparison = false;
                    Console.WriteLine("Missing argument value");
                }
            }
            catch
            {
                willStartComparison = false;
                Console.WriteLine("Missing argument value");
            }

            if (willStartComparison)
            {
                Console.WriteLine("Start comparison");
            }
            else
            {
                Console.WriteLine("Operation aborted");

            }

            Console.ReadKey();
        }
    }
}
