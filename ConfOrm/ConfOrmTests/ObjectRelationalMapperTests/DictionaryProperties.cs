using System.Collections.Generic;
using System.Reflection;
using ConfOrm;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.ObjectRelationalMapperTests
{
	public class DictionaryProperties
	{
		private class A
		{
			public IDictionary<string, string> NickNames { get; set; }

			public ICollection<KeyValuePair<string, string>> Emails { get; set; }

			public ICollection<string> Others { get; set; }
		}

		[Test]
		public void RecognizeDictionaryProperty()
		{
			var mapper = new ObjectRelationalMapper();
			PropertyInfo mi = typeof(A).GetProperty("NickNames");
			mapper.IsDictionary(mi).Should().Be.True();
		}

		[Test]
		public void RecognizeExplicitRegisteredDictionaryProperty()
		{
			var mapper = new ObjectRelationalMapper();
			mapper.Dictionary<A>(x => x.Emails);
			PropertyInfo mi = typeof(A).GetProperty("Emails");
			mapper.IsDictionary(mi).Should().Be.True();
		}

		[Test]
		public void NoRecognizeNoDictionaryProperty()
		{
			var mapper = new ObjectRelationalMapper();
			PropertyInfo mi = typeof (A).GetProperty("Others");
			mapper.IsDictionary(mi).Should().Be.False();
		}
	}
}