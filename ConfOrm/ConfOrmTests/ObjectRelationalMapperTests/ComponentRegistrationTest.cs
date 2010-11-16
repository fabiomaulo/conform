using System;
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
		public void WhenComponentRegisteredIsRecognized()
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
			Executing.This(() => mapper.TablePerClass<AComponent>()).Should().Throw().And.Exception.Should().Be.InstanceOf<MappingException>();
			Executing.This(() => mapper.TablePerConcreteClass<AComponent>()).Should().Throw().And.Exception.Should().Be.InstanceOf<MappingException>();
			Executing.This(() => mapper.TablePerClassHierarchy<AComponent>()).Should().Throw().And.Exception.Should().Be.InstanceOf<MappingException>();
		}

		[Test]
		public void RegisteringEntityAsComponentThrow()
		{
			var mapper = new ObjectRelationalMapper();
			mapper.TablePerClass<AComponent>();
			Executing.This(() => mapper.Component<AComponent>()).Should().Throw().And.Exception.Should().Be.InstanceOf<MappingException>();
		}

		[Test]
		public void WhenNotExplicitRegisteredRecognizeTheComponent()
		{
			var mapper = new ObjectRelationalMapper();
			mapper.IsComponent(typeof(AComponent)).Should().Be.True();
		}

		[Test]
		public void WhenExplicitRegisteredAsEntityIsNotComponent()
		{
			var mapper = new ObjectRelationalMapper();
			mapper.TablePerClass<AComponent>();
			mapper.IsComponent(typeof(AComponent)).Should().Be.False();
		}

		[Test]
		public void ValueTypesAndSystemTypesShouldBeNotComponents()
		{
			var mapper = new ObjectRelationalMapper();
			mapper.IsComponent(typeof(string)).Should().Be.False();
			mapper.IsComponent(typeof(DateTime)).Should().Be.False();
			mapper.IsComponent(typeof(int)).Should().Be.False();
		}
	}
}