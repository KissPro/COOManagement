using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace COO.Utilities.Constants
{
    public class SystemConstants
    {
        public const string MainConnectionString = "COO";

        // SAP Connection string
        // 1. QA
        public const string SAPConnectionString = @"MSHOST=10.134.92.27 R3NAME=LH1 GROUP=SPACE CLIENT=802 USER=RFCSHARE02 PASSWD=it0215 LANG=EN";
        // 2. PRD 
        //public const string SAPConnectionString = "MSHOST=10.134.28.98 R3NAME=LH1 GROUP=SPACE CLIENT=802 USER=RFCSHARE02 PASSWD=it0215 LANG=EN";

        public class AppSettings
        {
            public const string DefaultLanguageId = "DefaultLanguageId";
            public const string Token = "User";
            public const string BaseAddress = "BaseAddress";
        }

        public class ProductSettings
        {
            public const int NumberOfFeaturedProducts = 4;
            public const int NumberOfLatestProducts = 6;
        }
    }
}
