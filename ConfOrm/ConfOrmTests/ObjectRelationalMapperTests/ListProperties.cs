using System.Collections.Generic;
using ConfOrm;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.ObjectRelationalMapperTests
{
	public class ListProperties
	{
		private class A
		{
			public IList<string> List { get; set; }
			public IEnumerable<string> Bag { get; set; }
		}

		[Test]
		public void RecognizeListProperty()
		{
			var mapper = new ObjectRelationalMapper();
			var mi = typeof(A).GetProperty("List");
			mapper.IsList(mi).Should().Be.True();
		}

		[Test]
		public void NotRecognizeNoRegisteredAsListProperty()
		{
			var mapper = new ObjectRelationalMapper();
			var mi = typeof(A).GetProperty("Bag");
			mapper.IsList(mi).Should().Be.False();
		}

		[Test]
		public void RecognizeExplicitRegisteredAsListProperty()
		{
			var mapper = new ObjectRelationalMapper();
			mapper.List<A>(x => x.Bag);
			var mi = typeof(A).GetProperty("Bag");
			mapper.IsList(mi).Should().Be.True();
		}

		[Test]
		public void NotRecognizeExplicitRegisteredAsArrayProperty()
		{
			var mapper = new ObjectRelationalMapper();
			mapper.Array<A>(x => x.List);
			var mi = typeof(A).GetProperty("List");
			mapper.IsList(mi).Should().Be.False();
		}

		[Test]
		public void NotRecognizeExplicitRegisteredAsBagProperty()
		{
			var mapper = new ObjectRelationalMapper();
			mapper.Bag<A>(x => x.List);
			var mi = typeof(A).GetProperty("List");
			mapper.IsList(mi).Should().Be.False();
		}
	}
}