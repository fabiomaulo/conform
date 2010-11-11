using System;
using ConfOrm.Patterns;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns
{
	public class HeterogeneousAssociationOnPolymorphicPatternTest
	{
		// The HeterogeneousAssociationOnPolymorphicPattern was reimplemented from hardcoded impl inside ObjectRelationalMapper
		// So far there are some integration tests testing its behavior.

		[Test]
		public void CtorProtection()
		{
			Executing.This(() => new HeterogeneousAssociationOnPolymorphicPattern(null)).Should().Throw<ArgumentNullException>();
		}
	}
}