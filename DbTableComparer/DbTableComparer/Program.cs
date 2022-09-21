using DbTableComparer.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
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
            TablesConfiguration tablesConfig = new TablesConfiguration();

            if (argValues.Count() < 3)
            {
                willStartComparison = false;
                Console.WriteLine("Missing argument");
            }

            try
            {
                tablesConfig.FirstTableName = argValues[0].Split(' ')[1];
                tablesConfig.SecondTableName = argValues[1].Split(' ')[1];
                tablesConfig.PkName = argValues[2].Split(' ')[1];

                if (string.IsNullOrWhiteSpace(tablesConfig.FirstTableName) ||
                    string.IsNullOrWhiteSpace(tablesConfig.SecondTableName) ||
                    string.IsNullOrWhiteSpace(tablesConfig.PkName))
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
                var appConfig = GetAppConfiguration();
                appConfig.TablesConfig = tablesConfig;

                new ComparisonService(appConfig).CompareTables();
            }
            else
            {
                Console.WriteLine("Operation aborted");

            }

            Console.ReadKey();
        }

        static AppConfiguration GetAppConfiguration()
        {
            var appConfiguration = new AppConfiguration();
            appConfiguration.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ToString();
            appConfiguration.FindTablesQuery = ConfigurationManager.AppSettings["findTablesQuery"];
            appConfiguration.FindColumnQuery = ConfigurationManager.AppSettings["findColumnsQuery"];
            appConfiguration.AddedRowsQuery = ConfigurationManager.AppSettings["addedRowsQuery"];
            appConfiguration.DeletedRowsQuery = ConfigurationManager.AppSettings["deletedRowsQuery"];
            appConfiguration.ChangeDetectQuery = ConfigurationManager.AppSettings["changeDetectQuery"];
            appConfiguration.PageSize = ConfigurationManager.AppSettings["pageSize"];

            return appConfiguration;
        }
    }
}
