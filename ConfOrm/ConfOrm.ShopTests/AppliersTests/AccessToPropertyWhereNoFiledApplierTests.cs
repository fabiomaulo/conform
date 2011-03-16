using ConfOrm.Mappers;
using ConfOrm.Shop.Appliers;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrm.ShopTests.AppliersTests
{
	public class AccessToPropertyWhereNoFiledApplierTests
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
			var pattern = new AccessToPropertyWhereNoFieldApplier<IPropertyMapper>();
			var member = ForClass<MyClass>.Field("aField");
			pattern.Match(member).Should().Be.False();
		}

		[Test]
		public void WhenAutoPropertyMatch()
		{
			var pattern = new AccessToPropertyWhereNoFieldApplier<IPropertyMapper>();
			var member = ForClass<MyClass>.Property(x => x.Autoprop);
			pattern.Match(member).Should().Be.True();
		}

		[Test]
		public void WhenPropertyWithBackFieldNoMatch()
		{
			var pattern = new AccessToPropertyWhereNoFieldApplier<IPropertyMapper>();
			var member = ForClass<MyClass>.Property(x => x.PropWithField);
			pattern.Match(member).Should().Be.False();
		}

		[Test]
		public void WhenReadOnlyPropertyWithBackFieldNoMatch()
		{
			var pattern = new AccessToPropertyWhereNoFieldApplier<IPropertyMapper>();
			var member = ForClass<MyClass>.Property(x => x.ReadonlyPropWithField);
			pattern.Match(member).Should().Be.False();
		}

		[Test]
		public void WhenPropertyWithoutFieldMatch()
		{
			var pattern = new AccessToPropertyWhereNoFieldApplier<IPropertyMapper>();
			var member = ForClass<MyClass>.Property(x => x.ReadonlyProp);
			pattern.Match(member).Should().Be.True();
		}

		[Test]
		public void AlwaysApplyProperty()
		{
			var mapper = new Mock<IPropertyMapper>();
			var pattern = new AccessToPropertyWhereNoFieldApplier<IPropertyMapper>();
			pattern.Apply(null, mapper.Object);
			mapper.Verify(x => x.Access(It.Is<Accessor>(a => a == Accessor.Property)));
		}
	}
}