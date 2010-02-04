using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;

namespace ConfOrmExample
{
	public class CreateXmlMappingsDemo
	{
		[Test, Explicit]
		public void ShowSingleXmlMapping()
		{
			var document = Serialize(NHIntegrationTest.GetMapping());
			Console.Write(document);
		}

		protected static string Serialize(HbmMapping hbmElement)
		{
			var setting = new XmlWriterSettings { Indent = true };
			var serializer = new XmlSerializer(typeof(HbmMapping));
			using (var memStream = new MemoryStream(2048))
			using (var xmlWriter = XmlWriter.Create(memStream, setting))
			{
				serializer.Serialize(xmlWriter, hbmElement);
				memStream.Flush();
				memStream.Position = 0;
				using (var sr = new StreamReader(memStream))
				{
					return sr.ReadToEnd();
				}
			}
		}
	}
}