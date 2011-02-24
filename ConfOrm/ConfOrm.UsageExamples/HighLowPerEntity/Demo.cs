using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConfOrm.NH;
using ConfOrm.Patterns;
using Iesi.Collections.Generic;
using NHibernate.Dialect;
using NHibernate.Mapping;
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
			orm.Patterns.PoidStrategies.Add(new HighLowPoidPattern(poidProperty => new
			{
				table = "NextHighVaues",
				column = "NextHigh",
				max_lo = 100,
				where = string.Format("Entity = '{0}'", poidProperty.ReflectedType.GetRootEntity(orm).Name)
			}));

			// Instancing the Mapper using the result of Merge
			var mapper = new Mapper(orm);

			orm.TablePerClassHierarchy<Product>();
			orm.TablePerClass<Customer>();

			var entities = new[] { typeof(Product), typeof(Customer) };
			var mapping = mapper.CompileMappingFor(entities);
			Console.Write(mapping.AsString());

			// with the follow line you can add the script, to populate the HighLow table, directly to the NH's configuration
			// the SchemaExport will do the work.
			//nhConfiguration.AddAuxiliaryDatabaseObject(CreateHighLowScript(orm, entities));
		}

		private IAuxiliaryDatabaseObject CreateHighLowScript(IDomainInspector orm, IEnumerable<Type> entities)
		{
			var script = new StringBuilder(1024);
			script.AppendLine("DELETE FROM NextHighVaues;");
			script.AppendLine("ALTER TABLE NextHighVaues ADD Entity VARCHAR(128) NOT NULL;");
			script.AppendLine("CREATE NONCLUSTERED INDEX IdxNextHighVauesEntity ON NextHighVaues (Entity ASC);");
			script.AppendLine("GO");
			foreach (var entity in entities.Where(x => orm.IsRootEntity(x)))
			{
				script.AppendLine(string.Format("INSERT INTO [NextHighVaues] (Entity, NextHigh) VALUES ('{0}',1);", entity.Name));
			}
			return new SimpleAuxiliaryDatabaseObject(script.ToString(), null, new HashedSet<string> { typeof(MsSql2000Dialect).FullName, typeof(MsSql2005Dialect).FullName, typeof(MsSql2008Dialect).FullName });
		}
	}
}