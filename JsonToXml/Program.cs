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
    class Program
    {
        static void Main(string[] args)
        {
            using (var stream = new FileStream(args[0], FileMode.Open))
            {
                using (var reader = new StreamReader(stream))
                {
                    var jReader = new JsonTextReader(reader);
                    XmlWriter writer = null;
                    var queue = new Queue<Action<string>>();
                    var unknownActionQueue = new Queue<Action<Action<string>>>();
                    unknownActionQueue.Enqueue(a => a("o"));
                    while (jReader.Read())
                    {
                        if (writer == null)
                        {
                            writer = XmlWriter.Create(new FileStream(@"..\..\test.xml", FileMode.Create));
                            writer.WriteProcessingInstruction("xml", "version='1.0' encoding='utf-8'");
                        }
                        switch (jReader.TokenType)
                        {
                            case JsonToken.StartObject:
                                //writer.WriteStartElement("o");
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
                            case JsonToken.EndObject:
                                writer.WriteEndElement();
                                break;
                            case JsonToken.StartArray:
                                writer.WriteStartElement("a");
                                break;
                            case JsonToken.EndArray:
                                writer.WriteEndElement();
                                break;
                            default:
                                break;
                        }
                    }
                    writer.Close();
                    writer.Dispose();
                }
            }
        }
    }
}
