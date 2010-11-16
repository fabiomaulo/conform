using System;
using System.Linq;
using ConfOrm;
using ConfOrm.Mappers;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class KeyMapperTest
	{
		private class Animal
		{
			public virtual string Name { get; set; }
		}

		private class B
		{
			public virtual string Name { get; set; }
		}

		[Test]
		public void AutoAssignColumnNameByDefault()
		{
			var hbm = new HbmKey();
			var km = new KeyMapper(typeof (Animal), hbm);
			hbm.Columns.Should().Have.Count.EqualTo(1);
			hbm.Columns.First().name.Should().Not.Be.Empty().And.Not.Be.Null();
		}

		[Test]
		public void WhenExplicitAssignementThenOverrideDefaultColumnName()
		{
			var hbm = new HbmKey();
			var km = new KeyMapper(typeof(Animal), hbm);
			km.Column("blha");
			hbm.Columns.First().name.Should().Be.EqualTo("blha");
		}

		[Test]
		public void AssignOnDeleteAction()
		{
			var hbm = new HbmKey();
			var km = new KeyMapper(typeof(Animal), hbm);
			km.OnDelete(OnDeleteAction.Cascade);
			hbm.ondelete.Should().Be.EqualTo(HbmOndelete.Cascade);
			km.OnDelete(OnDeleteAction.NoAction);
			hbm.ondelete.Should().Be.EqualTo(HbmOndelete.Noaction);
		}

		[Test]
		public void AssignPropertyReference()
		{
			var hbm = new HbmKey();
			var km = new KeyMapper(typeof(Animal), hbm);
			
			km.PropertyRef(ForClass<Animal>.Property(x=> x.Name));
			hbm.propertyref.Should().Be("Name");
		}

		[Test]
		public void WhenAssignReferenceToNullThenNullifyReference()
		{
			var hbm = new HbmKey();
			var km = new KeyMapper(typeof(Animal), hbm);
			km.PropertyRef(null);
			hbm.propertyref.Should().Be.Null();
		}

		[Test]
		public void WhenAssignReferenceOutSideTheOwnerEntityThenThrow()
		{
			var hbm = new HbmKey();
			var km = new KeyMapper(typeof(Animal), hbm);
			Executing.This(() => km.PropertyRef(ForClass<B>.Property(x => x.Name))).Should().Throw<ArgumentOutOfRangeException>();
		}

		[Test]
		public void AssignUpdate()
		{
			var hbm = new HbmKey();
			var km = new KeyMapper(typeof(Animal), hbm);

			km.Update(false);
			hbm.update.Should().Be.False();
			hbm.updateSpecified.Should().Be.True();

			km.Update(true);
			hbm.update.Should().Be.True();
			hbm.updateSpecified.Should().Be.True();
		}

		[Test]
		public void WhenAssignFKToNullThenSetToNull()
		{
			var hbm = new HbmKey();
			hbm.foreignkey = "aPreviousValue";
			var km = new KeyMapper(typeof(Animal), hbm);
			km.ForeignKey(null);
			hbm.foreignkey.Should().Be.Null();
		}

		[Test]
		public void WhenAssignFKToEmptyThenAssignNone()
		{
			var hbm = new HbmKey();
			var km = new KeyMapper(typeof(Animal), hbm);
			km.ForeignKey("");
			hbm.foreignkey.Should().Be("none");
		}

		[Test]
		public void WhenAssignFKToWhiteSpacesThenAssignNone()
		{
			var hbm = new HbmKey();
			var km = new KeyMapper(typeof(Animal), hbm);
			km.ForeignKey("    ");
			hbm.foreignkey.Should().Be("none");
		}

		[Test]
		public void WhenAssignFKToValidNameThenAssignName()
		{
			var hbm = new HbmKey();
			var km = new KeyMapper(typeof(Animal), hbm);
			km.ForeignKey("FKDeLaPizza");
			hbm.foreignkey.Should().Be("FKDeLaPizza");
		}
	}
}