using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbTableComparer.Models
{
    public class DatabaseTable : DatabaseObject
    {
        public DatabaseTable(string name, int id) : base(name, id)
        {
        }
    }
}
