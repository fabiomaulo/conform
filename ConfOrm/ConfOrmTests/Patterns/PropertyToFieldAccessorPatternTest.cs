using System.Collections.Generic;
using System.Reflection;
using NHibernate.Mapping.ByCode;
using ConfOrm.Patterns;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns
{
	public class PropertyToFieldAccessorPatternTest
	{
		private class MyClass
		{
			private string aField;
			public int AProp { get; set; }

			private ICollection<int> withDifferentBackField;
			public IEnumerable<int> WithDifferentBackField
			{
				get { return withDifferentBackField; }
				set { withDifferentBackField = value as ICollection<int>; }
			}

			private string readOnlyWithSameBackField;
			public string ReadOnlyWithSameBackField
			{
				get { return readOnlyWithSameBackField; }
			}

			public string PropertyWithoutField
			{
				get { return ""; }
			}

			private string sameTypeOfBackField;
			public string SameTypeOfBackField
			{
				get { return sameTypeOfBackField; }
				set { sameTypeOfBackField = value; }
			}

			private int setOnlyProperty;
			public int SetOnlyProperty
			{
				set { setOnlyProperty = value; }
			}
		}

		[Test]
		public void WhenFieldNoMatch()
		{
			var pattern = new PropertyToFieldPattern();
			var member = typeof (MyClass).GetField("aField",
			                                       BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
			pattern.Match(member).Should().Be.False();
		}

		[Test]
		public void WhenAutoPropertyNoMatch()
		{
			var pattern = new PropertyToFieldPattern();
			var member = typeof(MyClass).GetProperty("AProp");
			pattern.Match(member).Should().Be.False();
		}

		[Test]
		public void WhenPropertyWithSameBackFieldNoMatch()
		{
			var pattern = new PropertyToFieldPattern();
			var member = typeof(MyClass).GetProperty("SameTypeOfBackField");
			pattern.Match(member).Should().Be.False();
		}

		[Test]
		public void WhenReadOnlyPropertyWithSameBackFieldNoMatch()
		{
			var pattern = new PropertyToFieldPattern();
			var member = typeof(MyClass).GetProperty("ReadOnlyWithSameBackField");
			pattern.Match(member).Should().Be.False();
		}

		[Test]
		public void WhenPropertyWithoutFieldNoMatch()
		{
			var pattern = new PropertyToFieldPattern();
			var member = typeof(MyClass).GetProperty("PropertyWithoutField");
			pattern.Match(member).Should().Be.False();
		}

		[Test]
		public void WhenPropertyWithDifferentBackFieldMatch()
		{
			var pattern = new PropertyToFieldPattern();
			var member = typeof(MyClass).GetProperty("WithDifferentBackField");
			pattern.Match(member).Should().Be.True();
		}

		[Test]
		public void WhenSetOnlyPropertyNoMatch()
		{
			var pattern = new PropertyToFieldPattern();
			var member = typeof(MyClass).GetProperty("SetOnlyProperty");
			pattern.Match(member).Should().Be.False();
		}

		[Test]
		public void ApplierAlwaysField()
		{
			var mapper = new Mock<IPropertyMapper>();
			var pattern = new MemberToFieldAccessorApplier<IPropertyMapper>();
			pattern.Apply(null, mapper.Object);
			mapper.Verify(x => x.Access(It.Is<Accessor>(a => a == Accessor.Field)));
		}
	}
}