using System;
using System.Reflection;
using NHibernate.Mapping.ByCode;
using ConfOrm.Patterns;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns
{
	public class DelegatedAdvancedApplierTest
	{
		[Test]
		public void WhenCreatedWithNullDelegatesThenThrow()
		{
			Executing.This(() => new DelegatedAdvancedApplier<MemberInfo, IPropertyMapper>(null, null)).Should().Throw<ArgumentNullException>();
			Executing.This(() => new DelegatedAdvancedApplier<MemberInfo, IPropertyMapper>(x => true, null)).Should().Throw<ArgumentNullException>();
			Executing.This(() => new DelegatedAdvancedApplier<MemberInfo, IPropertyMapper>(null, (x, y) => { })).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void WhenCallMatchThenExecuteDelegate()
		{
			var executed = false;
			var d = new DelegatedAdvancedApplier<MemberInfo, IPropertyMapper>(x => { executed = true; return true; }, (x, y) => { });
			d.Match(null).Should().Be.True();
			executed.Should().Be.True();
		}

		[Test]
		public void WhenCallAppltThenExecuteDelegate()
		{
			var executed = false;
			var d = new DelegatedAdvancedApplier<MemberInfo, IPropertyMapper>(x => true, (x, y) => executed = true);
			d.Apply(null, null);
			executed.Should().Be.True();
		}
	}
}