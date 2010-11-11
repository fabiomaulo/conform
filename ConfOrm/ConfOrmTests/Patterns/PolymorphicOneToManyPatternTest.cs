using System;
using ConfOrm.Patterns;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns
{
	public class PolymorphicOneToManyPatternTest
	{
		// The PolymorphicOneToManyPattern was reimplemented from hardcoded impl inside ObjectRelationalMapper
		// So far there are some integration tests testing its behavior.

		[Test]
		public void CtorProtection()
		{
			Executing.This(() => new PolymorphicOneToManyPattern(null)).Should().Throw<ArgumentNullException>();
		}
	}
}