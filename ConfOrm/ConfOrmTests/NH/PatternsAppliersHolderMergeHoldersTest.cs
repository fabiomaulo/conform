using System;
using System.Linq;
using System.Reflection;
using ConfOrm;
using ConfOrm.Mappers;
using ConfOrm.NH;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class PatternsAppliersHolderMergeHoldersTest
	{
		private class ACollectionApplier : IPatternApplier<MemberInfo, ICollectionPropertiesMapper>
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
			ActionAssert.Throws<ArgumentNullException>(() => source.Merge(null));
		}

		[Test]
		public void WhenSecondIsNullThenReturnFirstCleanedOfDuplications()
		{
			IPatternsAppliersHolder source = new EmptyPatternsAppliersHolder();
			source.Collection.Add(new ACollectionApplier());
			source.Collection.Add(new ACollectionApplier());

			var mergeResult = source.Merge(null);
		
			mergeResult.Should().Not.Be.SameInstanceAs(source);
			mergeResult.Collection.Count.Should().Be(1);
		}

		[Test]
		public void WhenSecondIsNotNullThenMerge()
		{
			IPatternsAppliersHolder first = new EmptyPatternsAppliersHolder();
			first.Collection.Add(new ACollectionApplier());
			first.RootClass.Add(new ARootClassApplier());

			IPatternsAppliersHolder second = new EmptyPatternsAppliersHolder();
			second.RootClass.Add(new ARootClassApplier());

			var mergeResult = first.Merge(second);

			mergeResult.Collection.Count.Should().Be(1);
			mergeResult.RootClass.Count.Should().Be(1);
		}

		[Test]
		public void MergeShouldGetAllPropertiesOfPatternsAppliersHolderOfBothSide()
		{
			string[] propertiesOfIPatternsAppliersHolder =
				typeof (IPatternsAppliersHolder).GetProperties().Select(x => x.Name).ToArray();

			var first = new PatternsAppliersHolderPropertyCallingMock();
			var second = new PatternsAppliersHolderPropertyCallingMock();
			
			first.Merge(second);

			first.PropertiesGettersUsed.Should().Have.SameValuesAs(propertiesOfIPatternsAppliersHolder);
			second.PropertiesGettersUsed.Should().Have.SameValuesAs(propertiesOfIPatternsAppliersHolder);
		}
	}
}