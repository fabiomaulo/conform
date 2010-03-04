using System;
using System.Linq;
using System.Reflection;
using ConfOrm;
using ConfOrm.NH;
using Moq;
using NHibernate;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH.MapperTests
{
	public class CustomPropertyPatternApplierTest
	{
		private class MyClass
		{
			public int Id { get; set; }
			public DateTime Date { get; set; }
			public DateTime DateOfMeeting { get; set; }
			public DateTime MeetingDate { get; set; }
			public DateTime StartAt { get; set; }
		}

		[Test]
		public void AddCustomDelegatedApplier()
		{
			var orm = new Mock<IDomainInspector>();
			var mapper = new Mapper(orm.Object);
			var previousPropertyApplierCount = mapper.PatternsAppliers.Property.Count;

			mapper.AddPropertyPattern(mi => mi.Name.StartsWith("Date") || mi.Name.EndsWith("Date"),
			                          pm => pm.Type(NHibernateUtil.Date));

			mapper.PatternsAppliers.Property.Count.Should().Be(previousPropertyApplierCount + 1);
		}

		[Test]
		public void ExecuteCustomDelegatedApplier()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);

			var mapper = new Mapper(orm.Object);
			mapper.AddPropertyPattern(mi => mi.GetPropertyOrFieldType() == typeof(DateTime) && (mi.Name.StartsWith("Date") || mi.Name.EndsWith("Date")),
			                          pm => pm.Type(NHibernateUtil.Date));
			var mapping = mapper.CompileMappingFor(new[] {typeof (MyClass)});

			var hbmClass = mapping.RootClasses.Single();
			
			var hbmProp = (HbmProperty) hbmClass.Properties.First(p => p.Name == "Date");
			hbmProp.Type.name.Should().Be.EqualTo("Date");

			hbmProp = (HbmProperty)hbmClass.Properties.First(p => p.Name == "DateOfMeeting");
			hbmProp.Type.name.Should().Be.EqualTo("Date");

			hbmProp = (HbmProperty)hbmClass.Properties.First(p => p.Name == "MeetingDate");
			hbmProp.Type.name.Should().Be.EqualTo("Date");

			hbmProp = (HbmProperty)hbmClass.Properties.First(p => p.Name == "StartAt");
			hbmProp.Type.Should().Be.Null();
		}
	}
}