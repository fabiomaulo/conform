using System;
using System.Linq.Expressions;
using ConfOrm.NH;
using NUnit.Framework;

namespace ConfOrm.UsageExamples.CustomShortcut
{
	public class Demo
	{
		private class MyEntity
		{
			public string Name { get; set; }
		}

		[Test, Explicit]
		public void UsageOfCustomExtensionForNullable()
		{
			// In this example you can see how create some shortcut (or, if you want, your own DSL) extending the Mapper.
			// In the result you can see also how ConfORM map an entity without a property for POID (as supported by NHibernate)

			var orm = new ObjectRelationalMapper();
			var mapper = new Mapper(orm);
			orm.TablePerClass<MyEntity>();

			mapper.NotNullable<MyEntity>(myEntity=> myEntity.Name);
			var mapping = mapper.CompileMappingFor(new[] { typeof(MyEntity) });
			Console.Write(mapping.AsString());
		}
	}

	public static class MyMapperExtensions
	{
		public static void NotNullable<T>(this Mapper mapper, Expression<Func<T, object>> property) where T : class
		{
			mapper.Customize<T>(map => map.Property(property, pm => pm.NotNullable(true)));
		}
	}
}