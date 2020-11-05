using System;
using System.Collections.Generic;
using System.Text;

namespace COO.ViewModels.Common
{
    public class PartInfo
    {
        public string Plant { get; set; }
        public int Qty { get; set; }
    }
    public class DataVM
    {
        public long TO { get; set; }
        public int TO_ORDER { get; set; }
        public string BIN { get; set; }
        public string MATERIAL { get; set; }
        public int QTY { get; set; }
        public string UNIT { get; set; }
    }
}
