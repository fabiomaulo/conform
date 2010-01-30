using System.Collections.Generic;
using ConfOrm;
using Iesi.Collections.Generic;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.ObjectRelationalMapperTests
{
	public class SetProperties
	{
		private class A
		{
			public ISet<string> Set { get; set; }
			public IEnumerable<string> NickNames { get; set; }
			public IEnumerable<string> Others { get; set; }
		}

		[Test]
		public void RecognizeSetProperty()
		{
			var mapper = new ObjectRelationalMapper();
			var mi = typeof(A).GetProperty("Set");
			mapper.IsSet(mi).Should().Be.True();
		}

		[Test]
		public void RecognizeExplicitRegisteredSetProperty()
		{
			var mapper = new ObjectRelationalMapper();
			mapper.Set<A>(x => x.NickNames);
			var mi = typeof(A).GetProperty("Set");
			mapper.IsSet(mi).Should().Be.True();
		}

		[Test]
		public void NoRecognizeNoSetProperty()
		{
			var mapper = new ObjectRelationalMapper();
			var mi = typeof(A).GetProperty("Others");
			mapper.IsSet(mi).Should().Be.False();
		}
	}
}