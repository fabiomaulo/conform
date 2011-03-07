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
		private class Person
		{
			
		}
		private class Role
		{

		}
		[Test]
		public void CtorProtection()
		{
			Executing.This(() => new ManyToManyPluralizedTableApplier(null, null)).Should().Throw<ArgumentNullException>();
			Executing.This(() => new ManyToManyPluralizedTableApplier(null, (new Mock<IInflector>()).Object)).Should().Throw<ArgumentNullException>();
			Executing.This(() => new ManyToManyPluralizedTableApplier((new Mock<IDomainInspector>()).Object, null)).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void ShouldApplyPluralsWithUnderscoreUpperCase()
		{
			var inflector = new EnglishInflector();
			var applier = new ManyToManyPluralizedTableApplier((new Mock<IDomainInspector>()).Object, inflector);
			applier.GetTableNameForRelation(new Relation(typeof(Person), typeof(Role)), new Relation(typeof(Role), typeof(Person))).Should().Be("PEOPLE_ROLES");
		}
	}
}