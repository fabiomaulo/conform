using System;
using System.Linq;
using ConfOrm.NH;
using ConfOrm.Patterns;
using ConfOrm.Shop.CoolNaming;
using NUnit.Framework;

namespace ConfOrm.UsageExamples.UseBagForIList
{
	public class Demo
	{
		[Test, Explicit]
		public void UseBagForLists()
		{
			// NH does not support <list> for bidirectional-one-to-many (aka parent child) and ConfORM know this fact so, in general, you don't have to do anything.
			// The IList<T> should be used only where the order is important in your domain.
			// Where the order is not important you can expose ICollection<T> (in a property or in a field).
			// If you want use IList<T> everywhere but you want map a IList<T> using a <bag> you can remove a pettern,
			// then you can always use the explicit declaration for those collection where you need a <list> mapping.

			var orm = new ObjectRelationalMapper();

			// With the follow line I'm removing the pattern to discover where map a <list>; doing so all IList<T> will be mapped using <bag>
			orm.Patterns.Lists.Remove(orm.Patterns.Lists.Single(p => p.GetType() == typeof(ListCollectionPattern)));

			// With the follow line I'm explicitly dwclaring that I want map the Contact.Phones property as <list>
			orm.List<Contact>(contact=> contact.Phones);

			var mapper = new Mapper(orm); // <== for this example the CoolPatternsAppliersHolder is not useful
			orm.TablePerClass<Contact>();

			// Show the mapping to the console
			var mapping = mapper.CompileMappingFor(new[] {typeof (Contact)});
			Console.Write(mapping.AsString());
		}

		[Test, Explicit]
		public void UseBagForListsWithCoolNaming()
		{
			// this is the same example as the previous but this time using the CoolPatternsAppliersHolder and you can see
			// which is the difference

			var orm = new ObjectRelationalMapper();
			orm.Patterns.Lists.Remove(orm.Patterns.Lists.Single(p => p.GetType() == typeof(ListCollectionPattern)));
			orm.List<Contact>(contact => contact.Phones);

			var mapper = new Mapper(orm, new CoolPatternsAppliersHolder(orm));
			orm.TablePerClass<Contact>();

			// Show the mapping to the console
			var mapping = mapper.CompileMappingFor(new[] { typeof(Contact) });
			Console.Write(mapping.AsString());
		}
	}
}