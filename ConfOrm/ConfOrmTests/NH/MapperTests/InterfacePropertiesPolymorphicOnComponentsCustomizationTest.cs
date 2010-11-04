using System.Collections.Generic;
using System.Linq;
using ConfOrm;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH.MapperTests
{
	public class InterfacePropertiesPolymorphicOnComponentsCustomizationTest
	{
		private interface IHasMessage
		{
			string Message { get; set; }
		}

		private class MyEntity
		{
			public int Id { get; set; }
			public MyComponent Component { get; set; }
		}

		private class MyComponent : IHasMessage
		{
			public string Message { get; set; }
			public MyNestedComponent NestedComponent { get; set; }
		}

		private class MyNestedComponent : IHasMessage
		{
			public string Message { get; set; }
		}

		private class MyEntityWithCollection
		{
			public int Id { get; set; }
			public ICollection<MyComponent> Components { get; set; }
		}

		[Test]
		public void WhenCustomizePropertyOnInterfaceThenApplyCustomizationOnImplementations()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<MyEntity>();

			var mapper = new Mapper(orm);
			mapper.Customize<IHasMessage>(x => x.Property(hasMessage => hasMessage.Message, pm => pm.Length(123)));

			var mappings = mapper.CompileMappingFor(new[] { typeof(MyEntity)});

			HbmClass hbmMyEntity = mappings.RootClasses.Single(hbm => hbm.Name == "MyEntity");
			var hbmComponent = hbmMyEntity.Properties.OfType<HbmComponent>().Where(p => p.Name == "Component").Single();
			hbmComponent.Properties.OfType<HbmProperty>().Where(p => p.Name == "Message").Single().length.Should().Be("123");
			var hbmNestedComponent = hbmComponent.Properties.OfType<HbmComponent>().Where(p => p.Name == "NestedComponent").Single();
			hbmNestedComponent.Properties.OfType<HbmProperty>().Where(p => p.Name == "Message").Single().length.Should().Be("123");
		}

		[Test]
		public void WhenCustomizePropertyOnInterfaceAndOverrideOnImplThenApplyCustomizationOnImplementations()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<MyEntity>();

			var mapper = new Mapper(orm);
			mapper.Customize<IHasMessage>(x => x.Property(hasMessage => hasMessage.Message, pm => pm.Length(123)));
			mapper.Customize<MyNestedComponent>(x => x.Property(hasMessage => hasMessage.Message, pm => pm.Length(456)));

			var mappings = mapper.CompileMappingFor(new[] { typeof(MyEntity) });

			HbmClass hbmMyEntity = mappings.RootClasses.Single(hbm => hbm.Name == "MyEntity");
			var hbmComponent = hbmMyEntity.Properties.OfType<HbmComponent>().Where(p => p.Name == "Component").Single();
			hbmComponent.Properties.OfType<HbmProperty>().Where(p => p.Name == "Message").Single().length.Should().Be("123");
			var hbmNestedComponent = hbmComponent.Properties.OfType<HbmComponent>().Where(p => p.Name == "NestedComponent").Single();
			hbmNestedComponent.Properties.OfType<HbmProperty>().Where(p => p.Name == "Message").Single().length.Should().Be("456");
		}

		[Test]
		public void WhenCustomizePropertyOnInterfaceAndComponentIsElementThenApplyCustomizationOnImplementations()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<MyEntityWithCollection>();

			var mapper = new Mapper(orm);
			mapper.Customize<IHasMessage>(x => x.Property(hasMessage => hasMessage.Message, pm => pm.Length(123)));

			var mappings = mapper.CompileMappingFor(new[] { typeof(MyEntityWithCollection) });

			HbmClass hbmMyEntity = mappings.RootClasses.Single(hbm => hbm.Name == "MyEntityWithCollection");
			var hbmBag = hbmMyEntity.Properties.OfType<HbmBag>().Where(p => p.Name == "Components").Single();
			var hbmComponent = (HbmCompositeElement)hbmBag.ElementRelationship;
			hbmComponent.Properties.OfType<HbmProperty>().Where(p => p.Name == "Message").Single().length.Should().Be("123");
			var hbmNestedComponent = hbmComponent.Properties.OfType<HbmNestedCompositeElement>().Where(p => p.Name == "NestedComponent").Single();
			hbmNestedComponent.Properties.OfType<HbmProperty>().Where(p => p.Name == "Message").Single().length.Should().Be("123");
		}

		[Test]
		public void WhenCustomizePropertyOnInterfaceAndOverrideOnImplAndComponentIsElementThenApplyCustomizationOnImplementations()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<MyEntityWithCollection>();

			var mapper = new Mapper(orm);
			mapper.Customize<IHasMessage>(x => x.Property(hasMessage => hasMessage.Message, pm => pm.Length(123)));
			mapper.Customize<MyComponent>(x => x.Property(hasMessage => hasMessage.Message, pm => pm.Length(456)));
			mapper.Customize<MyNestedComponent>(x => x.Property(hasMessage => hasMessage.Message, pm => pm.Length(789)));

			var mappings = mapper.CompileMappingFor(new[] { typeof(MyEntityWithCollection) });

			HbmClass hbmMyEntity = mappings.RootClasses.Single(hbm => hbm.Name == "MyEntityWithCollection");
			var hbmBag = hbmMyEntity.Properties.OfType<HbmBag>().Where(p => p.Name == "Components").Single();
			var hbmComponent = (HbmCompositeElement)hbmBag.ElementRelationship;
			hbmComponent.Properties.OfType<HbmProperty>().Where(p => p.Name == "Message").Single().length.Should().Be("456");
			var hbmNestedComponent = hbmComponent.Properties.OfType<HbmNestedCompositeElement>().Where(p => p.Name == "NestedComponent").Single();
			hbmNestedComponent.Properties.OfType<HbmProperty>().Where(p => p.Name == "Message").Single().length.Should().Be("789");
		}
	}
}