using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Wolf.Utility.Main.Xml
{
    public class XmlManipulator
    {
        public static string ReadXmlValue(string path, string propertyName)
        {
            if (!File.Exists(path))
                throw new ArgumentNullException($"{nameof(path)}", $@"No file Exist on the specified path => {path}");

            var doc = new XmlDocument();
            doc.Load(path);

            if (doc.Attributes == null)
                throw new ArgumentNullException($"{nameof(doc.Attributes)}", $"Xml document contained no attributes");
            
            var att = doc.Attributes[propertyName];
            return att.Value;
        }

        public static void WriteXmlValue(string path, string propertyName, string value)
        {
            if (!File.Exists(path))
                throw new ArgumentNullException($"{nameof(path)}", $@"No file Exist on the specified path => {path}");

            var doc = new XmlDocument();
            doc.Load(path);

            if (doc.Attributes == null)
                throw new ArgumentNullException($"{nameof(doc.Attributes)}", $"Xml document contained no attributes");

            var att = doc.Attributes[propertyName];
            att.Value = value;

            doc.Save(path);
        }

        /// <summary>
        /// Method will not override existing file!
        /// </summary>
        /// <param name="path"></param>
        /// <param name="elements"></param>
        public static void InitializeXmlDoc(string path, params (string propertyName, string value)[] elements)
        {
            if (path.Split('.').Last().ToLowerInvariant() != "xml") throw new ArgumentException("The path given did not end in 'xml'");

            if (!File.Exists(path))
            {
                var doc = new XmlDocument();
                var declaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                var root = doc.DocumentElement;
                doc.InsertBefore(declaration, root);

                var groups = elements.GroupBy(x => x.propertyName).AsEnumerable();

                foreach (var group in groups)
                {
                    if (group.Count() == 1)
                    {
                        foreach (var (propertyName, value) in group)
                        {
                            var element = doc.CreateElement(string.Empty, propertyName, string.Empty);
                            element.Value = value;

                            doc.AppendChild(element);
                        }
                    }
                    else
                    {
                        var element = doc.CreateElement(string.Empty, group.Key, string.Empty);
                        foreach (var (propertyName, value) in group)
                        {
                            var innerElement = doc.CreateElement(string.Empty, propertyName, string.Empty);
                            innerElement.Value = value;

                            element.AppendChild(innerElement);
                        }
                        doc.AppendChild(element);
                    }
                }
                
                doc.Save(path);
            }
        }
    }
}
