using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbTableComparer.Models
{
    public class AppConfiguration
    {
        public string ConnectionString { get; set; }
        public string FindTablesQuery { get; set; }
        public string AddedRowsQuery { get; set; }
        public string DeletedRowsQuery { get; set; }
        public string ChangeDetectQuery { get; set; }
        public string PageSize { get; set; }
        public TablesConfiguration TablesConfig { get; set; }
    }
}
