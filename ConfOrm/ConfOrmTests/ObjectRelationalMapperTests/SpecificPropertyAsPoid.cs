using ConfOrm;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.ObjectRelationalMapperTests
{
	public class SpecificPropertyAsPoid
	{
		private class MyClass
		{
			public string EMail { get; set; }
		}

		[Test]
		public void WhenExplicitDeclaredThenRecognizePoid()
		{
			var orm = new ObjectRelationalMapper();
			orm.Poid<MyClass>(mc => mc.EMail);
			orm.IsPersistentId(typeof(MyClass).GetProperty("EMail")).Should().Be.True();
		}

		[Test]
		public void WhenNotExplicitDeclaredThenNotRecognizePoid()
		{
			var orm = new ObjectRelationalMapper();
			orm.IsPersistentId(typeof(MyClass).GetProperty("EMail")).Should().Be.False();
		}
	}
}