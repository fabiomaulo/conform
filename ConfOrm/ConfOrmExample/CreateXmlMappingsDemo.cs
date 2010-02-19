using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using ConfOrm.NH;
using ConfOrmExample.Domain;
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
			File.WriteAllText("MyMapping.hbm.xml", document);
			Console.Write(document);
		}

		[Test, Explicit]
		public void WriteAllXmlMapping()
		{
			var orm = NHIntegrationTest.GetMappedDomain();
			var mapper = new Mapper(orm);
			var mappings = mapper.CompileMappingForEach(Assembly.GetExecutingAssembly().GetTypes().Where(t => t.Namespace == typeof(Animal).Namespace));
			
			foreach (var hbmMapping in mappings)
			{
				var fileName = GetFileName(hbmMapping);
				var document = Serialize(hbmMapping);
				File.WriteAllText(fileName, document);
			}
		}

		private string GetFileName(HbmMapping hbmMapping)
		{
			var name = "MyMapping";
			var rc = hbmMapping.RootClasses.FirstOrDefault();
			if(rc!=null)
			{
				name= rc.Name;
			}
			var sc = hbmMapping.SubClasses.FirstOrDefault();
			if (sc != null)
			{
				name = sc.Name;
			}
			var jc = hbmMapping.JoinedSubclasses.FirstOrDefault();
			if (jc != null)
			{
				name = jc.Name;
			}
			var uc = hbmMapping.UnionSubclasses.FirstOrDefault();
			if (uc != null)
			{
				name = uc.Name;
			}
			return name + ".hbm.xml";
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