using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viewer.Data
{
    public class DatabaseContext: DbContext
    {
        public DatabaseContext(): 
            base("ConnectionString") { }
        
        public DbSet<A0Protocol> A0Protocols { get; set; }

    }
}
