using ConfOrm;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.ObjectRelationalMapperTests
{
	public class OneToOneRelationTest
	{
		private class Person
		{
			public int Id { get; set; }
		}

		private class User
		{
			public int Id { get; set; }
			public Person Person { get; set; }
		}

		[Test]
		public void WhenNotRegisteredAsEntityNotRecognizeRelation()
		{
			var mapper = new ObjectRelationalMapper();
			mapper.OneToOne<User, Person>();
			mapper.IsOneToOne(typeof(User), typeof(Person)).Should().Be.False();
		}

		[Test]
		public void WhenNotRegisteredAsEntityNotRecognizeInverseRelation()
		{
			var mapper = new ObjectRelationalMapper();
			mapper.OneToOne<User, Person>();
			mapper.IsOneToOne(typeof(Person), typeof(User)).Should().Be.False();
		}

		[Test]
		public void WhenExplicitRegisteredRecognizeRelation()
		{
			var mapper = new ObjectRelationalMapper();
			mapper.TablePerClass<User>();
			mapper.TablePerClass<Person>();
			mapper.OneToOne<User, Person>();
			mapper.IsOneToOne(typeof(User), typeof(Person)).Should().Be.True();
		}

		[Test]
		public void WhenExplicitRegisteredRecognizeInverseRelation()
		{
			var mapper = new ObjectRelationalMapper();
			mapper.TablePerClass<User>();
			mapper.TablePerClass<Person>();
			mapper.OneToOne<User, Person>();
			mapper.IsOneToOne(typeof(Person), typeof(User)).Should().Be.True();
		}

	}
}