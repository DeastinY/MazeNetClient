using System;
using System.Diagnostics;
using System.IO;

namespace MazeNetClient
{
    static class Logger
    {
        const string FilePath = "MazeNetClient_Logfile.txt";

        const string NewLine = "\r\n";

        static Logger()
        {
            try
            {
                if (File.Exists(FilePath))
                    File.Delete(FilePath);
            }
            catch (Exception ex)
            {
                ShowException(ex);
            }
        }

        internal static void WriteLine(string value, bool writeToFile = false)
        {
            if (writeToFile)
            {
                WriteToFile(value);
            }
            Console.WriteLine(value);
        }

        internal static void WriteLine()
        {
            Console.WriteLine();
        }

        internal static void Write(string value)
        {
            Console.Write(value);
        }

        internal static void WriteToFile(string value)
        {
            File.AppendAllText(FilePath, value + NewLine);
        }

        internal static void ShowException(Exception ex)
        {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            WriteLine(ex.Message);
            Console.ForegroundColor = previousColor;

            WriteToFile(ex.Message);
        }

        [Conditional("DEBUG")]
        internal static void DebugWriteLine(string value, bool writeToFile = false)
        {
            WriteLine(value, writeToFile);
        }
    }
}