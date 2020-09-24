namespace QNMCL
{
    using System;
    using System.IO;

    public static class Logger
    {
        private static readonly TextWriter LogWriter = File.CreateText("QNMCL.log");

        public static void End()
        {
            TextWriter logWriter = LogWriter;
            lock (logWriter)
            {
                LogWriter.Flush();
                LogWriter.Dispose();
            }
        }

        public static void Log(string log)
        {
            TextWriter logWriter = LogWriter;
            lock (logWriter)
            {
                LogWriter.WriteLine(log);
            }
        }
    }
}

