using System;
using System.Reflection;
using ConfOrm.Patterns;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns
{
	public class DelegatedPatternTests
	{
		[Test]
		public void WhenCreatedWithNullDelegateThenThrow()
		{
			Executing.This(() => new DelegatedPattern<MemberInfo>(null)).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void WhenCallMatchThenExecuteDelegate()
		{
			var executed = false;
			var d = new DelegatedPattern<MemberInfo>(x => { executed = true; return true; });
			d.Match(null).Should().Be.True();
			executed.Should().Be.True();
		}
	}
}