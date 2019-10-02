using System;

namespace Viewer.Models
{
    /// <summary>
    /// The leaf of the tree
    /// </summary>
    public class DescriptionNode: Node
    {
        public DateTime EvDate { get; }
        public int ProjId { get; }
        public int SmObjId { get; }
        public string LogText { get; }

        public DescriptionNode(DateTime evDate, int projId, int smObjId, string logText)
        {
            EvDate = evDate;
            ProjId = projId;
            SmObjId = smObjId;
            LogText = logText;
            Name = $"{EvDate:yyyy'/'MM'/'dd'/'HH:mm:ss}_{ProjId}_{SmObjId}_{LogText}";
        }
    }
}
