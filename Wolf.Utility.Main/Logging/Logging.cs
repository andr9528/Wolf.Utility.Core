using System;
using System.IO;
using System.Runtime.CompilerServices;
using Wolf.Utility.Main.Logging.Enum;

namespace Wolf.Utility.Main.Logging
{
    public class Logging
    {
        private static string _path;
        private static bool _shouldLog;

        public static void Init(string path, bool shouldLog = true)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentException("Value cannot be null or empty.", nameof(path));
            
            if (string.IsNullOrEmpty(_path))
            {
                _path = path;
                _shouldLog = shouldLog;
            }
        }

        public static void Init(bool shouldLog)
        {
            _shouldLog = shouldLog;
        }

        /// <summary>
        /// If an Exception is throw trying to Log, it will be returned to the caller, gracefully, meaning the application will not crash, just fail to log.
        ///  </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static (bool didLog, Exception exception) Log(LogType type, string message)
        {
            if (_shouldLog)
            {
                try
                {
                    if (string.IsNullOrEmpty(_path))
                        throw new NullReferenceException(
                            $"{nameof(_path)} was null. Remember to call Init at start of the application");

                    using (var writer = new StreamWriter(_path, true))
                    {
                        string line = $"{LogTabulator(type)} {message}";

                        writer.WriteLine(line);
                    }

                    return (true, null);
                }
                catch (Exception e)
                {
                    return (false, e);
                }
            }

            return (false, null);
        }


        /// <summary>
        /// If an Exception is throw trying to Log, it will be returned to the caller, gracefully, meaning the application will not crash, just fail to log.
        /// </summary>
        /// <returns></returns>
        public static (bool didLog, Exception exception) AppStart()
        {
            if (_shouldLog)
            {
                try
                {
                    if (string.IsNullOrEmpty(_path))
                        throw new NullReferenceException(
                            $"{nameof(_path)} was null. Remember to call Init at start of the application");

                    string appStart = $"{LogTabulator(LogType.Information)} App Start Time ";

                    if (!File.Exists(_path))
                    {
                        new FileStream(_path, FileMode.Create).Dispose();

                        using (var writer = new StreamWriter(_path, true))
                        {
                            writer.WriteLine(appStart);
                        }
                    }
                    else
                    {
                        using (var writer = new StreamWriter(_path, true))
                        {
                            writer.WriteLine();
                            writer.WriteLine("==================");
                            writer.WriteLine(appStart);
                        }
                    }

                    return (true, null);
                }
                catch (Exception e)
                {
                    return (false, e);
                }
            }

            return (false, null);
        }

        private static string LogTabulator(LogType type)
        {
            string result = type.ToString().ToUpper();

            if (result.Length < 8) result += "\t \t";
            else if (result.Length < 16) result += "\t";

            result += DateTime.Now;
            result += "\t";

            return result;
        }
    }
}