using System.Collections.Generic;
using ConfOrm;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.ObjectRelationalMapperTests
{
	public class OneToManyInsideComponent
	{
		private class Parent
		{
			public int Id { get; set; }
			public IList<Child> Children { get; set; }
		}

		private class Child
		{
			public Parent Parent { get; set; }
			public IList<Image> MultiMedia { get; set; }
		}

		private class Image
		{
			public int Id { get; set; }
			public string FileName { get; set; }
		}

		[Test]
		public void RecognizeOneToMany()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Parent>();
			orm.TablePerClass<Image>();

			orm.IsOneToMany(typeof (Child), typeof (Image)).Should().Be.True();
		}
	}
}