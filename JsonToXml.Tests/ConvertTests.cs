namespace JsonToXml.Tests
{
    using System;
    using System.IO;
    using System.Text;
    using NUnit.Framework;
    using Convert = JsonToXml.Convert;

    public class ConvertTests
    {
        [Test]
        public void TransformJson()
        {
            var jsonStream = new MemoryStream();
            var json = Properties.Resources.Json;
            Console.Write(json);
            using (var writer = new StreamWriter(jsonStream, Encoding.UTF8, 1024, true))
            {
                writer.Write(json);
            }
            jsonStream.Position = 0;
            var xmlStream = new FakeStream(); // new MemoryStream(); 
            Convert.ToXml(jsonStream, xmlStream);
            xmlStream.Position = 0;
            using (var reader = new StreamReader(xmlStream))
            {
                var xml = reader.ReadToEnd();
                Console.Write(xml);
                Assert.AreEqual(Properties.Resources.Xml, xml);
            }
        }
    }
}
