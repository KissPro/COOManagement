using COO.ViewModels.Common;
using SAP.Middleware.Connector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace COO.Application.Common
{
    public interface ISAPService
    {
        RfcConfigParameters Connector3SapRfcConnGroup(string connStr);
        DataTable ConvertToDoNetTable(IRfcTable RFCTable);
        PartInfo GetAvailableStock(string line, string part);

        string CreateTO(string fromLine, string toLine, string fromBin, int number, string part, string plant, int qty);

        string ConfirmTO(DataVM entity, string confirm);

        DataTable DownloadTODetail(string toNumber);

        DataTable DownloadDeliverySale(string plant, DateTime firstDate, DateTime lastDate);

        DataTable DownloadBoom(string plant, string parentMaterial);

        DataTable GetListTO(DateTime from, DateTime to);

        string TestSAPCore();
    }
}
