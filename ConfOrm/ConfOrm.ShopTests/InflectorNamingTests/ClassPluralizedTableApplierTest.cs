using System;
using ConfOrm.Mappers;
using ConfOrm.Shop.InflectorNaming;
using ConfOrm.Shop.Inflectors;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrm.ShopTests.InflectorNamingTests
{
	public class ClassPluralizedTableApplierTest
	{
		private class Person
		{
			
		}
		[Test]
		public void CtorProtection()
		{
			Executing.This(() => new ClassPluralizedTableApplier(null)).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void WhenNotValidTypeThenNoMatch()
		{
			var inflector = new Mock<IInflector>();
			var applier = new ClassPluralizedTableApplier(inflector.Object);
			applier.Match(null).Should().Be.False();
		}

		[Test]
		public void WhenValidTypeThenMatch()
		{
			var inflector = new Mock<IInflector>();
			var applier = new ClassPluralizedTableApplier(inflector.Object);
			applier.Match(typeof(Person)).Should().Be.True();
		}

		[Test]
		public void WhenApplyThenCallInflector()
		{
			var inflector = new Mock<IInflector>();
			inflector.Setup(i => i.Pluralize("Person")).Returns("People");
			var applier = new ClassPluralizedTableApplier(inflector.Object);
			var mapper = new Mock<IClassAttributesMapper>();

			applier.Apply(typeof(Person), mapper.Object);

			mapper.Verify(m=> m.Table("People"));
		}
	}
}