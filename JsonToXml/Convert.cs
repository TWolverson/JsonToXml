using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace JsonToXml
{
    public static class Convert
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonStream">The stream to read the json from</param>
        /// <param name="xmlStream">The stream to write the xml to</param>
        public static void ToXml(Stream jsonStream, Stream xmlStream)
        {
            if (!xmlStream.CanWrite)
            {
                throw new ArgumentException("Cannot write to output stream");
            }
            using (XmlWriter writer = XmlWriter.Create(xmlStream))
            using (var reader = new StreamReader(jsonStream))
            {
                var jReader = new JsonTextReader(reader);
                writer.WriteProcessingInstruction("xml", "version='1.0' encoding='utf-8'"); ;
                var unknownActionQueue = new Queue<Action<Action<string>>>();
                unknownActionQueue.Enqueue(a => a("o"));
                while (jReader.Read())
                {
                    switch (jReader.TokenType)
                    {
                        case JsonToken.StartObject:
                            unknownActionQueue.Dequeue()((s) => writer.WriteStartElement(s));
                            break;
                        case JsonToken.PropertyName:
                            var propertyName = (string)jReader.Value;
                            Action<Action<string>> DoSomethingLater = (a) => a(propertyName);
                            unknownActionQueue.Enqueue(DoSomethingLater);
                            break;
                        case JsonToken.String:
                            var propertyValue = (string)jReader.Value;
                            unknownActionQueue.Dequeue()((s) => writer.WriteAttributeString(s, propertyValue));
                            break;
                        case JsonToken.Boolean:
                            unknownActionQueue.Dequeue()((s) => writer.WriteAttributeString(XmlConvert.EncodeName(s), XmlConvert.ToString((bool)jReader.Value)));
                            break;
                        case JsonToken.Float:
                            unknownActionQueue.Dequeue()((s) => writer.WriteAttributeString(XmlConvert.EncodeName(s), XmlConvert.ToString((double)jReader.Value)));
                            break;
                        case JsonToken.Date:
                            unknownActionQueue.Dequeue()((s) => writer.WriteAttributeString(XmlConvert.EncodeName(s), XmlConvert.ToString((DateTime)jReader.Value)));
                            break;
                        case JsonToken.Integer:
                            unknownActionQueue.Dequeue()((s) => writer.WriteAttributeString(XmlConvert.EncodeName(s), XmlConvert.ToString((int)jReader.Value)));
                            break;
                        case JsonToken.EndObject:
                            writer.WriteEndElement();
                            break;
                        case JsonToken.StartArray:
                            unknownActionQueue.Dequeue()((s) => writer.WriteStartElement(s));
                            unknownActionQueue.Enqueue(a => a("o"));
                            break;
                        case JsonToken.EndArray:
                            writer.WriteEndElement();
                            break;
                        default:
                            break;
                    }

                }
            }
        }
    }
}
