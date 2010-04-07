using System;
using ConfOrm.Mappers;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class OneToManyMapperTest
	{
		private class MyClass
		{
			
		}

		[Test]
		public void WhenCreateWithoutTypeThenNotThrow()
		{
			// relation using entity-name
			var hbm = new HbmOneToMany();
			ActionAssert.NotThrow(()=>new OneToManyMapper(null, hbm, null));
		}

		[Test]
		public void WhenNoHbmThenThrow()
		{
			ActionAssert.Throws<ArgumentNullException>(() => new OneToManyMapper(null, null, null));
		}

		[Test]
		public void WhenCreateWithTypeThenAssignClass()
		{
			var hbm = new HbmOneToMany();
			new OneToManyMapper(typeof (MyClass), hbm, null);
			hbm.Class.Should().Not.Be.NullOrEmpty().And.Contain("MyClass");
		}

		[Test]
		public void CanAssignNotFoundMode()
		{
			var hbm = new HbmOneToMany();
			var mapper = new OneToManyMapper(typeof(MyClass), hbm, null);
			mapper.NotFound(NotFoundMode.Ignore);
			hbm.NotFoundMode.Should().Be(HbmNotFoundMode.Ignore);
		}
	}
}