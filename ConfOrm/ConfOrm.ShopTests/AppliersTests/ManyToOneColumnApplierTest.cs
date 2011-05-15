using NHibernate.Mapping.ByCode;
using ConfOrm.NH;
using ConfOrm.Shop.Appliers;
using ConfOrm.Shop.CoolNaming;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrm.ShopTests.AppliersTests
{
	public class ManyToOneColumnApplierTest
	{
		private class MyClass
		{
			public MyOtherClass OtherClass { get; set; }
			public MyComponent Component { get; set; }
		}

		private class MyOtherClass
		{

		}

		private class MyComponent
		{
			public MyOtherClass AnotherClass { get; set; }			
		}

		[Test]
		public void WhenInEntityThenApplyPropertyName()
		{
			var applier = new ManyToOneColumnApplier();
			var path = new PropertyPath(null, ForClass<MyClass>.Property(x => x.OtherClass));
			var mayToOneMapper = new Mock<IManyToOneMapper>();

			applier.Match(path).Should().Be.True();
			applier.Apply(path, mayToOneMapper.Object);

			mayToOneMapper.Verify(x => x.Column(It.Is<string>(columnName => columnName == "OtherClassId")));
		}

		[Test]
		public void WhenInComponentThenApplyPropertyPath()
		{
			var applier = new ManyToOneColumnApplier();
			var level0 = new PropertyPath(null, ForClass<MyClass>.Property(x => x.Component));
			var path = new PropertyPath(level0, ForClass<MyComponent>.Property(x => x.AnotherClass));
			var mayToOneMapper = new Mock<IManyToOneMapper>();

			applier.Match(path).Should().Be.True();
			applier.Apply(path, mayToOneMapper.Object);

			mayToOneMapper.Verify(x => x.Column(It.Is<string>(columnName => columnName == "ComponentAnotherClassId")));
		}
	}
}