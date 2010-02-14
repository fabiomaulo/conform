using System.Collections.Generic;
using System.Linq;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class ComponentElementMapperTest
	{
		private class Person
		{
			public int Id { get; set; }
			public IList<Address> Addresses { get; set; }
		}

		private class Address
		{
			public Person Parent { get; set; }
			public string Street { get; set; }
			public Number Number { get; set; }
		}

		private class Number
		{
			public int Block { get; set; }
			public int Dir { get; set; }
		}

		[Test]
		public void CanMapParent()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmCompositeElement();
			var mapper = new ComponentElementMapper(typeof(Address), mapdoc, component);
			mapper.Parent(typeof(Address).GetProperty("Parent"));
			component.Parent.Should().Not.Be.Null();
			component.Parent.name.Should().Be.EqualTo("Parent");
		}

		[Test]
		public void CanMapProperty()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmCompositeElement();
			var mapper = new ComponentElementMapper(typeof(Address), mapdoc, component);
			mapper.Property(typeof(Address).GetProperty("Street"), x => { });
			component.Properties.Should().Have.Count.EqualTo(1);
			component.Properties.First().Name.Should().Be.EqualTo("Street");
		}

		[Test]
		public void CallPropertyMapper()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmCompositeElement();
			var mapper = new ComponentElementMapper(typeof(Address), mapdoc, component);
			var called = false;
			mapper.Property(typeof (Address).GetProperty("Street"), x => called = true);
			called.Should().Be.True();
		}

		[Test]
		public void CanMapManyToOne()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmCompositeElement();
			var mapper = new ComponentElementMapper(typeof(Address), mapdoc, component);
			mapper.ManyToOne(typeof(Address).GetProperty("Parent"), x => { });
			component.Properties.Should().Have.Count.EqualTo(1);
			component.Properties.First().Name.Should().Be.EqualTo("Parent");
			component.Properties.First().Should().Be.OfType<HbmManyToOne>();
		}

		[Test]
		public void CanMapNestedComponent()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmCompositeElement();
			var mapper = new ComponentElementMapper(typeof(Address), mapdoc, component);
			mapper.Component(typeof(Address).GetProperty("Number"), x => { });
			component.Properties.Should().Have.Count.EqualTo(1);
			component.Properties.First().Name.Should().Be.EqualTo("Number");
			component.Properties.First().Should().Be.OfType<HbmNestedCompositeElement>();
		}

		[Test]
		public void CallNestedComponentMapping()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmCompositeElement();
			var mapper = new ComponentElementMapper(typeof(Address), mapdoc, component);
			var called = false;
			mapper.Component(typeof(Address).GetProperty("Number"), x => called = true );
			called.Should().Be.True();
		}
	}
}