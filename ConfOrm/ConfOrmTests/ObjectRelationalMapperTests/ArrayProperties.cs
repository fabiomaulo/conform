using System.Collections.Generic;
using ConfOrm;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.ObjectRelationalMapperTests
{
	public class ArrayProperties
	{
		private class A
		{
			public string[] Array { get; set; }
			public IEnumerable<string> Bag { get; set; }
		}

		[Test]
		public void RecognizeListProperty()
		{
			var mapper = new ObjectRelationalMapper();
			var mi = typeof(A).GetProperty("Array");
			mapper.IsArray(mi).Should().Be.True();
		}

		[Test]
		public void NotRecognizeNoRegisteredAsArrayProperty()
		{
			var mapper = new ObjectRelationalMapper();
			var mi = typeof(A).GetProperty("Bag");
			mapper.IsArray(mi).Should().Be.False();
		}

		[Test]
		public void RecognizeExplicitRegisteredAsArrayProperty()
		{
			var mapper = new ObjectRelationalMapper();
			mapper.Array<A>(x => x.Bag);
			var mi = typeof(A).GetProperty("Bag");
			mapper.IsArray(mi).Should().Be.True();
		}

		[Test]
		public void NotRecognizeExplicitRegisteredAsListProperty()
		{
			var mapper = new ObjectRelationalMapper();
			mapper.List<A>(x => x.Array);
			var mi = typeof(A).GetProperty("Array");
			mapper.IsArray(mi).Should().Be.False();
		}

		[Test]
		public void NotRecognizeExplicitRegisteredAsBagProperty()
		{
			var mapper = new ObjectRelationalMapper();
			mapper.Bag<A>(x => x.Array);
			var mi = typeof(A).GetProperty("Array");
			mapper.IsArray(mi).Should().Be.False();
		}
	}
}