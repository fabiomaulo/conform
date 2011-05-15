using System;
using NHibernate.Mapping.ByCode;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class MapKeyRelationTest
	{
		private class MyClass
		{
			
		}
		[Test]
		public void CtorProtection()
		{
			Executing.This(() => new MapKeyRelation(null, null, null)).Should().Throw<ArgumentNullException>();
			Executing.This(() => new MapKeyRelation(typeof(string), null, null)).Should().Throw<ArgumentNullException>();
			Executing.This(() => new MapKeyRelation(typeof(string), new HbmMap(), null)).Should().Throw<ArgumentNullException>();
			Executing.This(() => new MapKeyRelation(typeof(string), null, new HbmMapping())).Should().Throw<ArgumentNullException>();
			Executing.This(() => new MapKeyRelation(null, new HbmMap(), new HbmMapping())).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void WhenAssignElementRelationThenUseHbmMapKeyElement()
		{
			var keyType = typeof (string);
			var hbmMap = new HbmMap();
			var hbmMapping = new HbmMapping();
			var mapper = new MapKeyRelation(keyType, hbmMap, hbmMapping);
			mapper.Element(mkm=> { });

			hbmMap.Item.Should().Be.OfType<HbmMapKey>();
		}

		[Test]
		public void WhenAssignElementRelationThenAssignType()
		{
			var keyType = typeof(string);
			var hbmMap = new HbmMap();
			var hbmMapping = new HbmMapping();
			var mapper = new MapKeyRelation(keyType, hbmMap, hbmMapping);
			mapper.Element(mkm => { });

			var keyElement = (HbmMapKey)hbmMap.Item;
			keyElement.Type.name.Should().Not.Be.Null();
			keyElement.Type.name.Should().Contain("String");
		}

		[Test]
		public void WhenAssignElementRelationThenCallActionWithNotNullParameter()
		{
			var keyType = typeof(string);
			var hbmMap = new HbmMap();
			var hbmMapping = new HbmMapping();
			var mapper = new MapKeyRelation(keyType, hbmMap, hbmMapping);
			var called = false;
			mapper.Element(mkm => { called = true;
			                      	mkm.Should().Not.Be.Null(); });

			called.Should().Be.True();
		}

		[Test]
		public void WhenAssignElementRelationThenActionShouldReuseSameInstanceOfMapper()
		{
			var keyType = typeof(string);
			var hbmMap = new HbmMap();
			var hbmMapping = new HbmMapping();
			var mapper = new MapKeyRelation(keyType, hbmMap, hbmMapping);
			IMapKeyMapper parameterCall1= null;
			IMapKeyMapper parameterCall2= null;
			mapper.Element(mkm =>
			{
				parameterCall1 = mkm;
			});
			mapper.Element(mkm =>
			{
				parameterCall2 = mkm;
			});

			parameterCall1.Should().Be.SameInstanceAs(parameterCall2);
		}

		[Test]
		public void WhenAssignElementRelationThenActionShouldReuseSameInstanceOfHbmElement()
		{
			var keyType = typeof(string);
			var hbmMap = new HbmMap();
			var hbmMapping = new HbmMapping();
			var mapper = new MapKeyRelation(keyType, hbmMap, hbmMapping);
			mapper.Element(mkm => mkm.Column("pizza"));
			mapper.Element(mkm => mkm.Length(30));

			var keyElement = (HbmMapKey)hbmMap.Item;
			keyElement.length.Should().Be("30");
			keyElement.column.Should().Be("pizza");
		}

		[Test]
		public void WhenAssignManyToManyRelationThenUseHbmMapKeyManyToManyElement()
		{
			var keyType = typeof(string);
			var hbmMap = new HbmMap();
			var hbmMapping = new HbmMapping();
			var mapper = new MapKeyRelation(keyType, hbmMap, hbmMapping);
			mapper.ManyToMany(mkm => { });

			hbmMap.Item.Should().Be.OfType<HbmMapKeyManyToMany>();
		}

		[Test]
		public void WhenAssignManyToManyRelationThenCallActionWithNotNullParameter()
		{
			var keyType = typeof(string);
			var hbmMap = new HbmMap();
			var hbmMapping = new HbmMapping();
			var mapper = new MapKeyRelation(keyType, hbmMap, hbmMapping);
			var called = false;
			mapper.ManyToMany(mkm =>
			{
				called = true;
				mkm.Should().Not.Be.Null();
			});

			called.Should().Be.True();
		}

		[Test]
		public void WhenAssignManyToManyRelationThenActionShouldReuseSameInstanceOfMapper()
		{
			var keyType = typeof(string);
			var hbmMap = new HbmMap();
			var hbmMapping = new HbmMapping();
			var mapper = new MapKeyRelation(keyType, hbmMap, hbmMapping);
			IMapKeyManyToManyMapper parameterCall1 = null;
			IMapKeyManyToManyMapper parameterCall2 = null;
			mapper.ManyToMany(mkm =>
			{
				parameterCall1 = mkm;
			});
			mapper.ManyToMany(mkm =>
			{
				parameterCall2 = mkm;
			});

			parameterCall1.Should().Be.SameInstanceAs(parameterCall2);
		}

		[Test]
		public void WhenAssignManyToManyRelationActionShouldReuseSameInstanceOfHbmElement()
		{
			var keyType = typeof(string);
			var hbmMap = new HbmMap();
			var hbmMapping = new HbmMapping();
			var mapper = new MapKeyRelation(keyType, hbmMap, hbmMapping);
			mapper.ManyToMany(mkm => mkm.Column("pizza"));
			mapper.ManyToMany(mkm => mkm.ForeignKey("FK"));

			var keyElement = (HbmMapKeyManyToMany)hbmMap.Item;
			keyElement.foreignkey.Should().Be("FK");
			keyElement.column.Should().Be("pizza");
		}

		[Test]
		public void WhenAssignManyToManyRelationThenAssignClass()
		{
			var keyType = typeof(MyClass);
			var hbmMap = new HbmMap();
			var hbmMapping = new HbmMapping();
			var mapper = new MapKeyRelation(keyType, hbmMap, hbmMapping);
			mapper.ManyToMany(mkm => { });

			var keyElement = (HbmMapKeyManyToMany)hbmMap.Item;
			keyElement.Class.Should().Not.Be.Null();
			keyElement.Class.Should().Contain("MyClass");
		}

		[Test]
		public void WhenAssignComponentRelationThenUseHbmCompositeMapKeyElement()
		{
			var keyType = typeof(string);
			var hbmMap = new HbmMap();
			var hbmMapping = new HbmMapping();
			var mapper = new MapKeyRelation(keyType, hbmMap, hbmMapping);
			mapper.Component(mkm => { });

			hbmMap.Item.Should().Be.OfType<HbmCompositeMapKey>();
		}

		[Test]
		public void WhenAssignComponentRelationThenCallActionWithNotNullParameter()
		{
			var keyType = typeof(string);
			var hbmMap = new HbmMap();
			var hbmMapping = new HbmMapping();
			var mapper = new MapKeyRelation(keyType, hbmMap, hbmMapping);
			var called = false;
			mapper.Component(mkm =>
			{
				called = true;
				mkm.Should().Not.Be.Null();
			});

			called.Should().Be.True();
		}

		[Test]
		public void WhenAssignComponentRelationThenActionShouldReuseSameInstanceOfMapper()
		{
			var keyType = typeof(string);
			var hbmMap = new HbmMap();
			var hbmMapping = new HbmMapping();
			var mapper = new MapKeyRelation(keyType, hbmMap, hbmMapping);
			IComponentMapKeyMapper parameterCall1 = null;
			IComponentMapKeyMapper parameterCall2 = null;
			mapper.Component(mkm =>
			{
				parameterCall1 = mkm;
			});
			mapper.Component(mkm =>
			{
				parameterCall2 = mkm;
			});

			parameterCall1.Should().Be.SameInstanceAs(parameterCall2);
		}

		[Test]
		public void WhenAssignComponentRelationThenAssignClass()
		{
			var keyType = typeof(MyClass);
			var hbmMap = new HbmMap();
			var hbmMapping = new HbmMapping();
			var mapper = new MapKeyRelation(keyType, hbmMap, hbmMapping);
			mapper.Component(mkm => { });

			var keyElement = (HbmCompositeMapKey)hbmMap.Item;
			keyElement.Class.Should().Not.Be.Null();
			keyElement.Class.Should().Contain("MyClass");
		}
	}
}