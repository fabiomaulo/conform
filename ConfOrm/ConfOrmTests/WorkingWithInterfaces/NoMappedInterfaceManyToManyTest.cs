using System.Collections.Generic;
using ConfOrm;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.WorkingWithInterfaces
{
	public class NoMappedInterfaceManyToManyTest
	{
		private interface ISecurity
		{
			bool IsUser();
		}

		private class Contact : ISecurity
		{
			public int Id { get; set; }
			public bool IsUser()
			{
				return false;
			}
		}

		private class UserGroup
		{
			public int Id { get; set; }
			public IEnumerable<ISecurity> Securities { get; set; }
		}

		[Test]
		public void WhenFindInterfaceForRootClassInCollectionThenRecognizeRelation()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Contact>();
			orm.TablePerClass<UserGroup>();
			orm.ManyToMany<UserGroup, ISecurity>();

			orm.IsManyToMany(typeof(ISecurity), typeof(UserGroup)).Should().Be.True();
		}
	}
}