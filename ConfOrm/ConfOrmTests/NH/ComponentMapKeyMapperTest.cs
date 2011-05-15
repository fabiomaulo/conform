using System.Collections.Generic;
using System.Linq;
using ConfOrm;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class ComponentMapKeyMapperTest
	{
		private class Person
		{
			public int Id { get; set; }
			public string Email { get; set; }
			public IDictionary<ToySkill, double> Skills { get; set; }
		}

		private class ToySkill
		{
			public Skill Skill { get; set; }
			public int Level { get; set; }
		}

		private class Skill
		{
			public int Id { get; set; }
			public string Name { get; set; }
		}

		[Test]
		public void WhenCreatedThenSetTheComponentClass()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmCompositeMapKey();
			new ComponentMapKeyMapper(typeof(ToySkill), component, mapdoc);

			component.Class.Should().Contain("ToySkill");
		}

		[Test]
		public void CanMapProperty()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmCompositeMapKey();
			var mapper = new ComponentMapKeyMapper(typeof(ToySkill), component, mapdoc);

			mapper.Property(ForClass<ToySkill>.Property(ts=> ts.Level), x => { });

			component.Properties.Should().Have.Count.EqualTo(1);
			component.Properties.First().Should().Be.OfType<HbmKeyProperty>();
			component.Properties.First().Name.Should().Be.EqualTo("Level");
		}

		[Test]
		public void CallPropertyMapper()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmCompositeMapKey();
			var mapper = new ComponentMapKeyMapper(typeof(ToySkill), component, mapdoc);
			var called = false;

			mapper.Property(ForClass<ToySkill>.Property(ts => ts.Level), x => called = true);

			called.Should().Be.True();
		}

		[Test]
		public void CanMapManyToOne()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmCompositeMapKey();
			var mapper = new ComponentMapKeyMapper(typeof(ToySkill), component, mapdoc);

			mapper.ManyToOne(ForClass<ToySkill>.Property(ts => ts.Skill), x => { });

			component.Properties.Should().Have.Count.EqualTo(1);
			component.Properties.First().Name.Should().Be.EqualTo("Skill");
			component.Properties.First().Should().Be.OfType<HbmKeyManyToOne>();
		}

		[Test]
		public void CallMapManyToOneMapper()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmCompositeMapKey();
			var mapper = new ComponentMapKeyMapper(typeof(ToySkill), component, mapdoc);
			var called = false;

			mapper.ManyToOne(ForClass<ToySkill>.Property(ts => ts.Skill), x => called = true);

			called.Should().Be.True();
		}
	}
}