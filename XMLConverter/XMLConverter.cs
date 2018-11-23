using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using WeMicroIt.Utils.XMLConverter.Interfaces;

namespace WeMicroIt.Utils.XMLConverter
{
    public class XMLConverter : IXMLConverter
    {
        private JsonSerializerSettings JSONSettings { get; set; }
        private Formatting JSONFormatter { get; set; }

        public XMLConverter()
        {
            JSONFormatter = Formatting.Indented;
        }

        public JsonSerializerSettings SetSettings(JsonSerializerSettings settings)
        {
            try
            {
                if (settings == null)
                {
                    settings = new JsonSerializerSettings();
                }
                JSONSettings = settings;
                return JSONSettings;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Formatting SetSettingsFormatting(Formatting format)
        {
            try
            {
                JSONFormatter = format;
                return format;
            }
            catch (Exception)
            {
                return format;
            }
        }

        public T DeSerializeObjects<T>(string content)
        {
            throw new NotImplementedException();
        }

        public string SerializeObjects(object content)
        {
            throw new NotImplementedException();
        }
    }
}
