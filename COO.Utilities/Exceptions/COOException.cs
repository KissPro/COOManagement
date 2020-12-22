using System;
using System.Collections.Generic;
using System.Text;

namespace COO.Utilities.Exceptions
{
    public class COOException : Exception
    {
        public COOException() { }

        public COOException(string message) : base(message) { }

        public COOException(string message, Exception ex) : base(message, ex) {
        }
    }
}
