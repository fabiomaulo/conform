using System;
using System.Linq;
using System.Reflection;
using ConfOrm;
using ConfOrm.NH;
using Iesi.Collections.Generic;
using Moq;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH.MapperTests
{
	public class SetOfElementsTest
	{
		private class Person
		{
			public int Id { get; set; }
			public ISet<string> NickNames { get; set; }
		}

		private Mock<IDomainInspector> GetMockedDomainInspector()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.Is<Type>(t=> t == typeof(Person)))).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => t == typeof(Person)))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.IsSet(It.Is<MemberInfo>(p => p == typeof(Person).GetProperty("NickNames")))).Returns(true);
			return orm;
		}

		private HbmMapping GetMapping(IDomainInspector domainInspector)
		{
			var mapper = new Mapper(domainInspector);
			return mapper.CompileMappingFor(new[] { typeof(Person) });
		}

		[Test, Ignore("Not supported yet")]
		public void MappingContainsClassWithComponent()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();

			var domainInspector = orm.Object;
			HbmMapping mapping = GetMapping(domainInspector);

			VerifyMapping(mapping);
		}

		private void VerifyMapping(HbmMapping mapping)
		{
			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("Person"));
			rc.Properties.Should().Have.Count.EqualTo(1);
			var relation = rc.Properties.First(p => p.Name == "NickNames");
			relation.Should().Be.OfType<HbmSet>();
			var collection = (HbmSet)relation;
			collection.ElementRelationship.Should().Be.OfType<HbmElement>();
			var elementRelation = (HbmElement) collection.ElementRelationship;
			elementRelation.Type.Should().Not.Be.Null();
			elementRelation.Type.name.Should().Be.EqualTo("string");
		}
	}
}