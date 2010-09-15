using System;
using ConfOrm.Mappers;
using ConfOrm.Shop.Subclassing;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrm.ShopTests.Subclassing
{
	public class SubclassDiscriminatorValueAsEnumValueApplierTest
	{
		public enum EntitiesTypes
		{
			Post = 0,
			Contribute = 5,
			Page = 7
		}
		public enum EntitiesTypesFixed
		{
			Post = 0,
			Contribute = 5,
			Page = 7
		}

		private class Item
		{
			public int Id { get; set; }
		}

		private class Post : Item { }
		private class Contribute : Post { }
		private class Page : Item { }
		private class Gallery : Item { }
		private class SomethingOutOfHierarchy { }

		[Test]
		public void WhenNotEnumThenThrow()
		{
			Executing.This(() => new SubclassDiscriminatorValueAsEnumValueApplier<Item, int>()).Should().Throw<NotSupportedException>();
		}

		[Test]
		public void AlwaysMatch()
		{
			// the pattern can match only when called for subclass
			var applier = new SubclassDiscriminatorValueAsEnumValueApplier<Item, EntitiesTypes>();
			applier.Match(typeof(Item)).Should().Be.True();
		}

		[Test]
		public void WhenOutsideTheDefinedHierarchyThenNoMatch()
		{
			var applier = new SubclassDiscriminatorValueAsEnumValueApplier<Item, EntitiesTypes>();
			applier.Match(typeof(SomethingOutOfHierarchy)).Should().Be.False();
		}

		[Test]
		public void WhenTheNameMatchThenApplyEnumValue()
		{
			var applier = new SubclassDiscriminatorValueAsEnumValueApplier<Item, EntitiesTypes>();
			var mapper = new Mock<ISubclassAttributesMapper>();

			applier.Apply(typeof(Contribute), mapper.Object);

			mapper.Verify(cm => cm.DiscriminatorValue(It.Is<int>(n => ((int)EntitiesTypes.Contribute).Equals(n))));
		}

		[Test]
		public void WhenTheNameNoMatchThenThrows()
		{
			var applier = new SubclassDiscriminatorValueAsEnumValueApplier<Item, EntitiesTypesFixed>();
			var mapper = new Mock<ISubclassAttributesMapper>();

			applier.Executing(a => a.Apply(typeof(Gallery), mapper.Object)).Throws<ArgumentException>();
		}
	}
}