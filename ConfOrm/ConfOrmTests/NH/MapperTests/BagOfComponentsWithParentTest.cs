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
	public class BagOfComponentsWithParentTest
	{
		private class Person
		{
			public int Id { get; set; }
			public ICollection<Address> Addresses { get; set; }
		}

		private class Address
		{
			public Person Owner { get; set; }
			public string Street { get; set; }
			public int Number { get; set; }
		}

		private Mock<IDomainInspector> GetMockedDomainInspector()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.Is<Type>(t => t == typeof(Person)))).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => t == typeof(Person)))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.IsComponent(It.Is<Type>(t => t == typeof(Address)))).Returns(true);
			orm.Setup(m => m.IsBag(It.Is<MemberInfo>(p => p == typeof(Person).GetProperty("Addresses")))).Returns(true);
			return orm;
		}

		private HbmMapping GetMapping(IDomainInspector domainInspector)
		{
			var mapper = new Mapper(domainInspector);
			return mapper.CompileMappingFor(new[] { typeof(Person) });
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
			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("Person"));
			var relation = rc.Properties.First(p => p.Name == "Addresses");
			var collection = (HbmBag)relation;
			collection.ElementRelationship.Should().Be.OfType<HbmCompositeElement>();
			var elementRelation = (HbmCompositeElement)collection.ElementRelationship;
			elementRelation.Class.Should().Contain("Address");
			elementRelation.Properties.Should().Have.Count.EqualTo(2);
			elementRelation.Properties.Select(p => p.Name).Should().Have.SameValuesAs("Street", "Number");
			elementRelation.Parent.Should().Not.Be.Null();
			elementRelation.Parent.name.Should().Be.EqualTo("Owner");
		}

	}
}