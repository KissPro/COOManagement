using System;
using System.Collections.Generic;

namespace COO.Data.EF
{
    public partial class TblConfig
    {
        public Guid Id { get; set; }
        public string EcusRuntime { get; set; }
        public string Dsruntime { get; set; }
        public int DstimeLastMonth { get; set; }
        public int DstimeNextMonth { get; set; }
        public int DstimeLastYear { get; set; }
        public int DstimeNextYear { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string RemarkConfig { get; set; }
    }
}
