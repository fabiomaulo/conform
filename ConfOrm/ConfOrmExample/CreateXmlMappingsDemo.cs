using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using System.Collections.Generic;

namespace ConfOrmExample
{
	public class CreateXmlMappingsDemo
	{
		[Test, Explicit]
		public void ShowSingleXmlMapping()
		{
			var domainMapper = new DefaultDomainMapper();
			var entities = new List<Type>();
			entities.AddRange(ModuleMappingUtil.RunModuleMapping<NaturalnessModuleMapping>(domainMapper.DomainDefinition, domainMapper.Mapper));

			var document = Serialize(domainMapper.Mapper.CompileMappingFor(entities));
			File.WriteAllText("MyMapping.hbm.xml", document);
			Console.Write(document);
		}

		[Test, Explicit]
		public void ShowSingleXmlMappingWithCoolAppliers()
		{
			var domainMapper = new CoolDomainMapper();
			var entities = new List<Type>();
			entities.AddRange(ModuleMappingUtil.RunModuleMapping<NaturalnessModuleMapping>(domainMapper.DomainDefinition, domainMapper.Mapper));

			var document = Serialize(domainMapper.Mapper.CompileMappingFor(entities));
			File.WriteAllText("MyMapping.hbm.xml", document);
			Console.Write(document);
		}

		[Test, Explicit]
		public void ShowSingleXmlMappingWithDearDbaAppliers()
		{
			var domainMapper = new DearDbaDomainMapper();
			var entities = new List<Type>();
			entities.AddRange(ModuleMappingUtil.RunModuleMapping<NaturalnessModuleMapping>(domainMapper.DomainDefinition, domainMapper.Mapper));

			var document = Serialize(domainMapper.Mapper.CompileMappingFor(entities));
			File.WriteAllText("MyMapping.hbm.xml", document);
			Console.Write(document);
		}

		[Test, Explicit]
		public void WriteAllXmlMapping()
		{
			var domainMapper = new DefaultDomainMapper();
			var entities = new List<Type>();
			entities.AddRange(ModuleMappingUtil.RunModuleMapping<NaturalnessModuleMapping>(domainMapper.DomainDefinition, domainMapper.Mapper));

			var mappings = domainMapper.Mapper.CompileMappingForEach(entities);
			
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