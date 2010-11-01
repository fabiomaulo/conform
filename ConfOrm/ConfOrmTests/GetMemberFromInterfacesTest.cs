using System;
using System.Linq;
using System.Reflection;
using ConfOrm;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests
{
	public class GetMemberFromInterfacesTest
	{
		private class BaseEntity
		{
			public int Id { get; set; }
		}

		private interface IEntity
		{
			bool IsValid { get; }
			string Something { get; set; }
		}

		private interface IHasSomething
		{
			string Something { get; set; }
		}

		private class Person : BaseEntity, IEntity, IHasSomething
		{
			private int someField;
			public string Name { get; set; }
			public bool IsValid { get { return false; } }
			public string Something { get; set; }
		}

		[Test]
		public void WhenNullArgumentThenThrows()
		{
			Executing.This(() => ((MemberInfo)null).GetPropertyFromInterfaces().ToList()).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void WhenNoInterfaceThenEmptyList()
		{
			ForClass<BaseEntity>.Property(x=> x.Id).GetPropertyFromInterfaces().Should().Be.Empty();
		}

		[Test]
		public void WhenFieldThenEmptyList()
		{
			ForClass<Person>.Field("someField").GetPropertyFromInterfaces().Should().Be.Empty();
		}

		[Test]
		public void WhenOneInterfaceThenReturnMemberInfoOfInterface()
		{
			var members = ForClass<Person>.Property(x => x.IsValid).GetPropertyFromInterfaces();
			members.Single().Should().Be(ForClass<IEntity>.Property(x=> x.IsValid));
		}

		[Test]
		public void WhenTwoInterfacesThenReturnMemberInfoOfEachInterface()
		{
			var members = ForClass<Person>.Property(x => x.Something).GetPropertyFromInterfaces();
			members.Should().Contain(ForClass<IEntity>.Property(x => x.Something));
			members.Should().Contain(ForClass<IHasSomething>.Property(x => x.Something));
		}
	}
}