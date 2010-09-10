using System;
using ConfOrm.Shop.DearDbaNaming;
using ConfOrm.Shop.Inflectors;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrm.ShopTests.DearDbaNamingTests
{
	public class ManyToManyTableApplierTest
	{
		[Test]
		public void CtorProtection()
		{
			Executing.This(() => new ManyToManyTableApplier(null, null)).Should().Throw<ArgumentNullException>();
			Executing.This(() => new ManyToManyTableApplier(null, (new Mock<IInflector>()).Object)).Should().Throw<ArgumentNullException>();
			Executing.This(() => new ManyToManyTableApplier((new Mock<IDomainInspector>()).Object, null)).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void ShouldApplyPluralsWithUnderscoreUpperCase()
		{
			var inflector = new EnglishInflector();
			var applier = new ManyToManyTableApplier((new Mock<IDomainInspector>()).Object, inflector);
			applier.GetTableNameForRelation(new[] {"Person", "Role"}).Should().Be("PEOPLE_ROLES");
		}
	}
}