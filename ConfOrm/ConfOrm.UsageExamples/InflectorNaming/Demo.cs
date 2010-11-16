using System;
using ConfOrm.NH;
using ConfOrm.Shop.CoolNaming;
using ConfOrm.Shop.InflectorNaming;
using ConfOrm.Shop.Inflectors;
using ConfOrm.Shop.Packs;
using NUnit.Framework;

namespace ConfOrm.UsageExamples.InflectorNaming
{
	public class UsuarioWeb
	{
		public int Id { get; set; }
	}

	public class Demo
	{
		[Test, Explicit]
		public void ComposingPatternsAppliersPacksUsingInflector()
		{
			// In this example you can see how use the inflector adding some special class name translation.

			var orm = new ObjectRelationalMapper();

			// Creation of inflector adding some special class name translation
			var inflector = new SpanishInflector();
			AddCustomData_Dictionary(inflector);

			IPatternsAppliersHolder patternsAppliers =
				(new SafePropertyAccessorPack())
					.Merge(new OneToOneRelationPack(orm))
					.Merge(new BidirectionalManyToManyRelationPack(orm))
					.Merge(new BidirectionalOneToManyRelationPack(orm))
					.Merge(new DiscriminatorValueAsClassNamePack(orm))
					.Merge(new CoolColumnsNamingPack(orm))
					.Merge(new TablePerClassPack())
					.Merge(new PluralizedTablesPack(orm, inflector)) // <=== 
					.Merge(new ListIndexAsPropertyPosColumnNameApplier());

			// Instancing the Mapper using the result of Merge
			var mapper = new Mapper(orm, patternsAppliers);

			orm.TablePerClass<UsuarioWeb>();

			var mapping = mapper.CompileMappingFor(new[] { typeof(UsuarioWeb) });
			Console.Write(mapping.AsString());
		}

		private void AddCustomData_Dictionary(SpanishInflector inflector)
		{
			// comment the follow line to see the different table name
			inflector.AddIrregular("UsuarioWeb", "UsuariosWeb");
		}
	}
}