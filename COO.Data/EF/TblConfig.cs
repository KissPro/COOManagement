using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;

namespace COO.Data.EF
{
    public partial class TblConfig
    {
        public Guid Id { get; set; }

        public string Key { get; set;}
        public string Value { get; set; }
        public string RemarkConfig { get; set; }

        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
