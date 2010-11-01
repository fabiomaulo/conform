using System.Linq;
using ConfOrm;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH.MapperTests
{
	public class InterfacePropertiesPolymorphicCustomizationTest
	{
		private interface IHasMessage
		{
			string Message { get; set; }
		}

		private class MyClass1: IHasMessage
		{
			public int Id { get; set; }
			public string Message { get; set; }
		}

		private class MyClass2 : IHasMessage
		{
			public int Id { get; set; }
			public string Message { get; set; }
		}

		[Test]
		public void WhenCustomizePropertyOnInterfaceThenApplyCustomizationOnImplementations()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<MyClass1>();
			orm.TablePerClass<MyClass2>();

			var mapper = new Mapper(orm);
			mapper.Customize<IHasMessage>(x=> x.Property(hasMessage=> hasMessage.Message, pm=> pm.Length(123)));
			var mappings = mapper.CompileMappingFor(new[] { typeof(MyClass1), typeof(MyClass2) });

			HbmClass hbmMyClass1 = mappings.RootClasses.Single(hbm => hbm.Name == "MyClass1");
			hbmMyClass1.Properties.OfType<HbmProperty>().Where(p=> p.Name == "Message").Single().length.Should().Be("123");

			HbmClass hbmMyClass2 = mappings.RootClasses.Single(hbm => hbm.Name == "MyClass2");
			hbmMyClass2.Properties.OfType<HbmProperty>().Where(p => p.Name == "Message").Single().length.Should().Be("123");
		}

		[Test]
		public void WhenCustomizePropertyOnInterfaceAndOverrideOnImplThenApplyCustomizationOnImplementations()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<MyClass1>();
			orm.TablePerClass<MyClass2>();

			var mapper = new Mapper(orm);
			mapper.Customize<IHasMessage>(x => x.Property(hasMessage => hasMessage.Message, pm => pm.Length(123)));
			mapper.Customize<MyClass2>(x => x.Property(hasMessage => hasMessage.Message, pm => pm.Length(456)));

			var mappings = mapper.CompileMappingFor(new[] { typeof(MyClass1), typeof(MyClass2) });

			HbmClass hbmMyClass1 = mappings.RootClasses.Single(hbm => hbm.Name == "MyClass1");
			hbmMyClass1.Properties.OfType<HbmProperty>().Where(p => p.Name == "Message").Single().length.Should().Be("123");

			HbmClass hbmMyClass2 = mappings.RootClasses.Single(hbm => hbm.Name == "MyClass2");
			hbmMyClass2.Properties.OfType<HbmProperty>().Where(p => p.Name == "Message").Single().length.Should().Be("456");
		}
	}
}