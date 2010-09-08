using System;
using ConfOrm.Shop.Inflectors;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrm.ShopTests.InflectorsTests
{
	public class InflectorRuleTest
	{
		[Test]
		public void Ctor()
		{
			Executing.This(() => new NounsRule(null, null)).Should().Throw<ArgumentNullException>();
			Executing.This(() => new NounsRule("$s", null)).Should().Throw<ArgumentNullException>();
			Executing.This(() => new NounsRule(null, "s")).Should().Throw<ArgumentNullException>();
			Executing.This(() => new NounsRule("", "s")).Should().Throw<ArgumentNullException>();
			Executing.This(() => new NounsRule("$s", "")).Should().NotThrow();
		}

		[Test]
		public void Equality()
		{
			var r = new NounsRule("$s", "s");
			r.Should().Be.EqualTo(new NounsRule("$s", "s"));
			r.Should().Not.Be.EqualTo(new NounsRule("$s", "ss"));
		}

		[Test]
		public void HashCode()
		{
			var r = new NounsRule("$s", "s");
			r.GetHashCode().Should().Be.EqualTo((new NounsRule("$s", "s")).GetHashCode());
			r.GetHashCode().Should().Not.Be.EqualTo((new NounsRule("$s", "ss")).GetHashCode());
		}

		[Test]
		public void Apply()
		{
			var r = new NounsRule("(pizz)a$", "$1e");
			r.Apply("pizza").Should().Be.EqualTo("pizze");
		}
	}
}