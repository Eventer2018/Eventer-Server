using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace EventerAPI.General
{
    public static class Logger
    {
        const string _log_path = @"C:\Temp\find_log.log";


        public static void Write(string message) {
            using (StreamWriter streamWriter = new StreamWriter(_log_path,true))
            {

                streamWriter.WriteLine(string.Format("{0} - {1}", DateTime.Now, message));

                streamWriter.Close();

            }   
        
        }

    }
}