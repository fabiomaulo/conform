using System.Collections.Generic;
using ConfOrm;
using ConfOrm.Patterns;
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

		private class B : A
		{

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
			var mi = typeof(A).GetProperty("NickNames");
			mapper.IsSet(mi).Should().Be.True();
		}

		[Test]
		public void NoRecognizeNoSetProperty()
		{
			var mapper = new ObjectRelationalMapper();
			var mi = typeof(A).GetProperty("Others");
			mapper.IsSet(mi).Should().Be.False();
		}

		[Test]
		public void WhenInBaseClassThenRecognizeSetProperty()
		{
			var mapper = new ObjectRelationalMapper();
			var mi = typeof(B).GetProperty("Set");
			mapper.IsSet(mi).Should().Be.True();
		}

		[Test]
		public void WhenInBaseClassThenNoRecognizeNoSetProperty()
		{
			var mapper = new ObjectRelationalMapper();
			var mi = typeof(A).GetProperty("Others");
			mapper.IsSet(mi).Should().Be.False();
		}

		[Test]
		public void WhenExplicitlyDeclaredAsBagThenDoesNotUseSet()
		{
			var mapper = new ObjectRelationalMapper();
			mapper.Patterns.Sets.Add(new BagCollectionPattern());
			mapper.Bag<A>(a=> a.Others);

			mapper.IsBag(ForClass<A>.Property(a=> a.Others)).Should().Be.True();
			mapper.IsSet(ForClass<A>.Property(a => a.NickNames)).Should().Be.True();
			mapper.IsSet(ForClass<A>.Property(a => a.Set)).Should().Be.True();
		}
	}
}