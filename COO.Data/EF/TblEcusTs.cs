using System;
using System.Collections.Generic;

namespace COO.Data.EF
{
    public partial class TblEcusTs
    {
        public Guid Id { get; set; }
        public string SoTk { get; set; }
        public DateTime? NgayDk { get; set; }
        public string MaHs { get; set; }
        public string MaHang { get; set; }
        public string TenHang { get; set; }
        public decimal? DonGiaHd { get; set; }
        public string Country { get; set; }
        public DateTime? InsertedDate { get; set; }
    }
}
