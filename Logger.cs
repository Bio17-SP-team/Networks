using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
        static StreamWriter sr = new StreamWriter("log.txt");
        public static void LogException(Exception ex)
        {
            // TODO: Create log file named log.txt to log exception details in it
            //Datetime:
            //message:
            // for each exception write its details associated with datetime 
            DateTime now = DateTime.Now;
            //Console.WriteLine(now);
            sr.Write("DateTime : ");
            sr.WriteLine(now);
            sr.Write("Message : ");
            sr.WriteLine(ex.Message);
            sr.Close();
        }
    }
}
