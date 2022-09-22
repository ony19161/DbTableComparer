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
                Console.WriteLine(String.Format("Comparing Table: {0} And Table: {1}", tablesConfig.FirstTableName, tablesConfig.SecondTableName));
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
            string changeDetectInnerQuery = ConfigurationManager.AppSettings["changeDetectInnerQuery"];
            appConfiguration.ChangeCountQuery = ConfigurationManager.AppSettings["changeCountQuery"].Replace("@CHANGE_DETECT_INNER_QUERY", changeDetectInnerQuery);
            appConfiguration.ChangeDetectQuery = ConfigurationManager.AppSettings["changeDetectQuery"].Replace("@CHANGE_DETECT_INNER_QUERY", changeDetectInnerQuery);
            appConfiguration.PageSize = GetPageSize();

            return appConfiguration;
        }

        static int GetPageSize()
        {
            int pageSize = 2;

            if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["pageSize"]))
            {
                var value = Convert.ToInt16(ConfigurationManager.AppSettings["pageSize"]);
                // Setting even page size for change detect method 
                pageSize = value % 2 == 0 ? value : value + 1;
            }

            return pageSize;
        }
    }
}
