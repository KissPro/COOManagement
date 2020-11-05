﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace COO.Utilities.Constants
{
    public class WriteLog
    {
        public static void Write(string message, Exception ex)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt", true);
                sw.WriteLine(DateTime.Now.ToString("g") + ": " + message + "-" + ex.ToString());
                sw.Flush();
                sw.Close();
            }
            catch
            {
                // ignored
            }
        }
        public static void Write(string message, string ex)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt", true);
                sw.WriteLine(DateTime.Now.ToString("g") + ": " + message + "-" + ex);
                sw.Flush();
                sw.Close();
            }
            catch
            {
                // ignored
            }
        }
    }
}
