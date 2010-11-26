using System.Reflection;
using ConfOrm;
using ConfOrm.Patterns;
using Iesi.Collections.Generic;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns.PolymorphismBidirectionalOneToManyTests.PatternTests
{
	public class ManyToManyTest
	{
		private interface IHasAddress { }

		private interface IHasPhone { }
		private class Contact : IHasPhone, IHasAddress
		{
			public int Id { get; set; }
			public ISet<Company> Companies { get; set; }
		}

		private interface IHasContact
		{
			ISet<Contact> Contacts { get; }
		}

		private class Company : IHasContact
		{
			public int Id { get; set; }
			public ISet<Contact> Contacts { get; set; }
			public Contact MainContact
			{
				get
				{
					return null; // logic to return main contact from collection
				}
			}
		}

		[Test]
		public void WhenManyToManyShouldNotMatch()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Contact>();
			orm.TablePerClass<Company>();
			orm.ManyToMany<Contact, Company>();

			var pattern = new PolymorphismBidirectionalOneToManyMemberPattern(orm);
			MemberInfo memberInfo = ForClass<Contact>.Property(x => x.Companies);
			pattern.Match(memberInfo).Should().Be.False();
		}
	}
}