using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Wolf.Utility.Main.Extensions.Methods
{
    public static class DataTypeExtensions
    {
        public static bool IsValidJson<T>(this string stringToCheck, out T result) where T : class
        {
            try
            {
                var data = JsonConvert.DeserializeObject<T>(stringToCheck);
                if (data != null)
                {
                    result = data;
                    return true;
                }
            }
            catch (Exception)
            {
                result = default;
                return false;
            }

            result = default;
            return false;
        }
    }
}
