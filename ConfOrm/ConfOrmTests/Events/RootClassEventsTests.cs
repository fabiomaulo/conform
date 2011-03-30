using ConfOrm;
using ConfOrm.NH;
using NUnit.Framework;
using System.Collections.Generic;
using SharpTestsEx;

namespace ConfOrmTests.Events
{
	public class RootClassEventsTests
	{
		private class MyClass
		{
			
		}

		[Test]
		public void CallBeforeEventBeforeFirstPatternApplier()
		{
			var callSequence = new List<string>();
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<MyClass>();
			
			var patternsAppliersHolder = new EmptyPatternsAppliersHolder();
			patternsAppliersHolder.RootClass.Add(t=> true, (t, cam) => callSequence.Add("pa"));
			
			var mapper = new Mapper(orm, patternsAppliersHolder);
			mapper.BeforeMapClass += (di, t, cam) => callSequence.Add("beforeevent");
			mapper.CompileMappingFor(new []{typeof(MyClass)});

			callSequence.Should().Have.SameSequenceAs("beforeevent", "pa");
		}

		[Test]
		public void CallAfterEventAfterLastCustomizer()
		{
			var callSequence = new List<string>();
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<MyClass>();

			var patternsAppliersHolder = new EmptyPatternsAppliersHolder();
			var mapper = new Mapper(orm, patternsAppliersHolder);
			mapper.AfterMapClass += (di, t, cam) => callSequence.Add("afterevent");

			mapper.Class<MyClass>(ca => callSequence.Add("c1"));
			mapper.Class<MyClass>(ca => callSequence.Add("c2"));
			mapper.CompileMappingFor(new[] { typeof(MyClass) });

			callSequence.Should().Have.SameSequenceAs("c1", "c2", "afterevent");
		}
	}
}