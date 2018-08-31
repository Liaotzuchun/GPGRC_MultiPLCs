using System;
using System.IO;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace GPGO_MultiPLCs.Helpers
{
    /// <summary>提供任何地方紀錄的靜態類別</summary>
    public static class ErrorRecoder
    {
        private const string Path = "C:\\GP\\Errors\\";
        private static readonly object Lock_Obj = new object();

        /// <summary>紀錄例外</summary>
        /// <param name="ex">例外</param>
        /// <param name="note">附註</param>
        public static void RecordError(this Exception ex, string note = "")
        {
            //! 避免同一時間大量的例外紀錄(通常發生於多執行緒的平行動作，即同一種例外發生多次)
            if (!Monitor.TryEnter(Lock_Obj))
            {
                return;
            }

            try
            {
                if (!Directory.Exists(Path))
                {
                    Directory.CreateDirectory(Path);
                }

                var str = new StringBuilder();
                str.Append(DateTime.Now.ToShortDateString().Replace("/", "-"));
                str.Append(".log");
                var filename = Path + str;

                var temp = new Error(DateTime.Now, note, ex);

                var json = JsonConvert.SerializeObject(temp, Formatting.Indented);
                var sw = File.AppendText(filename);
                sw.WriteLine(json);
                sw.Flush();
                sw.Close(); //!colse本身即呼叫了dispose，不需另外dispose
            }
            catch
            {
            }
            finally
            {
                Monitor.Exit(Lock_Obj);
            }
        }

        internal class Error
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
    }
}