using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbTableComparer.Models
{
    public class Column
    {
        public Column(string name, int type, int length, int scale, string dataType)
        {
            Name = name;
            Type = type;    
            Length = length;
            Scale = scale;
            DataType = dataType;
        }

        public string  Name { get; set; }
        public int Type { get; set; }
        public int Length { get; set; }
        public int Scale { get; set; }
        public string DataType { get; set; }

        public bool CompareWith(Column referenceColumn)
        {
            return this.Name == referenceColumn.Name && 
                   this.Type == referenceColumn.Type &&
                   this.Length == referenceColumn.Length && 
                   this.Scale == referenceColumn.Scale;
        }

    }
}
