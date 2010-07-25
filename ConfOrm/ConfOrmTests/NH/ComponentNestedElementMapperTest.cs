using System.Collections.Generic;
using System.Linq;
using ConfOrm;
using ConfOrm.Mappers;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class ComponentNestedElementMapperTest
	{
		private class Person
		{
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
			public Address Parent { get; set; }
			public int Block { get; set; }
			public int Dir { get; set; }
			public Number2 NestNumber { get; set; }
		}

		private class Number2
		{
			public int Block { get; set; }
			public int Dir { get; set; }
		}

		[Test]
		public void CanMapParent()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmNestedCompositeElement();
			var mapper = new ComponentNestedElementMapper(typeof(Number), mapdoc, component, ForClass<Address>.Property(a=> a.Number));
			mapper.Parent(typeof(Address).GetProperty("Parent"));
			component.Parent.Should().Not.Be.Null();
			component.Parent.name.Should().Be.EqualTo("Parent");
		}

		[Test]
		public void CanMapProperty()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmNestedCompositeElement();
			var mapper = new ComponentNestedElementMapper(typeof(Number), mapdoc, component, ForClass<Address>.Property(a => a.Number));
			
			mapper.Property(ForClass<Number>.Property(a => a.Block), x => { });
			mapper.Property(ForClass<Number>.Property(a => a.Dir), x => { });

			component.Properties.Should().Have.Count.EqualTo(2);
			component.Properties.Select(cp => cp.Name).Should().Have.SameValuesAs("Block", "Dir");
		}

		[Test]
		public void CallPropertyMapper()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmNestedCompositeElement();
			var mapper = new ComponentNestedElementMapper(typeof(Number), mapdoc, component, ForClass<Address>.Property(a => a.Number));
			var called = false;
			mapper.Property(ForClass<Number>.Property(a => a.Block), x => called = true);
			called.Should().Be.True();
		}

		[Test]
		public void CanMapManyToOne()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmNestedCompositeElement();
			var mapper = new ComponentNestedElementMapper(typeof(Number), mapdoc, component, ForClass<Address>.Property(a => a.Number));
			mapper.ManyToOne(ForClass<Number>.Property(a => a.Parent), x => { });
			component.Properties.Should().Have.Count.EqualTo(1);
			component.Properties.First().Name.Should().Be.EqualTo("Parent");
			component.Properties.First().Should().Be.OfType<HbmManyToOne>();
		}

		[Test]
		public void CanMapNestedComponent()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmNestedCompositeElement();
			var mapper = new ComponentNestedElementMapper(typeof(Number2), mapdoc, component, ForClass<Number>.Property(a => a.NestNumber));
			mapper.Component(ForClass<Number>.Property(a => a.NestNumber), x => { });
			component.Properties.Should().Have.Count.EqualTo(1);
			component.Properties.First().Name.Should().Be.EqualTo("NestNumber");
			component.Properties.First().Should().Be.OfType<HbmNestedCompositeElement>();
		}

		[Test]
		public void CallNestedComponentMapping()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmNestedCompositeElement();
			var mapper = new ComponentNestedElementMapper(typeof(Number2), mapdoc, component, ForClass<Number>.Property(a => a.NestNumber));
			var called = false;
			mapper.Component(ForClass<Number>.Property(a => a.NestNumber), x => called = true);
			called.Should().Be.True();
		}

		[Test]
		public void CanSetComponentAccessor()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmNestedCompositeElement();
			var mapper = new ComponentNestedElementMapper(typeof(Number), mapdoc, component, ForClass<Address>.Property(a => a.Number));
			mapper.Access(Accessor.Field);
			component.access.Should().Contain("field");
		}
	}
}