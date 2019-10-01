using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viewer.Models
{
    public class DescriptionNode: Node
    {
        public DateTime EvDate { get; }
        public int ProjID { get; }
        public int SmObjID { get; }
        public string LogText { get; }
        private new ObservableCollection<Node> Nodes { get; set; }

        public DescriptionNode(DateTime evDate, int projID, int smObjID, string logText)
        {
            EvDate = evDate;
            ProjID = projID;
            SmObjID = smObjID;
            LogText = logText;
            Name = $"{EvDate:yyyy'/'MM'/'dd'/'HH:mm:ss}_{ProjID}_{SmObjID}_{LogText}";
        }
    }
}
