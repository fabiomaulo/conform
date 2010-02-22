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
	public class OneToManyOnDeleteConstraintWhenCyclicTest
	{
		private class Node
		{
			public int Id { get; set; }
			public Node Parent { get; set; }
			public ICollection<Node> Children { get; set; }
		}

		private Mock<IDomainInspector> GetMockedDomainInspector()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.IsOneToMany(It.Is<Type>(t => t == typeof(Node)), It.Is<Type>(t => t == typeof(Node)))).Returns(true);
			orm.Setup(m => m.IsManyToOne(It.Is<Type>(t => t == typeof(Node)), It.Is<Type>(t => t == typeof(Node)))).Returns(true);
			orm.Setup(m => m.IsBag(It.Is<MemberInfo>(p => p == typeof(Node).GetProperty("Children")))).Returns(true);
			return orm;
		}

		private HbmMapping GetMapping(IDomainInspector domainInspector)
		{
			var mapper = new Mapper(domainInspector);
			return mapper.CompileMappingFor(new[] { typeof(Node)});
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
			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("Node"));
			var relation = rc.Properties.First(p => p.Name == "Children");
			relation.Should().Be.OfType<HbmBag>();
			var collection = (HbmBag)relation;
			collection.Key.ondelete.Should().Be.EqualTo(HbmOndelete.Noaction);
		}

		[Test]
		public void IntegrationWithObjectRelationalMapper()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Node>();
			HbmMapping mapping = GetMapping(orm);

			VerifyMapping(mapping);
		}
	}
}