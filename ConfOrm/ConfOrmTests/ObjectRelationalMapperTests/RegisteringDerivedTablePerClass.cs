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
			(typeof(TestSubEntity)).Satisfy(te => mapper.IsTablePerClass(te));
			(typeof(TestSubSubEntity)).Satisfy(te => mapper.IsTablePerClass(te));
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
			(typeof(TestSubEntity)).Satisfy(te => !mapper.IsComplex(te));
			(typeof(TestSubSubEntity)).Satisfy(te => !mapper.IsComplex(te));
		}

		[Test]
		public void IsNotRecognizedAsTablePerHierarchy()
		{
			(typeof(TestSubEntity)).Satisfy(te => !mapper.IsTablePerHierarchy(te));
			(typeof(TestSubSubEntity)).Satisfy(te => !mapper.IsTablePerHierarchy(te));
		}

		[Test]
		public void IsNotRecognizedAsTablePerConcreteClass()
		{
			(typeof(TestSubEntity)).Satisfy(te => !mapper.IsTablePerConcreteClass(te));
			(typeof(TestSubSubEntity)).Satisfy(te => !mapper.IsTablePerConcreteClass(te));
		}
	}
}