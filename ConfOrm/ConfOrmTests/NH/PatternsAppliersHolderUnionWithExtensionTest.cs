using System;
using System.Linq;
using System.Reflection;
using ConfOrm;
using ConfOrm.Mappers;
using ConfOrm.NH;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class PatternsAppliersHolderUnionWithExtensionTest
	{
		[Test]
		public void WhenSourceIsNullThenThrow()
		{
			IPatternsAppliersHolder source = null;
			ActionAssert.Throws<ArgumentNullException>(() => source.UnionWith<PropertyPath, IPropertyMapper>(null));
		}

		[Test]
		public void WhenApplierIsNullThenNotThrow()
		{
			IPatternsAppliersHolder source = new EmptyPatternsAppliersHolder();
			ActionAssert.NotThrow(() => source.UnionWith<PropertyPath, IPropertyMapper>(null));
		}

		[Test]
		public void WhenDoesNotExistWithSameNameThenAdd()
		{
			IPatternsAppliersHolder source = new EmptyPatternsAppliersHolder();

			var domainInspector = new Mock<IDomainInspector>();
			var toAdd = new ConfOrm.Patterns.BidirectionalManyToManyInverseApplier(domainInspector.Object);

			source.UnionWith(toAdd);
			source.Collection.Count.Should().Be(1);
		}

		[Test]
		public void WhenExistWithSameNameThenRemoveAndAdd()
		{
			IPatternsAppliersHolder source = new EmptyPatternsAppliersHolder();

			var domainInspector = new Mock<IDomainInspector>();
			var toRemove = new ConfOrm.Patterns.BidirectionalManyToManyInverseApplier(domainInspector.Object);
			source.Collection.Add(toRemove);

			var toAdd = new BidirectionalManyToManyInverseApplier();

			source.UnionWith(toAdd);
			source.Collection.Count.Should().Be(1);
			source.Collection.Single().Should().Be.SameInstanceAs(toAdd);
		}

		private class BidirectionalManyToManyInverseApplier: IPatternApplier<MemberInfo, ICollectionPropertiesMapper>
		{
			#region Implementation of IPattern<MemberInfo>

			public bool Match(MemberInfo subject)
			{
				throw new NotImplementedException();
			}

			#endregion

			#region Implementation of IPatternApplier<MemberInfo,ICollectionPropertiesMapper>

			public void Apply(MemberInfo subject, ICollectionPropertiesMapper applyTo)
			{
				throw new NotImplementedException();
			}

			#endregion
		}
	}
}