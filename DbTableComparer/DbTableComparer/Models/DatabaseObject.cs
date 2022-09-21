using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbTableComparer.Models
{
    public abstract class DatabaseObject
    {
        public string Name { get; set; }
        public int Id { get; set; }
        private Hashtable Columns;

        public DatabaseObject(string name, int id)
        {
            this.Name = name.ToLower();
            this.Id = id;
            Columns = new Hashtable();
        }

        public void GetColumnData(SqlConnection conn, string columnFetchQuery)
        {
            using (SqlCommand command = conn.CreateCommand())
            {
                command.CommandText = columnFetchQuery;
                command.Parameters.AddWithValue("@TABLE_ID", this.Id);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Columns[reader.GetString(0)] = new Column(reader.GetString(0), reader.GetByte(1),
                            reader.GetInt16(2), reader.GetByte(3));
                    }
                }
            }
        }

        public bool CompareColumns(DatabaseObject referenceTable)
        {
            if (this.Columns.Values.Count != referenceTable.Columns.Values.Count)
                return false;

            foreach (Column column in this.Columns.Values)
            {
                Column referenceColumn = referenceTable.Columns[column.Name] as Column;
                if (referenceColumn == null)
                    return false;
                if (!column.CompareWith(referenceColumn))
                    return false;
            }
            return true;
        }
    }
}
