using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonToXml.Tests
{
    [TestFixture]
    public class when_parsing_dates
    {
        [Test]
        public void then_ISO_8601_date_only_is_converted()
        {
            string json = "{\"date\":\"2014-01-01\"}";
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {

            }
        }
    }
}
