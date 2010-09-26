using ConfOrm.Mappers;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class SchemaActionConverterTest
	{
		[Test]
		public void WhenContainAllThenShouldBeNull()
		{
			(SchemaAction.All | SchemaAction.Drop).ToSchemaActionString().Should().Be.Null();
		}

		[Test]
		public void WhenEqualNoneThenHasOnlyNone()
		{
			SchemaAction.None.ToSchemaActionString().Should().Be("none");
		}

		[Test]
		public void WhenContainVariousValuesThenComposeString()
		{
			(SchemaAction.Drop | SchemaAction.Update).ToSchemaActionString().Should().Be("drop,update");
			(SchemaAction.Export | SchemaAction.Validate).ToSchemaActionString().Should().Be("export,validate");
			(SchemaAction.Export | SchemaAction.Validate | SchemaAction.Update).ToSchemaActionString()
				.Should().Contain("update").And.Contain("export").And.Contain("validate");
		}

		[Test]
		public void WhenComposeNoneThenIgnoreIt()
		{
			(SchemaAction.Drop | SchemaAction.None).ToSchemaActionString().ToLowerInvariant().Should()
				.Contain("drop").And.Not.Contain("none");
		}
	}
}