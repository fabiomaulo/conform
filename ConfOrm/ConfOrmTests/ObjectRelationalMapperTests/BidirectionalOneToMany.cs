using System.Collections.Generic;
using ConfOrm;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.ObjectRelationalMapperTests
{
	public class BidirectionalOneToMany
	{
		private class Parent
		{
			public int Id { get; set; }
			public IList<Child> Children { get; set; }
		}

		private class Child
		{
			public int Id { get; set; }
			public Parent Parent { get; set; }
		}
		ObjectRelationalMapper orm;

		[TestFixtureSetUp]
		public void ConfigureOrm()
		{
			orm = new ObjectRelationalMapper();
			orm.TablePerClass<Parent>();
			orm.TablePerClass<Child>();
		}

		[Test]
		public void RecognizeOneToMany()
		{
			orm.IsOneToMany(typeof (Parent), typeof (Child)).Should().Be.True();
		}

		[Test]
		public void RecognizeManyToOne()
		{
			orm.IsManyToOne(typeof(Child), typeof(Parent)).Should().Be.True();
		}

		[Test]
		public void RecognizeCollectionAsBag()
		{
			var childrenProperty = typeof(Parent).GetProperty("Children");
			orm.IsBag(childrenProperty).Should().Be.True();
		}

		[Test]
		public void NotRecognizeCollectionAsList()
		{
			var childrenProperty = typeof(Parent).GetProperty("Children");
			orm.IsList(childrenProperty).Should().Be.False();
		}

		[Test]
		public void NotRecognizeCollectionAsArray()
		{
			var childrenProperty = typeof(Parent).GetProperty("Children");
			orm.IsArray(childrenProperty).Should().Be.False();
		}
	}
}