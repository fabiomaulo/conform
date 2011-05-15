using System;
using System.Linq;
using System.Reflection;
using ConfOrm;
using NHibernate.Mapping.ByCode;
using ConfOrm.NH;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class PatternsAppliersHolderUnionWithHoldersTest
	{
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
		private class ARootClassApplier : IPatternApplier<Type, IClassAttributesMapper>
		{
			#region Implementation of IPattern<Type>

			public bool Match(Type subject)
			{
				throw new NotImplementedException();
			}

			#endregion

			#region Implementation of IPatternApplier<Type,IClassAttributesMapper>

			public void Apply(Type subject, IClassAttributesMapper applyTo)
			{
				throw new NotImplementedException();
			}

			#endregion
		}

		[Test]
		public void WhenSourceIsNullThenThrow()
		{
			IPatternsAppliersHolder source = null;
			Executing.This(() => source.UnionWith(null)).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void WhenSecondIsNullThenReturnFirstCleanedOfDuplications()
		{
			IPatternsAppliersHolder source = new EmptyPatternsAppliersHolder();

			var domainInspector = new Mock<IDomainInspector>();
			source.Collection.Add(new ConfOrm.Patterns.BidirectionalManyToManyInverseApplier(domainInspector.Object));
			source.Collection.Add(new BidirectionalManyToManyInverseApplier());

			var unionResult = source.UnionWith(null);

			unionResult.Should().Not.Be.SameInstanceAs(source);
			unionResult.Collection.Count.Should().Be(1);
		}

		[Test]
		public void WhenSecondIsNotNullThenUnion()
		{
			IPatternsAppliersHolder first = new EmptyPatternsAppliersHolder();

			var domainInspector = new Mock<IDomainInspector>();
			first.Collection.Add(new ConfOrm.Patterns.BidirectionalManyToManyInverseApplier(domainInspector.Object));

			IPatternsAppliersHolder second = new EmptyPatternsAppliersHolder();
			var bidirectionalManyToManyInverseApplier = new BidirectionalManyToManyInverseApplier();
			second.Collection.Add(bidirectionalManyToManyInverseApplier);
			second.RootClass.Add(new ARootClassApplier());

			var unionResult = first.UnionWith(second);

			unionResult.Collection.Count.Should().Be(1);
			unionResult.RootClass.Count.Should().Be(1);
			unionResult.Collection.Single().Should().Be.SameInstanceAs(bidirectionalManyToManyInverseApplier);
		}

		[Test]
		public void UnionShouldGetAllPropertiesOfPatternsAppliersHolderOfBothSide()
		{
			string[] propertiesOfIPatternsAppliersHolder =
				typeof(IPatternsAppliersHolder).GetProperties().Select(x => x.Name).ToArray();

			var first = new PatternsAppliersHolderPropertyCallingMock();
			var second = new PatternsAppliersHolderPropertyCallingMock();

			first.UnionWith(second);

			first.PropertiesGettersUsed.Should().Have.SameValuesAs(propertiesOfIPatternsAppliersHolder);
			second.PropertiesGettersUsed.Should().Have.SameValuesAs(propertiesOfIPatternsAppliersHolder);
		}
	}
}