using System.Collections.Generic;
using ConfOrm;
using Iesi.Collections.Generic;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.ObjectRelationalMapperTests
{
	public class BidirectionalRegistrationTests
	{
		private class A
		{
			public IList<B> List { get; set; }
			public ICollection<B> Bag { get; set; }
			public ISet<B> Set { get; set; }
			public IEnumerable<B> Generic { get; set; }
		}

		private class B
		{
			public A A { get; set; }
			public IEnumerable<A> Generic { get; set; }
		}

		[Test]
		public void DefinitionOfApi()
		{
			// only to try the API
			IObjectRelationalMapper orm = new Mock<IObjectRelationalMapper>().Object;
			orm.Bidirectional<A, B>(a=> a.Bag, b=> b.A);
			orm.Bidirectional<A, B>(a => a.List, b => b.A);
			orm.Bidirectional<A, B>(a => a.Set, b => b.A);
			orm.Bidirectional<A, B>(a => a.Generic, b => b.A);
			orm.Bidirectional<B, A>(a => a.Generic, b => b.Generic);
			orm.Bidirectional<B, A>(b => b.A, a => a.Generic);
		}
	}
}