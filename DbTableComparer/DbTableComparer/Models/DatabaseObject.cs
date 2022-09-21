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
        public Hashtable Columns;

        public DatabaseObject(string name, int id)
        {
            this.Name = name.ToLower();
            this.Id = id;
            Columns = new Hashtable();
        }

        public void GetColumnData(SqlConnection conn, string columnFetchQuery)
        {
            int index = 0;
            using (SqlCommand command = conn.CreateCommand())
            {
                command.CommandText = columnFetchQuery;
                command.Parameters.AddWithValue("@TABLE_ID", this.Id);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Columns[index] = new Column(reader.GetString(0), reader.GetByte(1),
                            reader.GetInt16(2), reader.GetByte(3), reader.GetString(4));
                        index++;
                    }
                }
            }
        }

        public bool CompareColumns(DatabaseObject referenceTable)
        {
            if (this.Columns.Values.Count != referenceTable.Columns.Values.Count)
                return false;

            for (int index = 0; index < this.Columns.Values.Count; index++)
            {
                Column column = (Column)this.Columns[index];
                Column referenceColumn = referenceTable.Columns[index] as Column;
                
                if (referenceColumn == null)
                    return false;
                if (!column.CompareWith(referenceColumn))
                    return false;
            }
            return true;
        }
    }
}
