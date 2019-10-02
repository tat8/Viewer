using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Viewer.Data
{
    public class A0Protocol
    {
        [Key, Column(Order = 0)]
        public DateTime EvDate { get; set; }

        [Key, Column(Order = 1)]
        public string Login { get; set; }

        public int Oper { get; set; }
        public int ProjId { get; set; }
        public int SmObjId { get; set; }
        public int SmType { get; set; }
        public string LogText { get; set; }
    }
}
