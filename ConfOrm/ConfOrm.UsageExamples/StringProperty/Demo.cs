using System;
using ConfOrm.NH;
using ConfOrm.Shop.CoolNaming;
using NHibernate;
using NUnit.Framework;

namespace ConfOrm.UsageExamples.StringProperty
{
	public class Demo
	{
		[Test, Explicit]
		public void CustomizationOfStrings()
		{
			// In this example you can see how customize multiple properties in the same sentence.
			// Specially have a look to how customize elements of a collection (in this cas a Bag).

			var orm = new ObjectRelationalMapper();
			var mapper = new Mapper(orm, new CoolPatternsAppliersHolder(orm));
			orm.TablePerClass<Customer>();

			mapper.Class<Customer>(c =>
			                           {
																	 c.Property(p => p.Name, pm => pm.Length(50));
																	 c.Property(p => p.Note, pm => pm.Type(NHibernateUtil.StringClob));
																	 c.Bag(p => p.Notes, x => { }, map => map.Element(e=> e.Type(NHibernateUtil.StringClob)));
			                           });


			var mapping = mapper.CompileMappingFor(new[] { typeof(Customer) });
			Console.Write(mapping.AsString());
		}
	}
}