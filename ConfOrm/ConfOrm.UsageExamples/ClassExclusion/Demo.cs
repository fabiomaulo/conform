using System;
using System.Linq;
using ConfOrm.NH;
using ConfOrm.Shop.CoolNaming;
using NUnit.Framework;

namespace ConfOrm.UsageExamples.ClassExclusion
{
	public class Demo
	{
		/*
		 * In this domain entities are:
		 * Movement, Income, Outcome, MovementDetail, IncomeDetail, OutcomeDetail.
		 * Taking advantage from OOP I have implemented some logic in two generic classes:  Movement<TDetail> and MovementDetail<TMovement>.
		 * In these two classes I implemented the logic for parent-child relationship. I have no interest to map it as entities but its properties
		 * (the collection in Movement<TDetail> and the reference in MovementDetail<TMovement>) should be mapped in its concrete implementations
		 * (Income, Outcome for Movement<TDetail> and IncomeDetail, OutcomeDetail for MovementDetail<TMovement>).
		 */
		[Test, Explicit]
		public void ExclusionDemo()
		{
			var orm = new ObjectRelationalMapper();

			// In this case I want exclude any implementation of Movement<TDetail> and MovementDetail<TMovement>.
			// Where the type if not generic or you have to jump a specific concrete-type you can use the method Exclude<TClass> (see ConfOrmTests.ObjectRelationalMapperTests.HierarchyClassExclusionTest)
			orm.Exclude(typeof(Movement<>));
			orm.Exclude(typeof(MovementDetail<>));

			var mapper = new Mapper(orm, new CoolPatternsAppliersHolder(orm)); // <== CoolPatternsAppliersHolder is not important... only nice to have

			// The strategy to represent classes is not important
			orm.TablePerClassHierarchy<Movement>();
			orm.TablePerClassHierarchy<MovementDetail>();

			// Show the mapping to the console
			var mapping = mapper.CompileMappingFor(typeof(Movement).Assembly.GetTypes().Where(t => t.Namespace == typeof(Movement).Namespace));
			Console.Write(mapping.AsString());
		}
	}
}