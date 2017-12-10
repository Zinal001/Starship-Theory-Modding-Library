using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StarshipTheory.ModLib
{
    public class ModLoggerHandler : ILogHandler
    {
        private System.Object _loggerLock;
        private String logFile;

        /// <summary>
        /// Should this logger also output to the global output_log.txt file?
        /// </summary>
        public bool OutputToGlobal { get; set; } = false;

        internal ModLoggerHandler(Mod mod)
        {
            _loggerLock = new System.Object();
            logFile = System.IO.Path.Combine(mod.ModFolder, "log.txt");
        }

        public void LogException(Exception exception, UnityEngine.Object context)
        {
            LogFormat(LogType.Exception, context, ExceptionToMessage(exception));
        }

        public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
        {
            lock(_loggerLock)
            {
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(this.logFile))
                {
                    String contextName = context == null ? "" : (context.name + " ");

                    writer.WriteLine(String.Format("[{0}]{1} {2}", logType.ToString(), contextName, String.Format(format, args)));
                }
            }

            if (OutputToGlobal)
                Debug.logger.LogFormat(logType, context, format, args);
        }

        private String ExceptionToMessage(Exception ex, int indentC = 0)
        {
            String indent = "";
            for (int i = 0; i < indentC; i++)
                indent += "  ";

            String str = indent + "Exception Type: " + ex.GetType().Name + "\n";
            str += indent + "Message: " + ex.Message + "\n";
            if (ex.Data != null && ex.Data.Count > 0)
            {
                str += indent + "Data:\n";
                foreach (System.Object Key in ex.Data.Keys)
                    str += indent + "  " + Key.ToString() + " = " + ex.Data[Key].ToString() + "\n";
            }

            if (!String.IsNullOrEmpty(ex.Source))
                str += indent + "Source: " + ex.Source + "\n";

            if (!String.IsNullOrEmpty(ex.StackTrace))
                str += indent + "Stacktrace: " + ex.StackTrace + "\n";

            if (ex is TypeLoadException)
                str += indent + "Type Name: " + ((TypeLoadException)ex).TypeName + "\n";
            else if (ex is System.Reflection.ReflectionTypeLoadException)
            {
                System.Reflection.ReflectionTypeLoadException rex = (System.Reflection.ReflectionTypeLoadException)ex;

                if (rex != null)
                {
                    if (rex.LoaderExceptions != null && rex.LoaderExceptions.Length > 0)
                    {
                        str += indent + "Loader exceptions:\n";
                        foreach (Exception subEx in rex.LoaderExceptions)
                        {
                            if (subEx != null)
                                str += ExceptionToMessage(subEx, indentC + 1);
                        }
                    }
                }
            }

            if (ex.InnerException != null)
            {
                str += indent + "Inner Exception:\n";
                str += ExceptionToMessage(ex.InnerException, indentC + 1);
            }

            return str;
        }
    }
}
