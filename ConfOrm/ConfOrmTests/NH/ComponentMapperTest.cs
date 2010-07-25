using System.Collections.Generic;
using ConfOrm.Mappers;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class ComponentMapperTest
	{
		private class Person
		{
			public int Id { get; set; }
			public Name Name { get; set; }
			public IList<Address> Addresses { get; set; }
		}

		private class Name
		{
			public string First { get; set; }
			public string Last { get; set; }
		}
		private class Address
		{
			private Person parent;
			public Person Parent
			{
				get { return parent; }
				set { parent = value; }
			}

			public string Street { get; set; }
		}

		[Test]
		public void WhenMapDocHasDefaultHasClassName()
		{
			var entityType = typeof (Person);
			var mapdoc = new HbmMapping { assembly = entityType.Assembly.FullName, @namespace = entityType.Namespace };
			var component = new HbmComponent();
			new ComponentMapper(component, typeof(Name), mapdoc);
			component.Class.Should().Be.EqualTo(typeof(Name).Name);
		}

		[Test]
		public void CallingParentSetTheParentNode()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmComponent();
			var mapper = new ComponentMapper(component, typeof(Name), mapdoc);
			mapper.Parent(typeof (Address).GetProperty("Parent"));
			component.Parent.Should().Not.Be.Null();
			component.Parent.name.Should().Be.EqualTo("Parent");
		}

		[Test]
		public void CanSetParentAccessor()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmComponent();
			var mapper = new ComponentMapper(component, typeof(Name), mapdoc);
			mapper.Parent(typeof(Address).GetProperty("Parent"), pm=> pm.Access(Accessor.Field));
			component.Parent.access.Should().Contain("field");
		}


		[Test]
		public void CanSetUpdate()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmComponent();
			var mapper = new ComponentMapper(component, typeof(Name), mapdoc);

			mapper.Update(false);
			component.update.Should().Be.False();
		}

		[Test]
		public void CanSetInsert()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmComponent();
			var mapper = new ComponentMapper(component, typeof(Name), mapdoc);

			mapper.Insert(false);
			component.insert.Should().Be.False();
		}
	}
}