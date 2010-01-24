using ConfOrm;
using ConfOrm.Patterns;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns
{
	public class BidirectionalRelationPatternTest
	{
		private class AEntity
		{
			public BEntity BEntity { get; set; }
			public CEntity CEntity { get; set; }
		}
		private class AInherited : AEntity {}

		private class BEntity
		{
			public AEntity AEntity { get; set; }
		}

		private class CEntity {}

		[Test]
		public void BidiretionalSimple()
		{
			var p = new BidirectionalRelationPattern();
			p.Match(new Relation(typeof(AEntity), typeof(BEntity))).Should().Be.True();
			p.Match(new Relation(typeof(BEntity), typeof(AEntity))).Should().Be.True();
		}

		[Test]
		public void BidiretionalInherited()
		{
			var p = new BidirectionalRelationPattern();
			p.Match(new Relation(typeof(AInherited), typeof(BEntity))).Should().Be.True();
			p.Match(new Relation(typeof(BEntity), typeof(AInherited))).Should().Be.True();
		}

		[Test]
		public void NoBidiretional()
		{
			var p = new BidirectionalRelationPattern();
			p.Match(new Relation(typeof(AEntity), typeof(CEntity))).Should().Be.False();
			p.Match(new Relation(typeof(BEntity), typeof(CEntity))).Should().Be.False();
		}
	}
}