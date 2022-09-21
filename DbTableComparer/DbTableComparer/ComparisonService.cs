using DbTableComparer.Models;
using System;
using System.Collections;
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
        private Hashtable databaseTables;
        public ComparisonService(AppConfiguration appConfiguration)
        {
            this.appConfiguration = appConfiguration;
            databaseTables = new Hashtable();
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
                                DataTable dataTable = new DataTable();
                                dataTable.Load(reader);
                                
                                if (dataTable.Rows.Count < 2)
                                {
                                    Console.WriteLine("Table not found in database");
                                    return;
                                }
                                else
                                {
                                    foreach (DataRow dataRow in dataTable.Rows)
                                    {
                                        DatabaseTable table = new DatabaseTable(dataRow[0].ToString(), (int)dataRow[1] );
                                        table.GetColumnData(dbConnection, appConfiguration.FindColumnQuery);
                                        databaseTables[table.Name] = table;
                                    }
                                }
                            }                            
                        }

                        if (!IsSchemaMatching((DatabaseObject)databaseTables[appConfiguration.TablesConfig.FirstTableName.ToLower()],
                                              (DatabaseObject)databaseTables[appConfiguration.TablesConfig.SecondTableName.ToLower()]))
                        {
                            Console.WriteLine("Tables schema does not match");
                            return;
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

        private bool IsSchemaMatching(DatabaseObject firstTable, DatabaseObject secondTable)
        {
            return firstTable.CompareColumns(secondTable);
        }
    }
}
