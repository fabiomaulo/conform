using ConfOrm;
using ConfOrm.NH;
using NUnit.Framework;
using System.Collections.Generic;
using SharpTestsEx;

namespace ConfOrmTests.Events
{
	public class SubclassEventsTests
	{
		private class MyClass
		{
			public int Id { get; set; }
		}
		private class Inherited : MyClass
		{

		}

		[Test]
		public void CallBeforeEventBeforeFirstPatternApplier()
		{
			var callSequence = new List<string>();
			var orm = new ObjectRelationalMapper();
			orm.TablePerClassHierarchy<MyClass>();
			
			var patternsAppliersHolder = new EmptyPatternsAppliersHolder();
			patternsAppliersHolder.Subclass.Add(t=> true, (t, cam) => callSequence.Add("pa"));
			
			var mapper = new Mapper(orm, patternsAppliersHolder);
			mapper.BeforeMapSubclass += (di, t, cam) => callSequence.Add("beforeevent");
			mapper.CompileMappingFor(new[] { typeof(MyClass), typeof(Inherited) });

			callSequence.Should().Have.SameSequenceAs("beforeevent", "pa");
		}

		[Test]
		public void CallAfterEventAfterLastCustomizer()
		{
			var callSequence = new List<string>();
			var orm = new ObjectRelationalMapper();
			orm.TablePerClassHierarchy<MyClass>();

			var patternsAppliersHolder = new EmptyPatternsAppliersHolder();
			var mapper = new Mapper(orm, patternsAppliersHolder);
			mapper.AfterMapSubclass += (di, t, cam) => callSequence.Add("afterevent");

			mapper.Subclass<Inherited>(ca => callSequence.Add("c1"));
			mapper.Subclass<Inherited>(ca => callSequence.Add("c2"));
			mapper.CompileMappingFor(new[] { typeof(MyClass), typeof(Inherited) });

			callSequence.Should().Have.SameSequenceAs("c1", "c2", "afterevent");
		}
	}
}