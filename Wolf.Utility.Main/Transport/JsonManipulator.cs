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
        /// <summary>
        /// Parses Json file, and returns the attribute specified by 'propertyName'.
        /// </summary>
        /// <typeparam name="T">The type returned, from the property named after the input 'propertyName'.</typeparam>
        /// <param name="path">Path of the Json file.</param>
        /// <param name="propertyName">Name of the property to return.</param>
        /// <returns></returns>
        public static T ReadValue<T>(string path, string propertyName)
        {
            if (!File.Exists(path))
                throw new ArgumentNullException(nameof(path), $@"No file Exist on the specified path => {path}");

            if (path.Split('.').Last().ToLowerInvariant() != "json")
                throw new ArgumentException("The path given did not end in 'json'");

            
            
            try
            {
                var json = File.ReadAllText(path);

                var obj = JObject.Parse(json);
                if (obj != null)
                    return obj[propertyName].ToObject<T>();
                throw new OperationFailedException($"Failed to parse Json from the file located at => {path}");

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        /// <summary>
        /// Deserializes Json file into specified model, and returns the property from it by the value specified in 'propertyName'.
        /// </summary>
        /// <typeparam name="T">The type returned, from the property named after the input 'propertyName'.</typeparam>
        /// <typeparam name="U">The model that is deserialized into, and from which the property is taken and returned.</typeparam>
        /// <param name="path">Path of the Json file.</param>
        /// <param name="propertyName">Name of the property to return.</param>
        /// <returns></returns>
        public static T ReadValueViaModel<T, U>(string path, string propertyName) where U : class, ISettingsModel
        {
            if (!File.Exists(path))
                throw new ArgumentNullException(nameof(path), $@"No file Exist on the specified path => {path}");

            if (path.Split('.').Last().ToLowerInvariant() != "json")
                throw new ArgumentException("The path given did not end in 'json'");

            try
            {
                var json = File.ReadAllText(path);

                var obj = JsonConvert.DeserializeObject<U>(json, new JsonSerializerSettings() 
                { 
                    NullValueHandling = NullValueHandling.Ignore, 
                    TypeNameHandling = TypeNameHandling.All
                });

                var prop = obj.GetType().GetProperties().First(x => x.Name == propertyName);

                return (T)prop.GetValue(obj);
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// Persists values to a Json file under the specified 'propertyName'. 
        /// </summary>
        /// <typeparam name="T">The type of the value given to the method.</typeparam>
        /// <param name="path">Path of the Json file.</param>
        /// <param name="propertyName">Name of the property to update or create in the file.</param>
        /// <param name="value">The value to put into the Json file, under the Attribute name defined by 'propertyName'.</param>
        /// <param name="allowUpdate">Limit to whether or not the method is allowed to update existing values or only create new Attributes.</param>
        public static void WriteValue<T>(string path, string propertyName, T value, bool allowUpdate = true)
        {
            if (!File.Exists(path))
                throw new ArgumentNullException(nameof(path), $@"No file Exist on the specified path => {path}");

            if ((path.Split('.')).Last().ToLowerInvariant() != "json")
                throw new ArgumentException("The path given did not end in 'json'");

            try
            {
                var json = File.ReadAllText(path);

                var obj = new JObject();
                try
                {
                    obj = JObject.Parse(json);
                }
                catch (Exception ex)
                {
                    Logging.Logging.Log(LogType.Warning,
                        $"Failed to parse Json from the file located at => {path}; Continuing with a default JObject ");
                }

                var property = obj.Property(propertyName);
                var str = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.All
                });

                if (property == null)
                    obj.Add(propertyName, str);
                else if (allowUpdate)
                    property.Value = str;

                File.WriteAllText(path, obj.ToString());
            }
            catch (Exception ex)
            {
                throw;  
            }
        }

        public static void WriteModel<U>(string path, U newModel, params string[] propertyNames) where U : class, ISettingsModel
        {
            if (!File.Exists(path))
                throw new ArgumentNullException(nameof(path), $@"No file Exist on the specified path => {path}");

            if ((path.Split('.')).Last().ToLowerInvariant() != "json")
                throw new ArgumentException("The path given did not end in 'json'");
            
            try
            {
                var oldJson = File.ReadAllText(path);

                var oldModel = JsonConvert.DeserializeObject<U>(oldJson,
                    new JsonSerializerSettings() {TypeNameHandling = TypeNameHandling.All});

                foreach (var name in propertyNames)
                {
                    var prop = newModel.GetType().GetProperties().First(x => x.Name == name);

                    prop.SetValue(oldModel, prop.GetValue(newModel));
                }

                var newJson = JsonConvert.SerializeObject(oldModel, new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.All
                });

                File.WriteAllText(path, newJson);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public static void InitializeJsonDoc(string path, params (string propertyName, object value)[] defaultElements)
        {
            if ((path.Split('.')).Last().ToLowerInvariant() != "json")
                throw new ArgumentException("The path given did not end in 'json'");
            
            if (!File.Exists(path))
                File.Create(path).Dispose();

            foreach (var (propertyName, value) in defaultElements)
            {
                WriteValue(path, propertyName, value, false);
            }
        }
    }
}
