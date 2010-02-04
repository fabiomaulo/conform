using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using ConfOrm;
using ConfOrm.NH;
using ConfOrm.Patterns;
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
			var document = Serialize(GetMapping());
			Console.Write(document);
		}

		public static ObjectRelationalMapper GetMappedDomain()
		{
			var orm = new ObjectRelationalMapper();

			orm.TablePerClass<Animal>();
			orm.TablePerClass<User>();
			orm.TablePerClass<StateProvince>();
			orm.TablePerClassHierarchy<Zoo>();

			orm.ManyToMany<Human, Human>();
			orm.OneToOne<User, Human>();

			orm.PoidStrategies.Add(new NativePoidPattern());

			return orm;
		}

		public static HbmMapping GetMapping()
		{
			var orm = GetMappedDomain();
			var mapper = new Mapper(orm);
			return mapper.CompileMappingFor(typeof(Animal).Assembly.GetTypes().Where(t => t.Namespace == typeof(Animal).Namespace));
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