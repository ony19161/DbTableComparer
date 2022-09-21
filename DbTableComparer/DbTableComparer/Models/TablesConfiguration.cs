using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbTableComparer.Models
{
    public class TablesConfiguration
    {
        public string FirstTableName { get; set; }
        public string SecondTableName { get; set; }
        public string PkName { get; set; }
    }
}
