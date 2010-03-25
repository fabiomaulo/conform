using System.Reflection;
using ConfOrm;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests
{
	public class ForClassTest
	{
		private class MyClass
		{
			private int privateField;
			public int Prop { get; set; }
		}

		[Test]
		public void WhenNullPropertyThenReturnNull()
		{
			ForClass<MyClass>.Property(null).Should().Be.Null();
		}

		[Test]
		public void WhenPropertyThenReturnMember()
		{
			ForClass<MyClass>.Property(mc => mc.Prop).Should().Be(typeof(MyClass).GetProperty("Prop"));
		}

		[Test]
		public void WhenNullFieldNameThenReturnNull()
		{
			ForClass<MyClass>.Field(null).Should().Be.Null();
		}

		[Test]
		public void WhenFieldThenReturnMember()
		{
			ForClass<MyClass>.Field("privateField").Should().Be(typeof(MyClass).GetField("privateField",BindingFlags.Instance | BindingFlags.NonPublic));
		}
	}
}