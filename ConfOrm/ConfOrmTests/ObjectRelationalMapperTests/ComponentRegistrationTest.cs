using ConfOrm;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.ObjectRelationalMapperTests
{
	public class ComponentRegistrationTest
	{
		private class AComponent
		{
			public string Fisrt { get; set; }
		}

		[Test]
		public void WhenComponetRegisteredIsRecognized()
		{
			var mapper = new ObjectRelationalMapper();
			mapper.Component<AComponent>();
			mapper.IsComponent(typeof(AComponent)).Should().Be.True();
		}

		[Test]
		public void RegisteringComponentAndEntityThrow()
		{
			var mapper = new ObjectRelationalMapper();
			mapper.Component<AComponent>();
			ActionAssert.Throws(() => mapper.TablePerClass<AComponent>()).Should().Be.InstanceOf<MappingException>();
			ActionAssert.Throws(() => mapper.TablePerConcreteClass<AComponent>()).Should().Be.InstanceOf<MappingException>();
			ActionAssert.Throws(() => mapper.TablePerClassHierarchy<AComponent>()).Should().Be.InstanceOf<MappingException>();
		}

		[Test]
		public void RegisteringEntityAsComponentThrow()
		{
			var mapper = new ObjectRelationalMapper();
			mapper.TablePerClass<AComponent>();
			ActionAssert.Throws(() => mapper.Component<AComponent>()).Should().Be.InstanceOf<MappingException>();
		}
	}
}