using System;
using System.Linq;
using ConfOrm.NH;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace ConfOrm.UsageExamples.CustomMulticolumnsType
{
	public class Demo
	{
		private static readonly string[] DaysOfTheWeekColumnsNames = new string[7] { "Su", "Mo", "Tu", "We", "Th", "Fr", "Sa" };

		[Test, Explicit]
		public void UsageOfCustomMulticolumnsTypeForSpecificProperty()
		{
			// In this example you can see how create setup ConfORM to map a multi-column usertype with parameters.

			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<LoveEvent>();

			// In this case the custom type is used only for a specific property.
			orm.Complex<LoveEvent>(loveEvent => loveEvent.AllowedWeekDays);

			var mapper = new Mapper(orm);

			// In this case I'm using the customizer for the specific property
			mapper.Customize<LoveEvent>(x => x.Property(le => le.AllowedWeekDays, pm =>
			{
				pm.Type(typeof(MulticolumnsBoolArrayType), new { ArraySize = DaysOfTheWeekColumnsNames.Length });
				pm.Columns(DaysOfTheWeekColumnsNames.Select(colName => new Action<IColumnMapper>(cm => cm.Name(colName))).ToArray());
			}));

			var mapping = mapper.CompileMappingFor(new[] { typeof(LoveEvent) });
			Console.Write(mapping.AsString());
		}

		[Test, Explicit]
		public void UsageOfCustomMulticolumnsTypeUsingPatternAppliers()
		{
			// In this example you can see how create setup ConfORM to map a multi-column usertype with parameters.

			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<LoveEvent>();

			// In this case the custom type is used only for a specific property.
			orm.Complex<LoveEvent>(loveEvent => loveEvent.AllowedWeekDays);

			var mapper = new Mapper(orm);

			// In this case I'm using a PatternApplier (directly with delegates instead a specific class) to define the persistent representation of the bool array
			mapper.AddPropertyPattern(mi => typeof(bool[]).Equals(mi.GetPropertyOrFieldType()) && mi.Name.Contains("Days"), (mi, pm) =>
			{
				pm.Type(typeof(MulticolumnsBoolArrayType), new { ArraySize = DaysOfTheWeekColumnsNames.Length });
				pm.Columns(DaysOfTheWeekColumnsNames.Select(colName => new Action<IColumnMapper>(cm => cm.Name(mi.Name + colName))).ToArray());
			});

			var mapping = mapper.CompileMappingFor(new[] { typeof(LoveEvent) });
			Console.Write(mapping.AsString());
		}
	}
}