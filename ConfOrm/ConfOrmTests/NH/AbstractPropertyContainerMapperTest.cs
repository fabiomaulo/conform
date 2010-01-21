using System;
using System.Collections.Generic;
using System.Linq;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class AbstractPropertyContainerMapperTest
	{
		private class EntitySimple
		{
			public string Name { get; set; }
		}
		private class InheritedEntitySimple : EntitySimple
		{
		}

		private class OtherSimple
		{
			public string Name { get; set; }
		}

		[Test]
		public void CantCreateWithoutHbmMapping()
		{
			ActionAssert.Throws<ArgumentNullException>(() => new HackPropertyContainerMapper(typeof(EntitySimple), null));
		}

		[Test]
		public void CantCreateWithoutContainerType()
		{
			ActionAssert.Throws<ArgumentNullException>(() => new HackPropertyContainerMapper(null, new HbmMapping()));
		}

		[Test]
		public void CanAddSimpleProperty()
		{
			var properties = new List<object>();
			var map = new StubPropertyContainerMapper<EntitySimple>(properties);
			map.Property(typeof(EntitySimple).GetProperty("Name"));

			properties.Should().Have.Count.EqualTo(1);
			properties.First().Should().Be.OfType<HbmProperty>().And.ValueOf.Name.Should().Be.EqualTo("Name");
		}

		[Test]
		public void CantAddPropertyOfNotInheritedType()
		{
			var map = new StubPropertyContainerMapper<OtherSimple>(new List<object>());
			ActionAssert.Throws<ArgumentOutOfRangeException>(()=>map.Property(typeof(EntitySimple).GetProperty("Name")));
		}

		[Test]
		public void CanAddPropertyOfInheritedType()
		{
			var properties = new List<object>();
			var map = new StubPropertyContainerMapper<InheritedEntitySimple>(properties);
			map.Property(typeof(InheritedEntitySimple).GetProperty("Name"));

			properties.Should().Have.Count.EqualTo(1);
			properties.First().Should().Be.OfType<HbmProperty>().And.ValueOf.Name.Should().Be.EqualTo("Name");
		}

		private class HackPropertyContainerMapper : AbstractPropertyContainerMapper
		{
			public HackPropertyContainerMapper(Type container, HbmMapping mapDoc) : base(container, mapDoc) {}

			#region Overrides of AbstractPropertyContainerMapper

			protected override void AddProperty(object property)
			{
				throw new NotImplementedException();
			}

			#endregion
		}

		private class StubPropertyContainerMapper<T> : AbstractPropertyContainerMapper
		{
			private readonly ICollection<object> elements;

			public StubPropertyContainerMapper(ICollection<object> elements)
				: base(typeof(T), new HbmMapping())
			{
				this.elements = elements;
			}

			#region Overrides of AbstractClassMapping<StateProvince>

			protected override void AddProperty(object property)
			{
				elements.Add(property);
			}

			#endregion
		}
	}
}