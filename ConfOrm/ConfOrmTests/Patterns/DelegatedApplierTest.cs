using System;
using System.Reflection;
using NHibernate.Mapping.ByCode;
using ConfOrm.Patterns;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns
{
	public class DelegatedApplierTest
	{
		[Test]
		public void WhenCreatedWithNullDelegatesThenThrow()
		{
			Executing.This(() => new DelegatedApplier<MemberInfo, IPropertyMapper>(null, null)).Should().Throw<ArgumentNullException>();
			Executing.This(() => new DelegatedApplier<MemberInfo, IPropertyMapper>(x => true, null)).Should().Throw<ArgumentNullException>();
			Executing.This(() => new DelegatedApplier<MemberInfo, IPropertyMapper>(null, x => { })).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void WhenCallMatchThenExecuteDelegate()
		{
			var executed = false;
			var d = new DelegatedApplier<MemberInfo, IPropertyMapper>(x => { executed = true; return true; }, x => { });
			d.Match(null).Should().Be.True();
			executed.Should().Be.True();
		}

		[Test]
		public void WhenCallAppltThenExecuteDelegate()
		{
			var executed = false;
			var d = new DelegatedApplier<MemberInfo, IPropertyMapper>(x => true, x => executed = true);
			d.Apply(null, null);
			executed.Should().Be.True();
		}
	}
}