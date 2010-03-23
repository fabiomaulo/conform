using System.Collections.Generic;
using ConfOrm;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.ObjectRelationalMapperTests
{
	public class BagProperties
	{
		private class A
		{
			public IList<string> List { get; set; }
			public IEnumerable<string> Set { get; set; }
		}

		private class B: A
		{
			
		}

		[Test]
		public void RecognizeBagProperty()
		{
			var mapper = new ObjectRelationalMapper();
			var mi = typeof(A).GetProperty("List");
			mapper.IsBag(mi).Should().Be.True();
		}

		[Test]
		public void NotRecognizeExplicitRegisteredAsListProperty()
		{
			var mapper = new ObjectRelationalMapper();
			mapper.List<A>(x => x.List);
			var mi = typeof(A).GetProperty("List");
			mapper.IsBag(mi).Should().Be.False();
		}

		[Test]
		public void NotRecognizeExplicitRegisteredAsArrayProperty()
		{
			var mapper = new ObjectRelationalMapper();
			mapper.Array<A>(x => x.List);
			var mi = typeof(A).GetProperty("List");
			mapper.IsBag(mi).Should().Be.False();
		}

		[Test]
		public void NotRecognizeExplicitRegisteredAsSetProperty()
		{
			var mapper = new ObjectRelationalMapper();
			mapper.Set<A>(x => x.Set);
			var mi = typeof(A).GetProperty("Set");
			mapper.IsBag(mi).Should().Be.False();
		}

		[Test]
		public void WhenInBaseClassThenRecognizeBagProperty()
		{
			var mapper = new ObjectRelationalMapper();
			var mi = typeof(B).GetProperty("List");
			mapper.IsBag(mi).Should().Be.True();
		}

		[Test]
		public void WhenInBaseClassThenNotRecognizeExplicitRegisteredAsSetProperty()
		{
			var mapper = new ObjectRelationalMapper();
			mapper.Set<A>(x => x.Set);
			var mi = typeof(B).GetProperty("Set");
			mapper.IsBag(mi).Should().Be.False();
		}
	}
}