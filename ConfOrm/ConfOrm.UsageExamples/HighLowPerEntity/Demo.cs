using System;
using ConfOrm.NH;
using ConfOrm.Patterns;
using NUnit.Framework;

namespace ConfOrm.UsageExamples.HighLowPerEntity
{
	public class Demo
	{
		[Test, Explicit]
		public void ComposingPatternsAppliersPacksUsingInflector()
		{
			// In this example you can see how configure the HighLowPoidPattern with a per hierarchy 'where' clause

			var orm = new ObjectRelationalMapper();
			orm.Patterns.PoidStrategies.Add(new HighLowPoidPattern(poidProperty=> new
			{
				max_lo = 100,
				where = string.Format("Entity = '{0}'", poidProperty.ReflectedType.GetRootEntity(orm).Name)
			}));

			// Instancing the Mapper using the result of Merge
			var mapper = new Mapper(orm);

			orm.TablePerClassHierarchy<Product>();
			orm.TablePerClass<Customer>();

			var mapping = mapper.CompileMappingFor(new[] { typeof(Product), typeof(Customer) });
			Console.Write(mapping.AsString());
		}
	}
}