using System;
using System.Collections.Generic;
using System.Text;

namespace COO.ViewModels.MainFunction.DN
{
    public class COOModel
    {
        // DN Information 
        public Guid Id { get; set; }
        public long Delivery { get; set; }
        public long InvoiceNo { get; set; }
        public string MaterialParent { get; set; }
        public string MaterialDesc { get; set; }
        public string ShipToCountry { get; set; }
        public string ShipToCountryName { get; set; } // From config table
        public string HMDShipToCode { get; set; } // From config table
        public string PartyName { get; set; }
        public long? CustomerInvoiceNo { get; set; }
        public string SaleUnit { get; set; }
        public DateTime ActualGidate { get; set; }
        public decimal NetValue { get; set; }
        public long Dnqty { get; set; }
        public decimal NetPrice { get; set; }
        public string HarmonizationCode { get; set; }
        public string Address { get; set; }
        public string Plant { get; set; }
        public DateTime PlanGidate { get; set; }
        public DateTime PlanGisysDate { get; set; }
        public DateTime InsertedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? Status { get; set; }

        // DS Manual Information
        public Guid IdManual { get; set; }
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
        public DateTime? UpdatedDateManual { get; set; }
        public string RemarkDs { get; set; }
    }
}
