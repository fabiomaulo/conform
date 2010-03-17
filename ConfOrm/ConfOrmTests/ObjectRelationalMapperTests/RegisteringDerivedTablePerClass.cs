using ConfOrm;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.ObjectRelationalMapperTests
{
	public class RegisteringDerivedTablePerClass
	{
		private class TestEntity
		{
			public int Id { get; set; }
		}
		private class TestSubEntity : TestEntity
		{
		}
		private class TestSubSubEntity : TestSubEntity
		{
		}

		private class Dummy
		{
			public TestSubEntity TestSubEntity { get; set; }
			public TestSubSubEntity TestSubSubEntity { get; set; }
		}
		private ObjectRelationalMapper mapper;
		[TestFixtureSetUp]
		public void RegisterTablePerClass()
		{
			mapper = new ObjectRelationalMapper();
			mapper.TablePerClass<TestEntity>();
		}

		[Test]
		public void IsRecognizedAsTablePerClass()
		{
			mapper.IsTablePerClass(typeof (TestSubEntity)).Should().Be.True();
			mapper.IsTablePerClass(typeof(TestSubSubEntity)).Should().Be.True();
		}

		[Test]
		public void IsRecognizedAsEntity()
		{
			(typeof(TestSubEntity)).Satisfy(te => mapper.IsEntity(te));
			(typeof(TestSubSubEntity)).Satisfy(te => mapper.IsEntity(te));
		}

		[Test]
		public void IsNotRecognizedAsRootEntity()
		{
			(typeof(TestSubEntity)).Satisfy(te => !mapper.IsRootEntity(te));
			(typeof(TestSubSubEntity)).Satisfy(te => !mapper.IsRootEntity(te));
		}

		[Test]
		public void IsNotRecognizedAsComplex()
		{
			(typeof(Dummy)).GetProperty("TestSubEntity").Satisfy(te => !mapper.IsComplex(te));
			(typeof(Dummy)).GetProperty("TestSubSubEntity").Satisfy(te => !mapper.IsComplex(te));
		}

		[Test]
		public void IsNotRecognizedAsTablePerHierarchy()
		{
			(typeof(TestSubEntity)).Satisfy(te => !mapper.IsTablePerClassHierarchy(te));
			(typeof(TestSubSubEntity)).Satisfy(te => !mapper.IsTablePerClassHierarchy(te));
		}

		[Test]
		public void IsNotRecognizedAsTablePerConcreteClass()
		{
			(typeof(TestSubEntity)).Satisfy(te => !mapper.IsTablePerConcreteClass(te));
			(typeof(TestSubSubEntity)).Satisfy(te => !mapper.IsTablePerConcreteClass(te));
		}
	}
}