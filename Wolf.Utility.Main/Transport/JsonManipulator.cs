using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wolf.Utility.Main.Exceptions;
using Wolf.Utility.Main.Logging;
using Wolf.Utility.Main.Logging.Enum;
using ZXing;

namespace Wolf.Utility.Main.Transport
{
    public class JsonManipulator
    {
        public static T ReadValue<T>(string path, string propertyName)
        {
            if (!File.Exists(path))
                throw new ArgumentNullException(nameof(path), $"No file Exist on the specified path => {path}");

            if (path.Split('.').Last().ToLowerInvariant() != "json")
                throw new ArgumentException("The path given did not end in 'json'");

            var json = File.ReadAllText(path);
            
            try
            {
                var jobject = JObject.Parse(json);
                if (jobject != null)
                    return jobject[propertyName].ToObject<T>();
                throw new OperationFailedException($"Failed to parse Json from the file located at => {path}");

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static void WriteValue<T>(string path, string propertyName, T value, bool allowUpdate = true)
        {
            if (!File.Exists(path))
                throw new ArgumentNullException(nameof(path), $"No file Exist on the specified path => {path}");

            if ((path.Split('.')).Last().ToLowerInvariant() != "json")
                throw new ArgumentException("The path given did not end in 'json'");

            var json = File.ReadAllText(path);

            try
            {
                var jobject = new JObject();
                try
                {
                    jobject = JObject.Parse(json);
                }
                catch (Exception ex)
                {
                    Logging.Logging.Log(LogType.Warning,
                        $"Failed to parse Json from the file located at => {path}; Continuing with a default JObject ");
                }

                var jproperty = jobject.Property(propertyName);
                var str = JsonConvert.SerializeObject(value, Formatting.Indented);

                if (jproperty == null)
                    jobject.Add(propertyName, str);
                else if (allowUpdate)
                    jproperty.Value = str;

                File.WriteAllText(path, jobject.ToString());
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public static void InitializeJsonDoc(string path, params (string propertyName, object value)[] elements)
        {
            if ((path.Split('.')).Last().ToLowerInvariant() != "json")
                throw new ArgumentException("The path given did not end in 'json'");
            
            if (!File.Exists(path))
                File.Create(path).Dispose();

            foreach (var (propertyName, value) in elements)
            {
                WriteValue(path, propertyName, value, false);
            }
        }
    }
}
