using System.Reflection;
using ConfOrm.Patterns;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns
{
	public class ReadOnlyPropertyPatternTest
	{
		public class MyEntity
		{
			private string noReadOnlyWithField;
			private string pizza;

			public string ReadOnly
			{
				get { return ""; }
			}

			public string NoReadOnlyWithField
			{
				get { return noReadOnlyWithField; }
			}

			public string NoReadOnly
			{
				get { return ""; }
				set { }
			}

			public string WriteOnly
			{
				set { }
			}
		}

		[Test]
		public void RecognizeReadOnly()
		{
			var pattern = new ReadOnlyPropertyPattern();
			PropertyInfo pi = typeof(MyEntity).GetProperty("ReadOnly");
			pattern.Match(pi).Should().Be(true);
		}

		[Test]
		public void DiscardReadOnlyWithField()
		{
			var pattern = new ReadOnlyPropertyPattern();
			PropertyInfo pi = typeof(MyEntity).GetProperty("NoReadOnlyWithField");
			pattern.Match(pi).Should().Be.False();
		}

		[Test]
		public void DiscardNoReadOnly()
		{
			var pattern = new ReadOnlyPropertyPattern();
			PropertyInfo pi = typeof(MyEntity).GetProperty("NoReadOnly");
			Assert.That(!pattern.Match(pi));
			pi = typeof(MyEntity).GetProperty("WriteOnly");
			pattern.Match(pi).Should().Be.False();
		}

		[Test]
		public void DiscardFields()
		{
			var pattern = new ReadOnlyPropertyPattern();
			var pi = typeof(MyEntity).GetField("pizza", BindingFlags.Instance | BindingFlags.NonPublic);
			pattern.Match(pi).Should().Be.False();
		}
	}
}