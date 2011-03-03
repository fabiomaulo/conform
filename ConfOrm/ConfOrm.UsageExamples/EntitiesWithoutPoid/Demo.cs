using System;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Patterns;
using NHibernate;
using NHibernate.Type;
using NUnit.Framework;

namespace ConfOrm.UsageExamples.EntitiesWithoutPoid
{
	public class Person
	{
		public string Name { get; set; }
		public string Surname { get; set; }
	}

	public class Address
	{
		public string Street { get; set; }
	}

	public class Product
	{
		public string Description { get; set; }
	}

	public class Demo
	{
		private class MyHighLowDef: IGeneratorDef
		{
			public string Class
			{
				get { return "hilo"; }
			}

			public object Params{get;set;}
		}

		[Test, Explicit]
		public void HighLowCustomization()
		{
			// In this example you can see how configure the HighLowPoidPattern for entities without neither a property nor a field representing the POID
			var entities = new[] { typeof(Product), typeof(Person), typeof(Address) };

			var orm = new ObjectRelationalMapper();

			// Instancing the Mapper using the result of Merge
			var mapper = new Mapper(orm);
			mapper.AddPoidPattern(mi => mi == null, (x,applyTo) =>
			                                        {
																								applyTo.Generator(new MyHighLowDef { Params = new { max_lo = 100 } });
																								applyTo.Type((IIdentifierType)NHibernateUtil.Int32);
			                                        });
			orm.TablePerClass(entities);

			var mapping = mapper.CompileMappingFor(entities);
			Console.Write(mapping.AsString());
		}

	}
}