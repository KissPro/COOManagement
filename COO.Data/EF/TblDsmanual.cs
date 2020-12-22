using System;
using System.Collections.Generic;

namespace COO.Data.EF
{
    public partial class TblDsmanual
    {
        public Guid Id { get; set; }
        public Guid DeliverySalesId { get; set; }
        public string Coono { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Cooform { get; set; }
        public string TrackingNo { get; set; }
        public DateTime? CourierDate { get; set; }
        public DateTime? TrackingDate { get; set; }
        public string Origin { get; set; }
        
        public string ShipFrom { get; set; }
        public string Package { get; set; }

        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string RemarkDs { get; set; }

        public virtual TblDeliverySales DeliverySales { get; set; }
    }
}
