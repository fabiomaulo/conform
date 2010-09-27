using System;
using System.Collections.Generic;
using ConfOrm;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.ObjectRelationalMapperTests
{
	public class DoubleRelationKindManyToManyOneToManyTest
	{
		public class Group
		{
			public virtual Guid Id { get; set; }
			public virtual string Name { get; set; }
			public virtual User Owner { get; set; }
			public virtual IList<User> Members { get; set; }
		}

		public class User
		{
			public virtual Guid Id { get; set; }
			public virtual string FirstName { get; set; }
			public virtual string LastName { get; set; }
		}

		[Test(Description = "CfgORM-5"), Ignore("Not fixed yet.")]
		public void WhenExplicitManyToManyThenShouldMapSimpleRelationAsManyToOneByDefault()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<User>();
			orm.TablePerClass<Group>();
			orm.ManyToMany<Group, User>();

			orm.IsManyToOne(typeof(Group), typeof(User)).Should().Be.True();
			orm.IsManyToMany(typeof(Group), typeof(User)).Should().Be.True();
			orm.IsOneToMany(typeof(Group), typeof(User)).Should().Be.False();
			orm.IsOneToOne(typeof(Group), typeof(User)).Should().Be.False();
		}
	}
}