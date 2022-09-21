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

                        DatabaseObject firstTable = (DatabaseObject)databaseTables[appConfiguration.TablesConfig.FirstTableName.ToLower()];
                        DatabaseObject secondTable = (DatabaseObject)databaseTables[appConfiguration.TablesConfig.SecondTableName.ToLower()];

                        if (!IsSchemaMatching(firstTable, secondTable))
                        {
                            Console.WriteLine("Tables schema does not match");
                            return;
                        }

                        Console.WriteLine();
                        FindAddedRows(dbConnection, firstTable.Columns);
                        Console.WriteLine();
                        FindDeletedRows(dbConnection, firstTable.Columns);
                        Console.WriteLine();
                        FindModifiedRows(dbConnection, firstTable.Columns);
                    }
                        
                    dbConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error on comparing tables. ErrorMsg: " + ex.Message);
            }
        }

        private void FindAddedRows(SqlConnection dbConnection, Hashtable Columns)
        {
            Console.WriteLine("--------Added Rows---------");
            int count = 0;

            using (SqlCommand command = dbConnection.CreateCommand())
            {
                command.CommandText = appConfiguration.AddedRowsQuery
                                      .Replace("@TABLE_1", appConfiguration.TablesConfig.FirstTableName)
                                      .Replace("@TABLE_2", appConfiguration.TablesConfig.SecondTableName)
                                      .Replace("@PK", appConfiguration.TablesConfig.PkName);
                
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        count++;
                        PrintColumnValues(reader, Columns, count);                        
                    }
                }

                if (count == 0)
                {
                    Console.WriteLine("No new rows added");
                }
            }
        }

        private void FindDeletedRows(SqlConnection dbConnection, Hashtable Columns)
        {
            Console.WriteLine("--------Deleted Rows---------");
            int count = 0;
            using (SqlCommand command = dbConnection.CreateCommand())
            {
                command.CommandText = appConfiguration.DeletedRowsQuery
                                      .Replace("@TABLE_1", appConfiguration.TablesConfig.FirstTableName)
                                      .Replace("@TABLE_2", appConfiguration.TablesConfig.SecondTableName)
                                      .Replace("@PK", appConfiguration.TablesConfig.PkName);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        count++;
                        PrintColumnValues(reader, Columns, count);                        
                    }
                }

                if (count == 0)
                {
                    Console.WriteLine("No rows deleted");
                }
            }
        }

        private void FindModifiedRows(SqlConnection dbConnection, Hashtable Columns)
        {
            Console.WriteLine("--------Modified Values---------");
            using (SqlCommand command = dbConnection.CreateCommand())
            {

                command.CommandText = appConfiguration.ChangeCountQuery
                                      .Replace("@TABLE_1", appConfiguration.TablesConfig.FirstTableName)
                                      .Replace("@TABLE_2", appConfiguration.TablesConfig.SecondTableName)
                                      .Replace("@PK", appConfiguration.TablesConfig.PkName);

                int rowCount = Convert.ToUInt16(command.ExecuteScalar().ToString());

                if (rowCount == 0)
                {
                    Console.WriteLine("No data is modified");
                    return;
                }

                int totalPageCount = (int)Math.Ceiling((double)rowCount / appConfiguration.PageSize);
                int offset = 0;
                
                
                for (int pageNo = 1; pageNo <= totalPageCount; pageNo++)
                {
                    command.CommandText = appConfiguration.ChangeDetectQuery
                                      .Replace("@TABLE_1", appConfiguration.TablesConfig.FirstTableName)
                                      .Replace("@TABLE_2", appConfiguration.TablesConfig.SecondTableName)
                                      .Replace("@PK", appConfiguration.TablesConfig.PkName)
                                      .Replace("@OFFSET", offset.ToString())
                                      .Replace("@ROW_COUNT", appConfiguration.PageSize.ToString())
                                      ;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);

                        for (int i = 0; i < dataTable.Rows.Count; i = i + 2)
                        {
                            DataRow modifiedRow = dataTable.Rows[i];
                            DataRow oldRow = dataTable.Rows[i + 1];
                            PrintModifiedValues(modifiedRow, oldRow, Columns);
                        }

                    }
                    offset = offset + appConfiguration.PageSize;
                }

                

                
            }
        }

        private void PrintModifiedValues(DataRow modifiedRow, DataRow oldRow, Hashtable Columns)
        {
            Column idColumnRef = (Column)Columns[0];
            string rowId = GetDataByType(modifiedRow, idColumnRef.DataType, 0);

            for (int index = 1; index < modifiedRow.ItemArray.Count(); index++)
            {
                Column columnRef = (Column)Columns[index];
                string modifiedValue = GetDataByType(modifiedRow, columnRef.DataType, index);
                string oldValue = GetDataByType(oldRow, columnRef.DataType, index);

                if (modifiedValue != oldValue)
                {
                    Console.WriteLine(string.Format("{0} - {1} has been changed from '{2}' to '{3}'",
                                             rowId, columnRef.Name, oldValue, modifiedValue));
                }
            }
        }

        private void PrintColumnValues(SqlDataReader reader, Hashtable Columns, int count)
        {
            string columnValues = string.Empty;
            

            for (int i = 0; i < Columns.Values.Count; i++)
            {
                Column columnRef = (Column)Columns[i];
                columnValues = columnValues + ", " + GetDataByType(reader, columnRef.DataType, i);
            }

          
            Console.WriteLine(count + ". (" + columnValues.Substring(1).Trim() + ")");
        }

        private string GetDataByType(DataRow dataRow, string dataType, int index)
        {
            if (dataType.Contains("char") || dataType.Contains("text"))
                return dataRow[index].ToString();

            if (dataType.Equals("uniqueidentifier"))
                return ((Guid)dataRow[index]).ToString();

            if (dataType.Equals("int") || dataType.Equals("tinyint") || dataType.Equals("smallint"))
            {
                return ((int)dataRow[index]).ToString();
            }

            if (dataType.Equals("bigint"))
                return ((Int64)dataRow[index]).ToString();

            if (dataType.Contains("float"))
                return ((double)dataRow[index]).ToString();

            if (dataType.Contains("datetime"))
                return ((DateTime)dataRow[index]).ToString("dd/MM/yyyy hh:mm:ss zzz");

            return "data type not found";
        }

        private string GetDataByType(SqlDataReader reader, string dataType, int index)
        {
            if (dataType.Contains("char") || dataType.Contains("text"))
                return reader.GetString(index);

            if (dataType.Equals("uniqueidentifier"))
                return ((Guid)reader.GetValue(index)).ToString();

            if (dataType.Equals("int") || dataType.Equals("tinyint") || dataType.Equals("smallint"))
            {
                return ((int)reader.GetValue(index)).ToString();
            }

            if (dataType.Equals("bigint"))
                return reader.GetInt64(index).ToString();

            if (dataType.Contains("float"))
                return reader.GetDouble(index).ToString();

            if (dataType.Contains("datetime"))
                return reader.GetDateTime(index).ToString("dd/MM/yyyy hh:mm:ss zzz");

            return "data type not found";
        }

        private bool IsSchemaMatching(DatabaseObject firstTable, DatabaseObject secondTable)
        {
            return firstTable.CompareColumns(secondTable);
        }
    }
}
