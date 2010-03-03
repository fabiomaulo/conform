using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConfOrm;
using ConfOrm.NH;
using Moq;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH.MapperTests
{
	public class BidirectionalManyToMany
	{
		private class User
		{
			public int Id { get; set; }
			public IEnumerable<Role> Roles { get; set; }			
		}

		private class Role
		{
			public int Id { get; set; }
			public IEnumerable<User> Users { get; set; }
		}
		private Mock<IDomainInspector> GetMockedDomainInspector()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.IsManyToMany(It.Is<Type>(t => t == typeof(User)), It.Is<Type>(t => t == typeof(Role)))).Returns(true);
			orm.Setup(m => m.IsBag(It.Is<MemberInfo>(p => p == typeof(User).GetProperty("Roles")))).Returns(true);
			orm.Setup(m => m.IsBag(It.Is<MemberInfo>(p => p == typeof(Role).GetProperty("Users")))).Returns(true);
			return orm;
		}

		[Test, Ignore("Not fixed yet.")]
		public void ShouldUseSameTable()
		{
			var orm = GetMockedDomainInspector();
			var mapper = new Mapper(orm.Object);
			var mappings = mapper.CompileMappingFor(new[] { typeof(Role), typeof(User) });
			var hbmUserRoles = mappings.RootClasses.First(rc => rc.Name.Contains("User")).Properties.OfType<HbmBag>().Single();
			var hbmRoleUsers = mappings.RootClasses.First(rc => rc.Name.Contains("Role")).Properties.OfType<HbmBag>().Single();
			hbmUserRoles.Table.Should().Not.Be.Null().And.Not.Be.Empty().And.Be.EqualTo(hbmRoleUsers.Table);
		}
	}
}