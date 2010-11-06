using System.Linq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.PolymorphismRelationsTests
{
	public class CaseWithHierarchyTest
	{
		private interface IRelation
		{
		}

		private class MyRelation : IRelation
		{
			public int Id { get; set; }
		}

		private class MyRelationLevel1 : MyRelation
		{
		}

		private class Relation1
		{
		}

		private class MyRelation1 : Relation1
		{
			public int Id { get; set; }
		}

		private class MyRelation1Lvel1 : MyRelation1
		{
		}

		[Test]
		public void WhenEntitiesOfDomainThenOnlyReturnFirstImplementorInTheHierarchy()
		{
			var domainAnalyzer = new PolymorphismResolver();
			domainAnalyzer.Add(typeof(MyRelation));
			domainAnalyzer.Add(typeof(MyRelation1));
			domainAnalyzer.Add(typeof(MyRelationLevel1));
			domainAnalyzer.Add(typeof(MyRelation1Lvel1));
			domainAnalyzer.GetBaseImplementors(typeof(IRelation)).Single().Should().Be(typeof(MyRelation));
			domainAnalyzer.GetBaseImplementors(typeof(Relation1)).Single().Should().Be(typeof(MyRelation1));
		}

		[Test]
		public void WhenRegisterWholeDomainThenOnlyReturnFirstNoJumpedImplementorInTheHierarchy()
		{
			var domainAnalyzer = new PolymorphismResolver();
			domainAnalyzer.Add(typeof(MyRelation));
			domainAnalyzer.Add(typeof(MyRelation1));
			domainAnalyzer.Add(typeof(MyRelationLevel1));
			domainAnalyzer.Add(typeof(MyRelation1Lvel1));
			domainAnalyzer.Add(typeof(IRelation));
			domainAnalyzer.Add(typeof(Relation1));

			domainAnalyzer.Exclude(typeof(Relation1));

			domainAnalyzer.GetBaseImplementors(typeof(IRelation)).Single().Should().Be(typeof(MyRelation));
			domainAnalyzer.GetBaseImplementors(typeof(Relation1)).Single().Should().Be(typeof(MyRelation1));
		}
	}
}