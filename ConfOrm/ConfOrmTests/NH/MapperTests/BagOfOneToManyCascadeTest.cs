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
	public class BagOfOneToManyCascadeTest
	{
		private class Parent
		{
			public int Id { get; set; }
			public ICollection<Child> Children { get; set; }
		}

		private class Child
		{
			public int Id { get; set; }
			public Parent Owner { get; set; }
		}

		private Mock<IDomainInspector> GetMockedDomainInspector()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.Is<Type>(t => t == typeof(Parent) || t == typeof(Child)))).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => t == typeof(Parent) || t == typeof(Child)))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.IsOneToMany(It.Is<Type>(t => t == typeof(Parent)), It.Is<Type>(t => t == typeof(Child)))).Returns(true);
			orm.Setup(m => m.IsManyToOne(It.Is<Type>(t => t == typeof(Child)), It.Is<Type>(t => t == typeof(Parent)))).Returns(true);
			orm.Setup(m => m.ApplyCascade(It.Is<Type>(t => t == typeof(Parent)), It.IsAny<MemberInfo>(), It.Is<Type>(t => t == typeof(Child)))).Returns(CascadeOn.All | CascadeOn.DeleteOrphans);
			orm.Setup(m => m.IsBag(It.Is<MemberInfo>(p => p == typeof(Parent).GetProperty("Children")))).Returns(true);
			return orm;
		}

		private HbmMapping GetMapping(IDomainInspector domainInspector)
		{
			var mapper = new Mapper(domainInspector);
			return mapper.CompileMappingFor(new[] { typeof(Parent), typeof(Child) });
		}

		[Test]
		public void MappingThroughMock()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();

			var domainInspector = orm.Object;
			HbmMapping mapping = GetMapping(domainInspector);

			VerifyMapping(mapping);
		}

		private void VerifyMapping(HbmMapping mapping)
		{
			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("Parent"));
			rc.Properties.Should().Have.Count.EqualTo(1);
			var relation = rc.Properties.First(p => p.Name == "Children");
			relation.Should().Be.OfType<HbmBag>();
			var collection = (HbmBag)relation;
			collection.Satisfy(c => c.Inverse);
			collection.Cascade.Should().Contain("all").And.Contain("delete-orphan");
			collection.Key.Columns.First().name.Should().Be.EqualTo("Owner");
		}
	}
}