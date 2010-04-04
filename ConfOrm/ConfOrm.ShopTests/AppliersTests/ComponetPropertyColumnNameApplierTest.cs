using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Shop.Appliers;
using ConfOrm.Shop.CoolNaming;
using Moq;
using NUnit.Framework;

namespace ConfOrm.ShopTests.AppliersTests
{
	public class ComponetPropertyColumnNameApplierTest
	{
		private class MyClass
		{
			public string Fake1 { get; set; }
		}


		private class MyClassWithComponent
		{
			public MyComponent Component1 { get; set; }
		}

		private class MyComponent
		{
			public MyClass MyNested { get; set; }
		}

		[Test]
		public void WhenLevel1ThenMatch()
		{
			var level0 = new PropertyPath(null, ForClass<MyClassWithComponent>.Property(x => x.Component1));
			var level1 = new PropertyPath(level0, ForClass<MyComponent>.Property(x => x.MyNested));
			var level2 = new PropertyPath(level1, ForClass<MyClass>.Property(x => x.Fake1));
			var pattern = new ComponetPropertyColumnNameApplier();
			var mapper = new Mock<IPropertyMapper>();
			
			pattern.Apply(level2, mapper.Object);
			mapper.Verify(x => x.Column(It.Is<string>(columnName => columnName == "Component1MyNestedFake1")));
		}
	}
}