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

        public XMLConversion()
        {
        }

        public T DeserializeObjects<T>(string path, string template)
        {
            try
            {
                // Load the style sheet.
                XslCompiledTransform xslt = new XslCompiledTransform();
                xslt.Load(template);

                Stream res = null;
                // Execute the transform and output the results to a file.
                xslt.Transform(path, null, res);
                return JsonConvert.DeserializeObject<T>(res.ToString());
            }
            catch (Exception)
            {

                throw;
            }
        }

        public T DeSerializeObjects<T>(XObject content)
        {
            try
            {
                var set = new JsonSerializerSettings()
                {
                    MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
                };
                return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeXNode(content), set);
            }
            catch (Exception exc)
            {
                return default(T);
            }
        }

        public string SerializeObjects(object content)
        {
            throw new NotImplementedException();
        }
    }
}
