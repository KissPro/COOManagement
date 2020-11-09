using System;
using System.Collections.Generic;

namespace COO.Data.EF
{
    public partial class TblCountryShip
    {
        public Guid Id { get; set; }
        public string HMDShipToCode { get; set; }
        public string HMDShipToParty { get; set; }
        public string ShipToCountryCode { get; set; }
        public string ShipToCountryName { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string RemarkCountry { get; set; }
    }
}
