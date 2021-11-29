using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using Wolf.Utility.Core.Exceptions;
using Xamarin.Forms;

namespace Wolf.Utility.Xamarin.Extensions.Methods
{
    public static class EntryExtensions
    {
        public static int GetInt(this Entry entry)
        {
            if (string.IsNullOrEmpty(entry.Text)) throw new ArgumentNullException($"{nameof(entry)}.{nameof(entry.Text)}");

            var str = Regex.Replace(entry.Text, "[^0-9]", "");
            var success = int.TryParse(str, out var result);

            if (!success) throw new OperationFailedException($"Failed to parse entry text to a 'Int'");

            return result;
        }

        public static double GetDouble(this Entry entry)
        {
            var str = Regex.Replace(entry.Text, "[^.0-9]", "");
            var success = double.TryParse(str, out var result);

            if (!success) throw new OperationFailedException($"Failed to parse entry text to a 'Double'");

            return result;
        }
    }
}
