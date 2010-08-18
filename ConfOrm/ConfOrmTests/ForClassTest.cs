using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using ConfOrm;
using Iesi.Collections.Generic;
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
			private int PrivateProp { get; set; }
		}

		private class Movement<TDetail>
		{
			private ISet<TDetail> _details;
			public IEnumerable<TDetail> Details
			{
				get { return _details; }
			}

			private int PrivateProp { get; set; }
		}

		private class MovementDetail<TMovement>
		{
			public TMovement Movement { get; set; }
		}

		private class Income: Movement<IncomeDetail> {}

		private class IncomeDetail: MovementDetail<Income> {}

		[Test]
		public void WhenNullPropertyThenReturnNull()
		{
			ForClass<MyClass>.Property((Expression<Func<MyClass, object>>)null).Should().Be.Null();
			ForClass<MyClass>.Property((string)null).Should().Be.Null();
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

		[Test]
		public void WhenFieldIsDeclaredInBaseClassThenReturnMember()
		{
			ForClass<Income>.Field("_details").Should().Be(typeof(Movement<IncomeDetail>).GetField("_details", BindingFlags.Instance | BindingFlags.NonPublic));
		}

		[Test]
		public void WhenPrivatePropThenReturnMember()
		{
			ForClass<MyClass>.Property("PrivateProp").Should().Be(typeof(MyClass).GetProperty("PrivateProp", BindingFlags.Instance | BindingFlags.NonPublic));
		}

		[Test]
		public void WhenPrivatePropIsDeclaredInBaseClassThenReturnMember()
		{
			ForClass<Income>.Property("PrivateProp").Should().Be(typeof(Movement<IncomeDetail>).GetProperty("PrivateProp", BindingFlags.Instance | BindingFlags.NonPublic));
		}
	}
}