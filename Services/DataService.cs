using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Viewer.Data;

namespace Viewer.Services
{
    /// <summary>
    /// Works with database
    /// </summary>
    public static class DataService
    {
        /// <summary>
        /// Gets records of A0Protocol type for selected year from database
        /// or (if a year is not set) gets all records of A0Protocol type from database
        /// </summary>
        /// <param name="year"> year to filter records </param>
        /// <returns> filtered 'A0Protocol' records </returns>
        public static IEnumerable<A0Protocol> GetProtocols(int? year = null)
        {
            using (var db = new DatabaseContext())
            {
                var protocols = year == null ? db.A0Protocols.ToList() : db.A0Protocols.Where(o => o.EvDate.Year == year).ToList();
                return protocols;
            }

        }

        /// <summary>
        /// Gets from database all years that appear in column 'EvDate' of 'A0Protocol' table
        /// </summary>
        /// <returns> all possible years for A0Protocol </returns>
        public static ObservableCollection<int> GetAllYears()
        {
            using (var db = new DatabaseContext())
            {
                var years = db.A0Protocols.GroupBy(o => o.EvDate.Year)
                    .OrderBy(s => s.Key)
                    .Select(p => p.Key).ToList();
                return new ObservableCollection<int>(years);
            }
        }
    }
}
