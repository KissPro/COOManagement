using System;
using System.Collections.Generic;

namespace COO.Data.EF
{
    public partial class TblCountryShip
    {
        public Guid Id { get; set; }
        public string ShipId { get; set; }
        public string CountryName { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string RemarkCountry { get; set; }
    }
}
