using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wolf.Utility.Core.Exceptions;
using Wolf.Utility.Core.Logging;
using Wolf.Utility.Core.Logging.Enum;

namespace Wolf.Utility.Core.Transport
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
        public static T ReadValueViaPath<T>(string path, string propertyName)
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
        public static T ReadValueViaModelViaPath<T, U>(string path, string propertyName) where U : class, ISettingsModel
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

        public static U ReadModelFormPath<U>(string path) where U : class, ISettingsModel
        {
            if (!File.Exists(path))
                throw new ArgumentNullException(nameof(path), $@"No file Exist on the specified path => {path}");

            if ((path.Split('.')).Last().ToLowerInvariant() != "json")
                throw new ArgumentException("The path given did not end in 'json'");

            try
            {
                var json = File.ReadAllText(path);

                var model = JsonConvert.DeserializeObject<U>(json,
                    new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });

                return model;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public static void WriteModelToPath<U>(string path, U newModel, bool onlyUpdateIfDefault = false, params string[] propertyNames) where U : class, ISettingsModel
        {
            if (!File.Exists(path))
                throw new ArgumentNullException(nameof(path), $@"No file Exist on the specified path => {path}");

            if ((path.Split('.')).Last().ToLowerInvariant() != "json")
                throw new ArgumentException("The path given did not end in 'json'");
            
            try
            {
                if (propertyNames.Length == 0)
                {
                    SerializeAndWriteModel(path, newModel);
                }
                else
                {
                    var oldJson = File.ReadAllText(path);

                    var oldModel = JsonConvert.DeserializeObject<U>(oldJson,
                        new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All }) ?? Activator.CreateInstance<U>();

                    foreach (var name in propertyNames)
                    {
                        var prop = newModel.GetType().GetProperties().First(x => x.Name == name);

                        var isEmptyOrDefault = oldJson.Length == 0 || prop.GetValue(oldModel) == default;

                        if (isEmptyOrDefault && onlyUpdateIfDefault)
                        {
                            prop.SetValue(oldModel, prop.GetValue(newModel));
                        }
                        else if (!onlyUpdateIfDefault)
                        {
                            prop.SetValue(oldModel, prop.GetValue(newModel));
                        }
                    }

                    SerializeAndWriteModel(path, oldModel);
                }

                
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void SerializeAndWriteModel<U>(string path, U model) where U : class, ISettingsModel
        {
            var newJson = JsonConvert.SerializeObject(model, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            });

            File.WriteAllText(path, newJson);
        }


        public static void InitializeJsonDoc<U>(string path, U model, params string[] defaultElements) where U : class, ISettingsModel
        {
            if ((path.Split('.')).Last().ToLowerInvariant() != "json")
                throw new ArgumentException("The path given did not end in 'json'");
            
            if (!File.Exists(path))
                File.Create(path).Dispose();

            WriteModelToPath(path, model, true, defaultElements);
        }
    }
}
