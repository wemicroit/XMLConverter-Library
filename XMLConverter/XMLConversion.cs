using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Xml.Xsl;
using WeMicroIt.Utils.XMLConverter.Interfaces;

namespace WeMicroIt.Utils.XMLConverter
{
    public class XMLConversion : IXMLConversion
    {
        private string xlstTemplatePath { get; set; }

        public XMLConversion()
        {
        }
        
        public string SetXLSPath(string path)
        {
            try
            {
                xlstTemplatePath = path;
                return xlstTemplatePath;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool TransformObjects(string sourcePath, string templatePath, string destinationPath)
        {
            if (string.IsNullOrEmpty(sourcePath))
            {
                throw new FileNotFoundException();
            }
            if (string.IsNullOrEmpty(templatePath))
            {
                throw new FileNotFoundException();
            }
            if (string.IsNullOrEmpty(destinationPath))
            {
                throw new DirectoryNotFoundException();
            }

            XslCompiledTransform xslt = new XslCompiledTransform();
            xslt.Load(templatePath);

            if (xslt.Equals(new XslCompiledTransform()))
            {
                throw new Exception();
            }

            xslt.Transform(sourcePath, destinationPath);
            return true;
        }

        public T DeSerializeObjects<T>(string path)
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
            return DeSerializeObjects<T>(doc.FirstNode);
        }

        public T DeSerializeObjects<T>(XObject content)
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
            return JsonConvert.DeserializeObject<T>(json);
        }

        public string SerializeObjects(object content)
        {
            throw new NotImplementedException();
        }
    }
}
