using ConfOrm;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.ObjectRelationalMapperTests
{
	public class RegisteringTablePerClass
	{
		private class TestEntity
		{
			public int Id { get; set; }
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
			(typeof (TestEntity)).Satisfy(te=> mapper.IsTablePerClass(te));
		}

		[Test]
		public void IsRecognizedAsEntity()
		{
			(typeof(TestEntity)).Satisfy(te => mapper.IsEntity(te));
		}

		[Test]
		public void IsRecognizedAsRootEntity()
		{
			(typeof(TestEntity)).Satisfy(te => mapper.IsRootEntity(te));
		}

		[Test]
		public void IsNotRecognizedAsComplex()
		{
			(typeof(TestEntity)).Satisfy(te => !mapper.IsComplex(te));
		}

		[Test]
		public void IsNotRecognizedAsTablePerHierarchy()
		{
			(typeof(TestEntity)).Satisfy(te => !mapper.IsTablePerClassHierarchy(te));
		}

		[Test]
		public void IsNotRecognizedAsTablePerConcreteClass()
		{
			(typeof(TestEntity)).Satisfy(te => !mapper.IsTablePerConcreteClass(te));
		}
	}
}