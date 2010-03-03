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

		private class UserMap
		{
			public int Id { get; set; }
			public IDictionary<string, RoleMap> RolesMap { get; set; }
		}

		private class RoleMap
		{
			public int Id { get; set; }
			public IDictionary<UserMap, string> UsersMap { get; set; }
		}

		private class UserMix
		{
			public int Id { get; set; }
			public IDictionary<string, RoleMix> RolesMap { get; set; }
		}

		private class RoleMix
		{
			public int Id { get; set; }
			public IEnumerable<UserMix> Users { get; set; }
		}

		private class Person
		{
			public int Id { get; set; }
			public IEnumerable<Person> Friends { get; set; }
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
			orm.Setup(m => m.IsManyToMany(It.Is<Type>(t => t == typeof(UserMap)), It.Is<Type>(t => t == typeof(RoleMap)))).Returns(true);
			orm.Setup(m => m.IsManyToMany(It.Is<Type>(t => t == typeof(UserMix)), It.Is<Type>(t => t == typeof(RoleMix)))).Returns(true);

			orm.Setup(m => m.IsBag(It.Is<MemberInfo>(p => p == typeof(User).GetProperty("Roles")))).Returns(true);
			orm.Setup(m => m.IsBag(It.Is<MemberInfo>(p => p == typeof(Role).GetProperty("Users")))).Returns(true);

			orm.Setup(m => m.IsDictionary(It.Is<MemberInfo>(p => p == typeof(UserMap).GetProperty("RolesMap")))).Returns(true);
			orm.Setup(m => m.IsDictionary(It.Is<MemberInfo>(p => p == typeof(RoleMap).GetProperty("UsersMap")))).Returns(true);


			orm.Setup(m => m.IsDictionary(It.Is<MemberInfo>(p => p == typeof(UserMix).GetProperty("RolesMap")))).Returns(true);
			orm.Setup(m => m.IsBag(It.Is<MemberInfo>(p => p == typeof(RoleMix).GetProperty("Users")))).Returns(true);

			orm.Setup(m => m.IsBag(It.Is<MemberInfo>(p => p == typeof(Person).GetProperty("Friends")))).Returns(true);

			return orm;
		}

		[Test]
		public void WhenPlainCollectionShouldUseSameTable()
		{
			var orm = GetMockedDomainInspector();
			var mapper = new Mapper(orm.Object);
			var mappings = mapper.CompileMappingFor(new[] { typeof(Role), typeof(User) });
			var hbmUserRoles = mappings.RootClasses.First(rc => rc.Name.Contains("User")).Properties.OfType<HbmBag>().Single();
			var hbmRoleUsers = mappings.RootClasses.First(rc => rc.Name.Contains("Role")).Properties.OfType<HbmBag>().Single();
			hbmUserRoles.Table.Should().Not.Be.Null().And.Not.Be.Empty().And.Be.EqualTo(hbmRoleUsers.Table);
		}

		[Test]
		public void WhenDictionariesShouldUseSameTable()
		{
			var orm = GetMockedDomainInspector();
			var mapper = new Mapper(orm.Object);
			var mappings = mapper.CompileMappingFor(new[] { typeof(RoleMap), typeof(UserMap) });
			var hbmUserRoles = mappings.RootClasses.First(rc => rc.Name.Contains("UserMap")).Properties.OfType<HbmMap>().Single();
			var hbmRoleUsers = mappings.RootClasses.First(rc => rc.Name.Contains("RoleMap")).Properties.OfType<HbmMap>().Single();
			hbmUserRoles.Table.Should().Not.Be.Null().And.Not.Be.Empty().And.Be.EqualTo(hbmRoleUsers.Table);
		}

		[Test]
		public void WhenMixShouldUseSameTable()
		{
			var orm = GetMockedDomainInspector();
			var mapper = new Mapper(orm.Object);
			var mappings = mapper.CompileMappingFor(new[] { typeof(RoleMix), typeof(UserMix) });
			var hbmUserRoles = mappings.RootClasses.First(rc => rc.Name.Contains("UserMix")).Properties.OfType<HbmMap>().Single();
			var hbmRoleUsers = mappings.RootClasses.First(rc => rc.Name.Contains("RoleMix")).Properties.OfType<HbmBag>().Single();
			hbmUserRoles.Table.Should().Not.Be.Null().And.Not.Be.Empty().And.Be.EqualTo(hbmRoleUsers.Table);
		}

		[Test]
		public void WhenCircularReferenceShouldNotMatch()
		{
			var orm = GetMockedDomainInspector();
			var mapper = new Mapper(orm.Object);
			var mappings = mapper.CompileMappingFor(new[] { typeof(Person) });
			var hbmPersonFriends = mappings.RootClasses.First(rc => rc.Name.Contains("Person")).Properties.OfType<HbmBag>().Single();
			hbmPersonFriends.Table.Should().Be.Null();
		}
	}
}