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
	public class PropertyAccessorTest
	{
		private class MyClass
		{
			public int Id { get; set; }
			private string readOnlyWithField;
			public string ReadOnlyWithField
			{
				get { return readOnlyWithField; }
			}
		}

		private HbmMapping GetMapping(IDomainInspector domainInspector)
		{
			var mapper = new Mapper(domainInspector);
			return mapper.CompileMappingFor(new[] { typeof(MyClass)});
		}

		[Test]
		public void WhenReadOnlyPersistentPropertyWithBackfieldThenUseAccessField()
		{
			// by default, if the property does not have setter it should use field
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.Is<Type>(t => t == typeof(MyClass)))).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => t == typeof(MyClass)))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);

			var domainInspector = orm.Object;
			var mapper = new Mapper(domainInspector);
			var mapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });

			HbmClass rc = mapping.RootClasses.Single();
			rc.Properties.First(p => p.Name == "ReadOnlyWithField").Access.Should().Be.EqualTo("nosetter.camelcase");
		}

		[Test]
		public void WhenExplicitFieldAccessThenUseAccessField()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.Is<Type>(t => t == typeof(MyClass)))).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => t == typeof(MyClass)))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.PersistentPropertyAccessStrategy(It.Is<MemberInfo>(mi => mi.Name == "ReadOnlyWithField"))).Returns(StateAccessStrategy.Field);

			var domainInspector = orm.Object;
			var mapper = new Mapper(domainInspector);
			var mapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });

			HbmClass rc = mapping.RootClasses.Single();
			rc.Properties.First(p => p.Name == "ReadOnlyWithField").Access.Should().Be.EqualTo("field.camelcase");
		}

		[Test]
		public void WhenExplicitReadOnlyAccessThenUseAccessReadOnly()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.Is<Type>(t => t == typeof(MyClass)))).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => t == typeof(MyClass)))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.PersistentPropertyAccessStrategy(It.Is<MemberInfo>(mi => mi.Name == "ReadOnlyWithField"))).Returns(StateAccessStrategy.ReadOnlyProperty);

			var domainInspector = orm.Object;
			var mapper = new Mapper(domainInspector);
			var mapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });

			HbmClass rc = mapping.RootClasses.Single();
			rc.Properties.First(p => p.Name == "ReadOnlyWithField").Access.Should().Be.EqualTo("readonly");
		}
	}
}