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
using ConfOrm.Mappers;

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
		private class MyOtherClass
		{
			public int Id { get; set; }
			public int AProp { get; set; }

			private ICollection<int> withDifferentBackField;
			public IEnumerable<int> WithDifferentBackField
			{
				get { return withDifferentBackField; }
			}

			private string readOnlyWithSameBackField;
			public string ReadOnlyWithSameBackField
			{
				get { return readOnlyWithSameBackField; }
			}

			private string sameTypeOfBackField;
			public string SameTypeOfBackField
			{
				get { return sameTypeOfBackField; }
				set { sameTypeOfBackField = value; }
			}

			public string PropertyWithoutField
			{
				get { return ""; }
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
			Mapper mapper = MyClassScenario();
			var mapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });

			HbmClass rc = mapping.RootClasses.Single();
			rc.Properties.First(p => p.Name == "ReadOnlyWithField").Access.Should().Be.EqualTo("nosetter.camelcase");
		}

		private Mapper MyClassScenario()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.Is<Type>(t => t == typeof (MyClass)))).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => t == typeof (MyClass)))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);

			var domainInspector = orm.Object;
			return new Mapper(domainInspector);
		}

		[Test]
		public void WhenExplicitFieldAccessThenUseAccessField()
		{
			var mapper = MyClassScenario();
			mapper.Property<MyClass>(mc => mc.ReadOnlyWithField, pm => pm.Access(Accessor.Field));
			var mapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });

			HbmClass rc = mapping.RootClasses.Single();
			rc.Properties.First(p => p.Name == "ReadOnlyWithField").Access.Should().Be.EqualTo("field.camelcase");
		}

		[Test]
		public void WhenExplicitReadOnlyAccessThenUseAccessReadOnly()
		{
			var mapper = MyClassScenario();
			mapper.Property<MyClass>(mc => mc.ReadOnlyWithField, pm => pm.Access(Accessor.ReadOnly));
			var mapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });

			HbmClass rc = mapping.RootClasses.Single();
			rc.Properties.First(p => p.Name == "ReadOnlyWithField").Access.Should().Be.EqualTo("readonly");
		}

		private Mapper MyOtherClassScenario()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.Is<Type>(t => t == typeof(MyOtherClass)))).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => t == typeof(MyOtherClass)))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);

			var domainInspector = orm.Object;
			return new Mapper(domainInspector);
		}

		[Test]
		public void WhenAutoPropertyThenProperty()
		{
			var mapper = MyOtherClassScenario();
			var mapping = mapper.CompileMappingFor(new[] { typeof(MyOtherClass) });

			HbmClass rc = mapping.RootClasses.Single();
			rc.Properties.First(p => p.Name == "AProp").Access.Should().Be.Null();
		}

		[Test]
		public void WhenDifferentPropertyTypeThenField()
		{
			var mapper = MyOtherClassScenario();
			var mapping = mapper.CompileMappingFor(new[] { typeof(MyOtherClass) });

			HbmClass rc = mapping.RootClasses.Single();
			rc.Properties.First(p => p.Name == "WithDifferentBackField").Access.Should().Be.EqualTo("field.camelcase");
		}

		[Test]
		public void WhenNosetterPropertyWithFieldThenFieldOnSet()
		{
			var mapper = MyOtherClassScenario();
			var mapping = mapper.CompileMappingFor(new[] { typeof(MyOtherClass) });

			HbmClass rc = mapping.RootClasses.Single();
			rc.Properties.First(p => p.Name == "ReadOnlyWithSameBackField").Access.Should().Be.EqualTo("nosetter.camelcase");
		}

		[Test]
		public void WhenNosetterPropertyWithoutFieldThenReadOnly()
		{
			var mapper = MyOtherClassScenario();
			var mapping = mapper.CompileMappingFor(new[] { typeof(MyOtherClass) });

			HbmClass rc = mapping.RootClasses.Single();
			rc.Properties.First(p => p.Name == "PropertyWithoutField").Access.Should().Be.EqualTo("readonly");
		}
	}
}