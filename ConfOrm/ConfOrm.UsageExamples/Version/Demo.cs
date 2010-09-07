using System;
using System.Linq;
using ConfOrm.NH;
using ConfOrm.Shop.CoolNaming;
using NUnit.Framework;

namespace ConfOrm.UsageExamples.Version
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
			orm.TablePerClass<CurrencyDefinition>();
			orm.TablePerClass<Company>();
			orm.TablePerClass<Customer>();
			orm.TablePerClass<Provider>();

			// Defining relations
			orm.OneToOne<Company, Customer>();
			orm.OneToOne<Company, Provider>();

			// In the follow line I'm defining which is the property used as Version for all classes inherited from VersionedEntity
			orm.VersionProperty<VersionedEntity>(ve=> ve.Version);

			// In the follow line I'm customizing the column-name for the property used as Version for all classes inherited from VersionedEntity....
			// Note : VersionedEntity is not an entity, it is only a base class.
			mapper.Class<VersionedEntity>(cm => cm.Version(ve => ve.Version, vm => vm.Column("Revision")));

			// In the follow line I'm customizing the column-name for the property used as Version only for the class Provider
			// Note : You can move the follow line before the previous and the result does not change, this is because ConfORM can understand
			// which is a base customization and which is the specific customization.
			mapper.Class<Provider>(cm => cm.Version(ve => ve.Version, vm => vm.Column("IncrementalVersion")));

			// Note : I have to create mappings for the whole domain; Entity and VersionedEntity are excluded from de mapping because out-side
			// root-entities hierarchy (root-entities are : CurrencyDefinition, Company, Customer, Provider)
			var mapping = mapper.CompileMappingFor(typeof(Entity).Assembly.GetTypes().Where(t => t.Namespace == typeof(Entity).Namespace));
			Console.Write(mapping.AsString());
		}
	}
}