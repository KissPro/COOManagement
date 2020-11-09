using System;
using System.Collections.Generic;

namespace COO.Data.EF
{
    public partial class TblPlant
    {
        public Guid Id { get; set; }
        public string Plant { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string RemarkPlant { get; set; }
    }
}
