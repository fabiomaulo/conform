using System.Reflection;
using ConfOrm;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.ObjectRelationalMapperTests
{
	public class PersistentIdTest
	{
		private class TestEntity
		{
			public int Id { get; set; }
			public int Pizza { get; set; }
		}

		private ObjectRelationalMapper mapper;
		[TestFixtureSetUp]
		public void RegisterTablePerClass()
		{
			mapper = new ObjectRelationalMapper();
		}

		[Test]
		public void RecognizePoidAsPersistentProperty()
		{
			PropertyInfo pi = typeof(TestEntity).GetProperty("Id");
			mapper.IsPersistentProperty(pi).Should().Be.True();
		}

		[Test]
		public void RecognizePoidAsPersistentId()
		{
			PropertyInfo pi = typeof(TestEntity).GetProperty("Id");
			mapper.IsPersistentId(pi).Should().Be.True();
		}

		[Test]
		public void NotRecognizeAnyPropertyAsPersistentId()
		{
			PropertyInfo pi = typeof(TestEntity).GetProperty("Pizza");
			mapper.IsPersistentId(pi).Should().Be.False();
		}
	}
}