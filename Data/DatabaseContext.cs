using System.Data.Entity;

namespace Viewer.Data
{
    public class DatabaseContext: DbContext
    {
        public DatabaseContext(): 
            base("ConnectionString") { }
        
        public DbSet<A0Protocol> A0Protocols { get; set; }
    }
}
