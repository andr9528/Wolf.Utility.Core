using System;
using System.Collections.Generic;
using System.Text;

namespace Wolf.Utility.Core.Arduino
{
    public abstract class HeaderBuilder
    {
        protected const string FOOTER = "#endif";

        public List<string> Libraries { get; }
        public List<string> Files { get; }
        public string FileName { get; }

        protected string HEADER() 
        {
            var builder = new StringBuilder();

            builder.AppendLine($"#ifndef _{FileName}_H");
            builder.AppendLine($"define _{FileName}_H");
            
            if(Libraries.Count != 0) builder.AppendLine();

            foreach (var lib in Libraries)
            {
                builder.AppendLine($"#include <{lib}>");
            }

            if (Files.Count != 0) builder.AppendLine();

            foreach (var file in Files)
            {
                builder.AppendLine($"#include \"{file}\"");

            }

            builder.AppendLine();

            return builder.ToString();
        }

        protected HeaderBuilder(string fileName, IEnumerable<string> libraries = null, IEnumerable<string> files = null)
        {
            Libraries = (List<string>)libraries ?? new List<string>();
            Files = (List<string>)files ?? new List<string>();
            FileName = fileName;
        }

        /// <summary>
        /// Use a Stringbuilder to build the header filer. 
        /// Start with a call to 'BuildCheck'. It ensures the savePath has the correct fileformat.
        /// When doing the Stringbuilder, Start with adding the result from HEADER, and end with the result from FOOTER.
        /// When done building the file, save it to the location specified in savePath. 
        /// </summary>
        /// <param name="savePath"></param>
        public abstract void Build(string savePath);

        protected void BuildCheck(string savePath) 
        {
            var split = savePath.Split('.');
            var last = split[split.Length - 1];

            if (last != "h") throw new ArgumentException($"savePath has to end with .h; inputtet savePath: {savePath}");
        }

        
    }
}
