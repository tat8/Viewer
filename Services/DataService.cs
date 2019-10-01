using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viewer.Data;

namespace Viewer.Services
{
    public static class DataService
    {
        public static List<A0Protocol> GetProtocols(int? year = null)
        {
            using (var db = new DatabaseContext())
            {
                var protocols = year == null ? db.A0Protocols.ToList() : db.A0Protocols.Where(o => o.EvDate.Year == year).ToList();
                return protocols;
            }
        }

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
