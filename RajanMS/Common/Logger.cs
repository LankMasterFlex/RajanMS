using System;
using System.Collections.Generic;
using System.IO;

namespace Common
{
    public enum LogLevel : byte
    {
        Error = 0,
        Warning,
        Info,
        Connection,
        DataLoad,
        Server,
    }

    public static class Logger
    {
        private static object sLocker = new object();

        private static readonly Dictionary<LogLevel, ConsoleColor> sLogColors = new Dictionary<LogLevel, ConsoleColor>
        {
            { LogLevel.Error,       ConsoleColor.Red        },
            { LogLevel.Warning,     ConsoleColor.Yellow     },
            { LogLevel.Info,        ConsoleColor.Blue       },
            { LogLevel.Connection,  ConsoleColor.Green      },
            { LogLevel.DataLoad,    ConsoleColor.Cyan       },
            { LogLevel.Server,      ConsoleColor.Magenta    },
        };

        private static readonly Dictionary<LogLevel, string> sLogNames = new Dictionary<LogLevel, string>
        {
            { LogLevel.Error,       "[Error] "      },
            { LogLevel.Warning,     "[Warning] "    },
            { LogLevel.Info,        "[Info] "       },
            { LogLevel.Connection,  "[Connection] " },
            { LogLevel.DataLoad,    "[DataLoad] "   },
            { LogLevel.Server,      "[Server] "     },
        };

        public static void InitConsole(string title)
        {
            Console.Title = title;
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Write(LogLevel logLevel, string format, params object[] objects)
        {
            lock (sLocker)
            {
                Console.ForegroundColor = sLogColors[logLevel];
                Console.Write(sLogNames[logLevel]);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(format, objects);
            }
            if (logLevel == LogLevel.Error)
                Console.Beep();
        }
        public static void Exception(Exception ex)
        {
            string exception = ex.ToString();
            string message = string.Format("[{0}]-----------------{1}{2}{1}", DateTime.Now, Environment.NewLine, exception);

            lock (sLocker)
            {
                File.AppendAllText("EXCEPTIONS.txt", message);
            }

            //Keep outside to prevent a deadlock.
            Write(LogLevel.Error, "An exception was logged{0}{1}", Environment.NewLine, exception);
        }

        public static void Initializer(string message, Action function)
        {
            int startTime = Environment.TickCount;
            Console.Write(message);
            Console.Write("... ");
            
            function();

            Console.Write(Environment.TickCount - startTime);
            Console.Write("ms");

            Console.SetCursorPosition(Console.WindowWidth - 7, Console.CursorTop);
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("DONE");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("]");
        }
    }
}
