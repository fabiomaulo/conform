using System;
using System.Linq;
using ConfOrm.NH;
using ConfOrm.Shop.CoolNaming;
using NHibernate;
using NUnit.Framework;

namespace ConfOrm.UsageExamples.StringProperty
{
	public class Demo
	{
		[Test, Explicit]
		public void DefiningAndCustomizingVersionThroughBaseImplementation()
		{
			// In this example I'll show how you can work with Version and how ConfORM understands OOP

			var orm = new ObjectRelationalMapper();
			var mapper = new Mapper(orm, new CoolPatternsAppliersHolder(orm));

			// In this case I will use the definition of table-to-class strategy class by class
			orm.TablePerClass<Customer>();

            mapper.Customize<Customer>(c => c.Property(p => p.Name, pm =>
                pm.Length(50)));

            mapper.Customize<Customer>(c => c.Property(p => p.Notes, pm =>
                pm.Type(NHibernateUtil.StringClob)));
			
			// Note : I have to create mappings for the whole domain; Entity and VersionedEntity are excluded from de mapping because out-side
			// root-entities hierarchy (root-entities are : CurrencyDefinition, Company, Customer, Provider)
			var mapping = mapper.CompileMappingFor(typeof(Entity).Assembly.GetTypes().Where(t => t.Namespace == typeof(Entity).Namespace));
			Console.Write(mapping.AsString());
		}
	}
}