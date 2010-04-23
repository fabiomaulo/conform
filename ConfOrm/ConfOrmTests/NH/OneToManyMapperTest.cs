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
		private interface IMyInterface
		{
			
		}
		private class MyClass: IMyInterface
		{
			
		}
		private class Whatever
		{

		}

		[Test]
		public void WhenCreateWithoutTypeThenNotThrow()
		{
			// relation using entity-name
			var hbm = new HbmOneToMany();
			Executing.This(() => new OneToManyMapper(null, hbm, null)).Should().NotThrow();
		}

		[Test]
		public void WhenNoHbmThenThrow()
		{
			Executing.This(() => new OneToManyMapper(null, null, null)).Should().Throw<ArgumentNullException>();
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

		[Test]
		public void CanForceClassRelation()
		{
			var hbm = new HbmOneToMany();
			var mapper = new OneToManyMapper(typeof(IMyInterface), hbm, null);

			mapper.Class(typeof(MyClass));

			hbm.Class.Should().Contain("MyClass").And.Not.Contain("IMyInterface");
		}

		[Test]
		public void WhenForceClassRelationToIncompatibleTypeThenThrows()
		{
			var hbm = new HbmOneToMany();
			var mapper = new OneToManyMapper(typeof(IMyInterface), hbm, null);

			Executing.This(() => mapper.Class(typeof(Whatever))).Should().Throw<ArgumentOutOfRangeException>();
		}

		[Test]
		public void CanAssignEntityName()
		{
			var hbm = new HbmOneToMany();
			var mapper = new OneToManyMapper(typeof(MyClass), hbm, null);
			mapper.EntityName("myname");
			hbm.EntityName.Should().Be("myname");
		}
	}
}