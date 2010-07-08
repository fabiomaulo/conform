using ConfOrm.Mappers;
using ConfOrm.Shop.Subclassing;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrm.ShopTests.Subclassing
{
	public class SubclassDiscriminatorValueAsClassNameApplierTest
	{
		private class MyClass
		{
			
		}
		[Test]
		public void WhenMatchCalledThenAlwaysMatch()
		{
			var applier = new SubclassDiscriminatorValueAsClassNameApplier();
			applier.Match(null).Should().Be.True();
		}

		[Test]
		public void AlwaysApplyDicriminatorValueAsClassName()
		{
			var applier = new SubclassDiscriminatorValueAsClassNameApplier();
			var mapper = new Mock<ISubclassAttributesMapper>();

			applier.Apply(typeof(MyClass), mapper.Object);

			mapper.Verify(cm => cm.DiscriminatorValue(It.Is<string>(n => "MyClass".Equals(n))));
		}
	}
}