using System;
using System.IO;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace GPGO_MultiPLCs.Helpers
{
    public static class ErrorRecoder
    {
        private static readonly object Lock_Obj = new object();

        public class Error
        {
            public Exception Except;
            public string NoteStr;
            public DateTime Updated;

            public Error(DateTime updated, string noteStr, Exception except)
            {
                Updated = updated;
                NoteStr = noteStr;
                Except = except;
            }
        }

        public static void RecordError(Exception ex, string note = "")
        {
            if (!Monitor.TryEnter(Lock_Obj))
            {
                return;
            }

            try
            {
                if (!Directory.Exists("C:\\GP\\Errors\\"))
                {
                    Directory.CreateDirectory("C:\\GP\\Errors");
                }

                var str = new StringBuilder();
                str.Append(DateTime.Now.ToShortDateString().Replace("/", "-"));
                str.Append(".log");
                var filename = "C:\\GP\\Errors\\" + str;

                var temp = new Error(DateTime.Now, note, ex);

                var json = JsonConvert.SerializeObject(temp, Formatting.Indented);
                using (var sw = File.AppendText(filename))
                {
                    sw.WriteLine(json);
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                Monitor.Exit(Lock_Obj);
            }
        }
    }
}