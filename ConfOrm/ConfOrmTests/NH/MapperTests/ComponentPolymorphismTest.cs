using System;
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
	public class ComponentPolymorphismTest
	{
		private class Person
		{
			public int Id { get; set; }
			public Name Name { get; set; }
		}

		private class NameBase
		{
			public string First { get; set; }
			public string Last { get; set; }
		}

		private class Name : NameBase
		{
		}

		[Test]
		public void ShouldMapPropertiesOfSuperClasses()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.Is<Type>(t => t != typeof(Name)))).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => t != typeof(Name)))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.IsComponent(It.Is<Type>(t => t == typeof(Name)))).Returns(true);

			var mapper = new Mapper(orm.Object);
			var mapping = mapper.CompileMappingFor(new[] { typeof(Person) });

			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("Person"));
			rc.Properties.Should().Have.Count.EqualTo(1);
			rc.Properties.Select(p => p.Name).Should().Have.SameValuesAs("Name");
			var relation = rc.Properties.First(p => p.Name == "Name");
			relation.Should().Be.OfType<HbmComponent>();
			var component = (HbmComponent)relation;
			component.Properties.Should().Have.Count.EqualTo(2);
			component.Properties.Select(p => p.Name).Should().Have.SameValuesAs("First", "Last");
		}
	}
}