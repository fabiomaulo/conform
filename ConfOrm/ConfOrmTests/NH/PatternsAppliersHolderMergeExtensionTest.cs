using System;
using System.Linq;
using System.Reflection;
using ConfOrm;
using NHibernate.Mapping.ByCode;
using ConfOrm.NH;
using ConfOrm.Patterns;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class PatternsAppliersHolderMergeExtensionTest
	{
		[Test]
		public void WhenSourceIsNullThenThrow()
		{
			IPatternsAppliersHolder source = null;
			Executing.This(() => source.Merge<PropertyPath, IPropertyMapper>(null)).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void WhenApplierIsNullThenNotThrow()
		{
			IPatternsAppliersHolder source = new EmptyPatternsAppliersHolder();
			Executing.This(() => source.Merge<PropertyPath, IPropertyMapper>(null)).Should().NotThrow();
		}

		private IPatternsAppliersHolder GetPatternsAppliersHolderWithApplierAdded<TSubject, TApplyTo>()
		{
			IPatternsAppliersHolder source = new EmptyPatternsAppliersHolder();
			var applier = new Mock<IPatternApplier<TSubject, TApplyTo>>();
			source.Merge(applier.Object);
			return source;
		}

		[Test]
		public void WhenApplier_TypeClassAttributesMapper_ThenAddToItsCollection()
		{
			IPatternsAppliersHolder source = GetPatternsAppliersHolderWithApplierAdded<Type, IClassAttributesMapper>();
			source.RootClass.Count.Should().Be(1);
		}

		[Test]
		public void WhenApplier_TypeJoinedSubclassAttributesMapper_ThenAddToItsCollection()
		{
			IPatternsAppliersHolder source = GetPatternsAppliersHolderWithApplierAdded<Type, IJoinedSubclassAttributesMapper>();
			source.JoinedSubclass.Count.Should().Be(1);
		}

		[Test]
		public void WhenApplier_TypeSubclassAttributesMapper_ThenAddToItsCollection()
		{
			IPatternsAppliersHolder source = GetPatternsAppliersHolderWithApplierAdded<Type, ISubclassAttributesMapper>();
			source.Subclass.Count.Should().Be(1);
		}

		[Test]
		public void WhenApplier_TypeUnionSubclassAttributesMapper_ThenAddToItsCollection()
		{
			IPatternsAppliersHolder source = GetPatternsAppliersHolderWithApplierAdded<Type, IUnionSubclassAttributesMapper>();
			source.UnionSubclass.Count.Should().Be(1);
		}

		[Test]
		public void WhenApplier_MemberInfoIdMapper_ThenAddToItsCollection()
		{
			IPatternsAppliersHolder source = GetPatternsAppliersHolderWithApplierAdded<MemberInfo, IIdMapper>();
			source.Poid.Count.Should().Be(1);
		}

		[Test]
		public void WhenApplier_MemberInfoPropertyMapper_ThenAddToItsCollection()
		{
			IPatternsAppliersHolder source = GetPatternsAppliersHolderWithApplierAdded<MemberInfo, IPropertyMapper>();
			source.Property.Count.Should().Be(1);
		}

		[Test]
		public void WhenApplier_PropertyPathPropertyMapper_ThenAddToItsCollection()
		{
			IPatternsAppliersHolder source = GetPatternsAppliersHolderWithApplierAdded<PropertyPath, IPropertyMapper>();
			source.PropertyPath.Count.Should().Be(1);
		}

		[Test]
		public void WhenApplier_MemberInfoManyToOneMapper_ThenAddToItsCollection()
		{
			IPatternsAppliersHolder source = GetPatternsAppliersHolderWithApplierAdded<MemberInfo, IManyToOneMapper>();
			source.ManyToOne.Count.Should().Be(1);
		}

		[Test]
		public void WhenApplier_PropertyPathManyToOneMapper_ThenAddToItsCollection()
		{
			IPatternsAppliersHolder source = GetPatternsAppliersHolderWithApplierAdded<PropertyPath, IManyToOneMapper>();
			source.ManyToOnePath.Count.Should().Be(1);
		}

		[Test]
		public void WhenApplier_MemberInfoOneToOneMapper_ThenAddToItsCollection()
		{
			IPatternsAppliersHolder source = GetPatternsAppliersHolderWithApplierAdded<MemberInfo, IOneToOneMapper>();
			source.OneToOne.Count.Should().Be(1);
		}

		[Test]
		public void WhenApplier_PropertyPathOneToOneMapper_ThenAddToItsCollection()
		{
			IPatternsAppliersHolder source = GetPatternsAppliersHolderWithApplierAdded<PropertyPath, IOneToOneMapper>();
			source.OneToOnePath.Count.Should().Be(1);
		}

		[Test]
		public void WhenApplier_MemberInfoAnyMapper_ThenAddToItsCollection()
		{
			IPatternsAppliersHolder source = GetPatternsAppliersHolderWithApplierAdded<MemberInfo, IAnyMapper>();
			source.Any.Count.Should().Be(1);
		}

		[Test]
		public void WhenApplier_PropertyPathAnyMapper_ThenAddToItsCollection()
		{
			IPatternsAppliersHolder source = GetPatternsAppliersHolderWithApplierAdded<PropertyPath, IAnyMapper>();
			source.AnyPath.Count.Should().Be(1);
		}

		[Test]
		public void WhenApplier_MemberInfoCollectionPropertiesMapper_ThenAddToItsCollection()
		{
			IPatternsAppliersHolder source = GetPatternsAppliersHolderWithApplierAdded<MemberInfo, ICollectionPropertiesMapper>();
			source.Collection.Count.Should().Be(1);
		}

		[Test]
		public void WhenApplier_PropertyPathCollectionPropertiesMapper_ThenAddToItsCollection()
		{
			IPatternsAppliersHolder source = GetPatternsAppliersHolderWithApplierAdded<PropertyPath, ICollectionPropertiesMapper>();
			source.CollectionPath.Count.Should().Be(1);
		}

		[Test]
		public void WhenApplier_MemberInfoManyToManyMapper_ThenAddToItsCollection()
		{
			IPatternsAppliersHolder source = GetPatternsAppliersHolderWithApplierAdded<MemberInfo, IManyToManyMapper>();
			source.ManyToMany.Count.Should().Be(1);
		}

		[Test]
		public void WhenApplier_PropertyPathManyToManyMapper_ThenAddToItsCollection()
		{
			IPatternsAppliersHolder source = GetPatternsAppliersHolderWithApplierAdded<PropertyPath, IManyToManyMapper>();
			source.ManyToManyPath.Count.Should().Be(1);
		}

		[Test]
		public void WhenApplier_MemberInfoElementMapper_ThenAddToItsCollection()
		{
			IPatternsAppliersHolder source = GetPatternsAppliersHolderWithApplierAdded<MemberInfo, IElementMapper>();
			source.Element.Count.Should().Be(1);
		}

		[Test]
		public void WhenApplier_PropertyPathElementMapper_ThenAddToItsCollection()
		{
			IPatternsAppliersHolder source = GetPatternsAppliersHolderWithApplierAdded<PropertyPath, IElementMapper>();
			source.ElementPath.Count.Should().Be(1);
		}

		[Test]
		public void WhenApplierIsNotSupportedThenThrows()
		{
			IPatternsAppliersHolder source = new EmptyPatternsAppliersHolder();
			var applier = new Mock<IPatternApplier<int, string>>();
			Executing.This(()=>source.Merge(applier.Object)).Should().Throw<ArgumentOutOfRangeException>();
		}

		[Test]
		public void WhenExistsApplierOfSameTypeThenDoesNotAdd()
		{
			IPatternsAppliersHolder source = new EmptyPatternsAppliersHolder();
			var toAdd = new BidirectionalManyToManyInverseApplier();
			source.Collection.Add(toAdd);
			source.Merge(new BidirectionalManyToManyInverseApplier());

			source.Collection.Count.Should().Be(1);
			source.Collection.Single().Should().Be.SameInstanceAs(toAdd);
		}

		[Test]
		public void WhenAddDelegatedApplierThenAlwaysAdd()
		{
			IPatternsAppliersHolder source = new EmptyPatternsAppliersHolder();
			source.Collection.Add(new DelegatedApplier<MemberInfo, ICollectionPropertiesMapper>(x => true, x => { }));
			source.Merge(new DelegatedApplier<MemberInfo, ICollectionPropertiesMapper>(x => true, x => { }));

			source.Collection.Count.Should().Be(2);
		}

		[Test]
		public void WhenAddDelegatedAdvancedApplierThenAlwaysAdd()
		{
			IPatternsAppliersHolder source = new EmptyPatternsAppliersHolder();
			source.Collection.Add(new DelegatedAdvancedApplier<MemberInfo, ICollectionPropertiesMapper>(x => true, (x, y) => { }));
			source.Merge(new DelegatedAdvancedApplier<MemberInfo, ICollectionPropertiesMapper>(x => true, (x, y) => { }));

			source.Collection.Count.Should().Be(2);
		}

		private class BidirectionalManyToManyInverseApplier : IPatternApplier<MemberInfo, ICollectionPropertiesMapper>
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