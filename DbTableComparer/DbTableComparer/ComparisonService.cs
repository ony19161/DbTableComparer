using DbTableComparer.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DbTableComparer
{
    public class ComparisonService
    {
        private readonly AppConfiguration appConfiguration;

        public ComparisonService(AppConfiguration appConfiguration)
        {
            this.appConfiguration = appConfiguration;
        }

        public void CompareTables()
        {
            try
            {
                using (SqlConnection dbConnection = new SqlConnection(appConfiguration.ConnectionString))
                {
                    dbConnection.Open();
                    
                    using (SqlCommand command = dbConnection.CreateCommand())
                    {
                        command.CommandText = appConfiguration.FindTablesQuery
                                              .Replace("@TABLE_1", appConfiguration.TablesConfig.FirstTableName)
                                              .Replace("@TABLE_2", appConfiguration.TablesConfig.SecondTableName);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                DataTable dt = new DataTable();
                                dt.Load(reader);
                                
                                if (dt.Rows.Count < 2)
                                {
                                    Console.WriteLine("Table not found in database");
                                    return;
                                }
                                else
                                {

                                }
                            }
                            
                        }
                    }
                        
                    dbConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error on comparing tables. ErrorMsg: " + ex.Message);
            }
        }
    }
}
