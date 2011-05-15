using System.Collections.Generic;
using NHibernate.Mapping.ByCode;
using ConfOrm.NH;
using ConfOrm.Shop.NamingAppliers;
using Moq;
using NUnit.Framework;

namespace ConfOrm.ShopTests.AppliersTests
{
	public class ComponentPropertyColumnNameApplierTest
	{
		private class MyClass
		{
			public string Fake1 { get; set; }
		}


		private class MyClassWithComponent
		{
			public MyComponent Component1 { get; set; }
			public IEnumerable<MyComponent> Components { get; set; }
		}

		private class MyComponent
		{
			public MyClass MyNested { get; set; }
			public string Fake0 { get; set; }
		}

		[Test]
		public void WhenLevel1ThenMatch()
		{
			var level0 = new PropertyPath(null, ForClass<MyClassWithComponent>.Property(x => x.Component1));
			var level1 = new PropertyPath(level0, ForClass<MyComponent>.Property(x => x.MyNested));
			var level2 = new PropertyPath(level1, ForClass<MyClass>.Property(x => x.Fake1));
			var pattern = new ComponentPropertyColumnNameApplier();
			var mapper = new Mock<IPropertyMapper>();
			
			pattern.Apply(level2, mapper.Object);
			mapper.Verify(x => x.Column(It.Is<string>(columnName => columnName == "Component1MyNestedFake1")));
		}

		[Test]
		public void WhenLevel1ThroughCollectionThenShouldIgnoreTheCollectionPropertyName()
		{
			var level0 = new PropertyPath(null, ForClass<MyClassWithComponent>.Property(x => x.Components));
			var level1 = new PropertyPath(level0, ForClass<MyComponent>.Property(x => x.MyNested));
			var level2 = new PropertyPath(level1, ForClass<MyClass>.Property(x => x.Fake1));
			var pattern = new ComponentPropertyColumnNameApplier();
			var mapper = new Mock<IPropertyMapper>();

			pattern.Apply(level2, mapper.Object);
			mapper.Verify(x => x.Column(It.Is<string>(columnName => columnName == "MyNestedFake1")));
		}

		[Test]
		public void WhenLevel0ThroughCollectionThenShouldIgnoreTheCollectionPropertyName()
		{
			var level0 = new PropertyPath(null, ForClass<MyClassWithComponent>.Property(x => x.Components));
			var level1 = new PropertyPath(level0, ForClass<MyComponent>.Property(x => x.Fake0));
			var pattern = new ComponentPropertyColumnNameApplier();
			var mapper = new Mock<IPropertyMapper>();

			pattern.Apply(level1, mapper.Object);
			mapper.Verify(x => x.Column(It.Is<string>(columnName => columnName == "Fake0")));
		}
	}
}