using NHibernate.Mapping.ByCode;
using ConfOrm.Shop.Appliers;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrm.ShopTests.AppliersTests
{
	public class AlwaysAccessToFieldWhereAvailableApplierTests
	{
		private class MyClass
		{
			private string aField;
			public string Autoprop { get; set; }
			private string propWithField;
			public string PropWithField
			{
				get { return propWithField; }
				set { propWithField = value; }
			}
			private string _readonlyPropWithField;
			public string ReadonlyPropWithField
			{
				get { return _readonlyPropWithField; }
			}
			public string ReadonlyProp
			{
				get { return ""; }
			}
		}

		[Test]
		public void WhenFieldNoMatch()
		{
			var pattern = new AlwaysAccessToFieldWhereAvailableApplier<IPropertyMapper>();
			var member = ForClass<MyClass>.Field("aField");
			pattern.Match(member).Should().Be.False();
		}

		[Test]
		public void WhenAutoPropertyNoMatch()
		{
			var pattern = new AlwaysAccessToFieldWhereAvailableApplier<IPropertyMapper>();
			var member = ForClass<MyClass>.Property(x => x.Autoprop);
			pattern.Match(member).Should().Be.False();
		}

		[Test]
		public void WhenPropertyWithBackFieldMatch()
		{
			var pattern = new AlwaysAccessToFieldWhereAvailableApplier<IPropertyMapper>();
			var member = ForClass<MyClass>.Property(x => x.PropWithField);
			pattern.Match(member).Should().Be.True();
		}

		[Test]
		public void WhenReadOnlyPropertyWithBackFieldMatch()
		{
			var pattern = new AlwaysAccessToFieldWhereAvailableApplier<IPropertyMapper>();
			var member = ForClass<MyClass>.Property(x => x.ReadonlyPropWithField);
			pattern.Match(member).Should().Be.True();
		}

		[Test]
		public void WhenPropertyWithoutFieldNoMatch()
		{
			var pattern = new AlwaysAccessToFieldWhereAvailableApplier<IPropertyMapper>();
			var member = ForClass<MyClass>.Property(x => x.ReadonlyProp);
			pattern.Match(member).Should().Be.False();
		}

		[Test]
		public void ApplierAlwaysField()
		{
			var mapper = new Mock<IPropertyMapper>();
			var pattern = new AlwaysAccessToFieldWhereAvailableApplier<IPropertyMapper>();
			pattern.Apply(null, mapper.Object);
			mapper.Verify(x => x.Access(It.Is<Accessor>(a => a == Accessor.Field)));
		}

	}
}