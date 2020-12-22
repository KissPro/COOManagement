using COO.Data.EF;
using System;
using System.Collections.Generic;
using System.Text;

namespace COO.ViewModels.Common
{
    public class FileRespondModel
    {
        public string path { get; set; }
        public string userId { get; set; }
    }

    public class ExportCOOModel
    {
        public List<TblDeliverySales> Dn { get; set; }
        public string Ship { get; set; }
        public string CooNo { get; set; }

        public string PackageNo { get; set; }
    }
}
