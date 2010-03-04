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
	public class ComponentDoublePropertyTest
	{
		private class Person
		{
			public int Id { get; set; }
			public Name RealName { get; set; }
			public Name Aka { get; set; }
		}

		private class Name
		{
			public string First { get; set; }
			public string Last { get; set; }
			public Relation SomeRelation { get; set; }
		}

		private class Relation
		{
			public int Id { get; set; }			
		}

		private Mock<IDomainInspector> GetMockedDomainInspector()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.Is<Type>(t => t != typeof(Name)))).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => t != typeof(Name)))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.IsComponent(It.Is<Type>(t => t == typeof(Name)))).Returns(true);
			orm.Setup(m => m.IsManyToOne(It.Is<Type>(t => t == typeof(Name)), It.Is<Type>(t => t == typeof(Relation)))).Returns(true);
			return orm;
		}


		[Test]
		public void WhenNotSpecifiedThenComposeColumnNameByDefault()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();

			var domainInspector = orm.Object;
			var mapper = new Mapper(domainInspector);
			HbmMapping mapping = mapper.CompileMappingFor(new[] { typeof(Person) });

			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("Person"));
			var hbmRealName = (HbmComponent)rc.Properties.First(p => p.Name == "RealName");
			var hbmAka = (HbmComponent)rc.Properties.First(p => p.Name == "Aka");
			hbmRealName.Properties.OfType<HbmProperty>().Select(p => p.Columns.Single().name).Should().Have.SameValuesAs("RealNameFirst", "RealNameLast");
			hbmAka.Properties.OfType<HbmProperty>().Select(p => p.Columns.Single().name).Should().Have.SameValuesAs("AkaFirst", "AkaLast");
		}

		[Test, Ignore("Not fixed yet")]
		public void WhenNotSpecifiedThenComposeColumnNameByDefaultForManyToOne()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();

			var domainInspector = orm.Object;
			var mapper = new Mapper(domainInspector);
			HbmMapping mapping = mapper.CompileMappingFor(new[] { typeof(Person) });

			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("Person"));
			var hbmRealName = (HbmComponent)rc.Properties.First(p => p.Name == "RealName");
			var hbmAka = (HbmComponent)rc.Properties.First(p => p.Name == "Aka");
			hbmRealName.Properties.OfType<HbmManyToOne>().Select(p => p.Columns.Single().name).Single().Should().Be("RealNameSomeRelation");
			hbmAka.Properties.OfType<HbmManyToOne>().Select(p => p.Columns.Single().name).Single().Should().Be("AkaSomeRelation");
		}

		[Test]
		public void WhenSpecifiedThenSetDifferentConfigurationForSameComponent()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();

			var domainInspector = orm.Object;
			var mapper = new Mapper(domainInspector);
			mapper.Class<Person>(c =>
				{
					c.Component(p => p.RealName, cname =>
						{
							cname.Property(cnp => cnp.First, pm => pm.Length(50));
							cname.Property(cnp => cnp.Last, pm => pm.Length(51));
						});
					c.Component(p => p.Aka, cname =>
					{
						cname.Property(cnp => cnp.First, pm => pm.Length(20));
						cname.Property(cnp => cnp.Last, pm => pm.Length(21));
					});
				});
			HbmMapping mapping = mapper.CompileMappingFor(new[] { typeof(Person) });

			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("Person"));
			var hbmRealName = (HbmComponent)rc.Properties.First(p => p.Name == "RealName");
			var hbmAka = (HbmComponent)rc.Properties.First(p => p.Name == "Aka");
			hbmRealName.Properties.OfType<HbmProperty>().Select(p => p.length).Should().Have.SameValuesAs("50", "51");
			hbmAka.Properties.OfType<HbmProperty>().Select(p => p.length).Should().Have.SameValuesAs("20", "21");
		}

		[Test]
		public void WhenCustomizedAndSpecifiedThenSetDifferentConfigurationForSameComponent()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();

			var domainInspector = orm.Object;
			var mapper = new Mapper(domainInspector);
			mapper.Customize<Name>(name =>
				{
					name.Property(np => np.First, pm => pm.Length(20));
					name.Property(np => np.Last, pm => pm.Length(35));
				});

			mapper.Class<Person>(c =>
				{
					c.Component(p => p.RealName, cname => cname.Property(cnp => cnp.Last, pm => pm.Length(50)));
					c.Component(p => p.Aka, cname => cname.Property(cnp => cnp.First, pm => pm.Length(50)));
				});

			HbmMapping mapping = mapper.CompileMappingFor(new[] { typeof(Person) });

			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("Person"));
			var hbmRealName = (HbmComponent)rc.Properties.First(p => p.Name == "RealName");
			var hbmAka = (HbmComponent)rc.Properties.First(p => p.Name == "Aka");
			hbmRealName.Properties.OfType<HbmProperty>().Select(p => p.length).Should().Have.SameValuesAs("20", "50");
			hbmAka.Properties.OfType<HbmProperty>().Select(p => p.length).Should().Have.SameValuesAs("50", "35");
		}

	}
}