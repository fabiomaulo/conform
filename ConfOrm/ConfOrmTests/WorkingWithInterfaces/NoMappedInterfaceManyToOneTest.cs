using System;
using System.Collections.Generic;
using ConfOrm;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.WorkingWithInterfaces
{
	public class NoMappedInterfaceManyToOneTest
	{
		private interface ISecurity
		{
			bool IsUser();
		}

		private class Contact: ISecurity
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
			public ISecurity Security { get; set; }
		}

		private class UserSuperGroup
		{
			public int Id { get; set; }
			public IEnumerable<ISecurity> Securities { get; set; }
		}

		[Test]
		public void WhenFindInterfaceForRootClassThenRecognizeRelation()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Contact>();
			orm.TablePerClass<UserGroup>();
			orm.ManyToOne<UserGroup, ISecurity>();

			orm.IsManyToOne(typeof (UserGroup), typeof (ISecurity)).Should().Be.True();
		}

		[Test]
		public void WhenFindInterfaceForRootClassInCollectionThenRecognizeRelation()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Contact>();
			orm.TablePerClass<UserGroup>();
			orm.ManyToOne<UserSuperGroup, ISecurity>();

			orm.IsOneToMany(typeof(ISecurity), typeof(UserSuperGroup)).Should().Be.True();
		}
	}
}