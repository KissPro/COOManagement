using System;
using System.Collections.Generic;

namespace COO.Data.EF
{
    public partial class TblBoom
    {
        public Guid Id { get; set; }
        public string ParentMaterial { get; set; }
        public string Material { get; set; }
        public int Quantity { get; set; }
        public string SortString { get; set; }
        public string AltGroup { get; set; }
        public string Plant { get; set; }
        public string Description { get; set; }
        public DateTime? InsertedDate { get; set; }
    }
}
