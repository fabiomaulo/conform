using System;
using ConfOrm.Mappers;
using ConfOrm.Shop.Subclassing;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrm.ShopTests.Subclassing
{
	public class ClassDiscriminatorValueAsEnumValueApplierTest
	{
		public enum EntitiesTypes
		{
			Unknown = -1,
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

		private class Post:Item{}
		private class Contribute:Post{}
		private class Page:Item{}
		private class Gallery : Item { }
		private class SomethingOutOfHierarchy { }

		[Test]
		public void WhenNotEnumThenThrow()
		{
			var orm = new Mock<IDomainInspector>();
			Executing.This(()=> new ClassDiscriminatorValueAsEnumValueApplier<Item, int>(orm.Object)).Should().Throw<NotSupportedException>();
		}

		[Test]
		public void WhenEntityIsNotTablePerHierarchyThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsTablePerClassHierarchy(typeof(Item))).Returns(false);
			IPatternApplier<Type, IClassAttributesMapper> applier = new ClassDiscriminatorValueAsEnumValueApplier<Item, EntitiesTypes>(orm.Object);
			applier.Match(typeof(Item)).Should().Be.False();
		}

		[Test]
		public void WhenEntityIsTablePerHierarchyThenMatch()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsTablePerClassHierarchy(typeof(Item))).Returns(true);
			IPatternApplier<Type, IClassAttributesMapper> applier = new ClassDiscriminatorValueAsEnumValueApplier<Item, EntitiesTypes>(orm.Object);
			applier.Match(typeof(Item)).Should().Be.True();
		}

		[Test]
		public void WhenEntityIsTablePerHierarchyButOutsideTheDefinedHierarchyThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsTablePerClassHierarchy(typeof(Item))).Returns(true);
			orm.Setup(x => x.IsTablePerClassHierarchy(typeof(SomethingOutOfHierarchy))).Returns(true);
			var applier = new ClassDiscriminatorValueAsEnumValueApplier<Item, EntitiesTypes>(orm.Object);
			applier.Match(typeof(SomethingOutOfHierarchy)).Should().Be.False();
		}

		[Test]
		public void WhenTheNameMatchThenApplyEnumValue()
		{
			var orm = new Mock<IDomainInspector>();
			var applier = new ClassDiscriminatorValueAsEnumValueApplier<Item, EntitiesTypes>(orm.Object);

			var mapper = new Mock<IClassAttributesMapper>();

			applier.Apply(typeof(Contribute), mapper.Object);

			mapper.Verify(cm => cm.DiscriminatorValue(It.Is<int>(n => ((int)EntitiesTypes.Contribute).Equals(n))));
		}

		[Test]
		public void WhenTheNameNoMatchAndEnumCotainsUnknowThenApplyUnknowEnumValue()
		{
			var orm = new Mock<IDomainInspector>();
			var applier = new ClassDiscriminatorValueAsEnumValueApplier<Item, EntitiesTypes>(orm.Object);

			var mapper = new Mock<IClassAttributesMapper>();

			applier.Apply(typeof(Gallery), mapper.Object);

			mapper.Verify(cm => cm.DiscriminatorValue(It.Is<int>(n => ((int)EntitiesTypes.Unknown).Equals(n))));
		}

		[Test]
		public void WhenTheNameNoMatchThenThrows()
		{
			var orm = new Mock<IDomainInspector>();
			var applier = new ClassDiscriminatorValueAsEnumValueApplier<Item, EntitiesTypesFixed>(orm.Object);
			var mapper = new Mock<IClassAttributesMapper>();

			applier.Executing(a => a.Apply(typeof(Gallery), mapper.Object)).Throws<ArgumentException>();
		}
	}
}