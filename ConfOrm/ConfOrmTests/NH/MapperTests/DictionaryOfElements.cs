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
	public class DictionaryOfElements
	{
		private class Person
		{
			public int Id { get; set; }
			public IDictionary<string, DateTime> Something { get; set; }
		}

		private Mock<IDomainInspector> GetMockedDomainInspector()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.Is<Type>(t => t == typeof(Person)))).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => t == typeof(Person)))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.IsDictionary(It.Is<MemberInfo>(p => p == typeof(Person).GetProperty("Something")))).Returns(true);
			return orm;
		}

		private HbmMapping GetMapping(IDomainInspector domainInspector)
		{
			var mapper = new Mapper(domainInspector);
			return mapper.CompileMappingFor(new[] { typeof(Person) });
		}

		[Test]
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
			var relation = rc.Properties.First(p => p.Name == "Something");
			relation.Should().Be.OfType<HbmMap>();
			var collection = (HbmMap)relation;
			collection.ElementRelationship.Should().Be.OfType<HbmElement>();
			var elementRelation = (HbmElement)collection.ElementRelationship;
			elementRelation.Type.Should().Not.Be.Null();
			elementRelation.Type.name.Should().Be.EqualTo("DateTime");
			collection.Item.Should().Not.Be.Null().And.Be.OfType<HbmMapKey>();
			var index = (HbmMapKey)collection.Item;
			index.Type.Should().Not.Be.Null();
			index.Type.name.Should().Be.EqualTo("String");
		}

		[Test]
		public void IntegrationWithObjectRelationalMapper()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Person>();
			HbmMapping mapping = GetMapping(orm);

			VerifyMapping(mapping);
		}
	}
}