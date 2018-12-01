using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using WeMicroIt.Utils.XMLConverter.Interfaces;

namespace WeMicroIt.Utils.XMLConverter
{
    /// <include file='./docs/Classes.xml' path='doc/classes[@name="XMLConversion"]/class/*'/> 
    public class XMLConversion : IXMLConversion
    {
        /// <include file='./docs/Classes.xml' path='doc/classes[@name="XMLConversion"]/constructor[@name="Default"]/*'/> 
        public XMLConversion()
        {
        }

        /// <include file='./docs/Classes.xml' path='doc/classes[@name="XMLConversion"]/methods[@name="Transform"][@version="Default"]/*'/> 
        public bool Transforms(string sourcePath, string templatePath, string destinationPath)
        {            
            return Transforms(templatePath, sourcePath, destinationPath, new XmlWriterSettings());
        }

        /// <include file='./docs/Classes.xml' path='doc/classes[@name="XMLConversion"]/methods[@name="Transform"][@version="Settings"]/*'/> 
        public bool Transforms(string sourcePath, string templatePath, string destinationPath, XmlWriterSettings settings)
        {
            transform(templatePath, sourcePath, destinationPath, settings);
            return true;
        }

        /// <include file='./docs/Classes.xml' path='doc/classes[@name="XMLConversion"]/methods[@name="Split"][@version="Pure"]/*'/> 
        public List<XElement> PureSplits(string path, string sourcePath)
        {
            if (string.IsNullOrEmpty(sourcePath))
            {
                throw new FileNotFoundException();
            }
            var doc = XDocument.Load(sourcePath);
            var element = doc.Elements();
            if (!string.IsNullOrEmpty(path) && !path.Contains('/'))
            {
                element = element.Elements(XName.Get(path));
            }
            if (path.Contains('/'))
            {
                foreach (var item in path.Split('/'))
                {
                    element = element.Elements(XName.Get(item));
                }
            }
            var items = element.ToList();
            if (items.Count < 1)
            {
                throw new KeyNotFoundException();
            }
            return items;
        }

        /// <include file='./docs/Classes.xml' path='doc/classes[@name="XMLConversion"]/methods[@name="Split"][@version="Group"]/*'/> 
        public List<XElement> GroupSplits(string path, string sourcePath, string grouping)
        {
            if (grouping.Contains('/'))
            {
                throw new NotSupportedException();
            }
            var items = PureSplits(path, sourcePath);
            List<XElement> groups = new List<XElement>();
            List<string> keys = items.Where(x => x.Element(XName.Get("type")).Value.ToLower() == "t").Select(x => x.Element(XName.Get(grouping)).Value.ToString()).Distinct().ToList();
            foreach (var item in keys)
            {
                XElement ele = new XElement(XName.Get("class"));
                XElement temp = new XElement(XName.Get("name"))
                {
                    Value = item
                };
                ele.Add(temp);
                temp = new XElement(XName.Get("attributes"));
                temp.Add(items.Where(x => x.Element(XName.Get(grouping)).Value.ToLower().StartsWith(item.ToLower())).ToList());
                ele.Add(temp);
                groups.Add(ele);
            }
            return groups;
        }

        /// <include file='./docs/Classes.xml' path='doc/classes[@name="XMLConversion"]/methods[@name="DeserializeDoc"][@version="General"]/*'/> 
        public XDocument DeSerializeDocument(string path)
        {
            return loadDocument(path);
        }

        /// <include file='./docs/Classes.xml' path='doc/classes[@name="XMLConversion"]/methods[@name="DeserializeObj"][@version="Path"]/*'/> 
        public T DeserializeObjects<T>(string path)
        {
            return DeserializeObjects<T>(loadDocument(path));
        }

        /// <include file='./docs/Classes.xml' path='doc/classes[@name="XMLConversion"]/methods[@name="DeserializeObj"][@version="Document"]/*'/> 
        public T DeserializeObjects<T>(XDocument doc)
        {
            return DeserializeObjects<T>(doc);
        }

        /// <include file='./docs/Classes.xml' path='doc/classes[@name="XMLConversion"]/methods[@name="DeserializeObj"][@version="Object"]/*'/> 
        public T DeSerializeObjects<T>(XObject content)
        {
            return convertToObject<T>(content);
        }

        /// <include file='./docs/Classes.xml' path='doc/classes[@name="XMLConversion"]/methods[@name="SerializeObj"]/*'/> 
        public string SerializeObjects(object content)
        {
            throw new NotImplementedException();
        }

        private void transform(string template, string source, string destination)
        {
            transform(template, source, destination, null);
        }

        private void transform(string template, string source, string destination, XmlWriterSettings settings)
        {
            if (string.IsNullOrEmpty(destination))
            {
                throw new DirectoryNotFoundException();
            }
            XslCompiledTransform  xslt = createTransform(template, source);
            if (settings == null || settings == new XmlWriterSettings())
            {
                settings = new XmlWriterSettings()
                {
                    Indent = true,
                    IndentChars = "\t"
                };
            }
            using (XmlWriter writer = XmlWriter.Create(destination, settings))
            {
                xslt.Transform(source, writer);
            }
        }

        private XslCompiledTransform createTransform(string template, string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new FileNotFoundException();
            }
            if (string.IsNullOrEmpty(template))
            {
                throw new FileNotFoundException();
            }
            XslCompiledTransform xslt = new XslCompiledTransform();
            xslt.Load(template);

            if (xslt.Equals(new XslCompiledTransform()))
            {
                throw new Exception();
            }
            return xslt;
        }

        private XDocument loadDocument(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new FileNotFoundException();
            }
            XDocument doc = XDocument.Load(path);
            if (doc == null)
            {
                throw new InvalidDataException();
            }
            return doc;
        }

        private t convertToObject<t>(XObject content)
        {
            if (content == null)
            {
                throw new ArgumentNullException();
            }
            string json = JsonConvert.SerializeXNode(content);
            if (string.IsNullOrEmpty(json))
            {
                throw new ArgumentNullException();
            }
            return JsonConvert.DeserializeObject<t>(json);
        }
    }
}