using System.Linq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.PolymorphismRelationsTests
{
	public class SimpleCaseWithoutComplexHierarchyTest
	{
		private interface IRelation
		{
		}

		private class MyRelation : IRelation
		{
			public int Id { get; set; }
		}
		private class Relation1
		{
		}

		private class MyRelation1 : Relation1
		{
			public int Id { get; set; }
		}

		[Test]
		public void WhenAskForInterfaceThenGetFistEntityImplementingTheInterface()
		{
			var domainAnalyzer = new DomainAnalyzer();
			domainAnalyzer.Add(typeof(MyRelation));
			domainAnalyzer.Add(typeof(MyRelation1));
			domainAnalyzer.GetBaseImplementors(typeof(IRelation)).Single().Should().Be(typeof(MyRelation));
			domainAnalyzer.GetBaseImplementors(typeof(Relation1)).Single().Should().Be(typeof(MyRelation1));
		}
	}
}