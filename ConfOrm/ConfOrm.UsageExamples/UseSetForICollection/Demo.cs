using System;
using System.Linq;
using ConfOrm.NH;
using ConfOrm.Patterns;
using ConfOrm.Shop.CoolNaming;
using ConfOrm.Shop.Patterns;
using NUnit.Framework;

namespace ConfOrm.UsageExamples.UseSetForICollection
{
	public class Demo
	{
		[Test, Explicit]
		public void UseSetForCollection()
		{
			// by default ConfORM will use <bag> for ICollection<T>.
			// If you want to use <set> you have to add a patterm to the ObjectRelationalMapper

			var orm = new ObjectRelationalMapper();

			// With the follow line I'm adding the pattern to discover where map a <set>
			orm.Patterns.Sets.Add(new UseSetWhenGenericCollectionPattern());

			// With the follow line I'm explicitly declaring that I want map the Contact.Aliases property as <bag>
			orm.Bag<Contact>(contact => contact.Aliases);

			var mapper = new Mapper(orm); // <== for this example the CoolPatternsAppliersHolder is not useful
			orm.TablePerClass<Contact>();

			// Show the mapping to the console
			var mapping = mapper.CompileMappingFor(new[] { typeof(Contact) });
			Console.Write(mapping.AsString());
		}

		[Test, Explicit]
		public void ShowRecommendedEntityImplementation()
		{
			// This shows the behavior with the recommended entity implementation
			// As you can see I'm neither Adding nor Removing any pattern; everything is by default.
			// Have a look to the ContactRecommended to know where is the difference.

			var orm = new ObjectRelationalMapper();
			var mapper = new Mapper(orm);
			orm.TablePerClass<ContactRecommended>();

			// Show the mapping to the console
			var mapping = mapper.CompileMappingFor(new[] { typeof(ContactRecommended) });
			Console.Write(mapping.AsString());
		}
	}
}