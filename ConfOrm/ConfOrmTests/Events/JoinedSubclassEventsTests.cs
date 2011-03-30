using ConfOrm;
using ConfOrm.NH;
using NUnit.Framework;
using System.Collections.Generic;
using SharpTestsEx;

namespace ConfOrmTests.Events
{
	public class JoinedSubclassEventsTests
	{
		private class MyClass
		{
		}
		private class Inherited : MyClass
		{

		}

		[Test]
		public void CallBeforeEventBeforeFirstPatternApplier()
		{
			var callSequence = new List<string>();
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<MyClass>();
			
			var patternsAppliersHolder = new EmptyPatternsAppliersHolder();
			patternsAppliersHolder.JoinedSubclass.Add(t=> true, (t, cam) => callSequence.Add("pa"));
			
			var mapper = new Mapper(orm, patternsAppliersHolder);
			mapper.BeforeMapJoinedSubclass += (di, t, cam) => callSequence.Add("beforeevent");
			mapper.CompileMappingFor(new[] { typeof(MyClass), typeof(Inherited) });

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
			mapper.AfterMapJoinedSubclass += (di, t, cam) => callSequence.Add("afterevent");

			mapper.JoinedSubclass<Inherited>(ca => callSequence.Add("c1"));
			mapper.JoinedSubclass<Inherited>(ca => callSequence.Add("c2"));
			mapper.CompileMappingFor(new[] { typeof(MyClass), typeof(Inherited) });

			callSequence.Should().Have.SameSequenceAs("c1", "c2", "afterevent");
		}
	}
}