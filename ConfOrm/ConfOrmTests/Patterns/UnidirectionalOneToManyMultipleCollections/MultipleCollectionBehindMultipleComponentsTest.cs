using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using ConfOrm;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Patterns;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns.UnidirectionalOneToManyMultipleCollections
{
	public class MultipleCollectionBehindMultipleComponentsTest
	{
		public class Contact
		{
			public int Id { get; set; }
			public MyComponent Component1 { get; set; }
			public MyComponent Component2 { get; set; }
		}

		public class MyComponent
		{
			public IEnumerable<JobRecord> PastPositions { get; set; }			
		}

		public class JobRecord
		{
			public int Id { get; set; }
			public string Position { get; set; }
		}

		private Mock<IDomainInspector> GetDomainInspectorMockForBaseTests()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsPersistentProperty(It.IsAny<MemberInfo>())).Returns(true);
			orm.Setup(
				dm =>
				dm.IsEntity(It.Is<Type>(t => (new[] { typeof(Contact), typeof(JobRecord) }).Contains(t)))).Returns(true);
			orm.Setup(dm => dm.IsComponent(typeof (MyComponent))).Returns(true);
			orm.Setup(dm => dm.IsOneToMany(typeof(Contact), typeof(JobRecord))).Returns(true);
			orm.Setup(dm => dm.IsOneToMany(typeof(MyComponent), typeof(JobRecord))).Returns(true);
			return orm;
		}

		[Test]
		public void WhenMultipleUsageBehindAComponentThenMatch()
		{
			var orm = GetDomainInspectorMockForBaseTests();

			var applier = new UnidirectionalOneToManyMultipleCollectionsKeyColumnApplier(orm.Object);
			var componentProperty = new PropertyPath(null, ForClass<Contact>.Property(x => x.Component1));
			var property = new PropertyPath(componentProperty, ForClass<MyComponent>.Property(x => x.PastPositions));
			applier.Match(property).Should().Be.True();
		}

		[Test]
		public void ApplyPropertyPathAndPostFixForPastPositions()
		{
			var orm = new Mock<IDomainInspector>();
			var applier = new UnidirectionalOneToManyMultipleCollectionsKeyColumnApplier(orm.Object);
			var collectionMapper = new Mock<ICollectionPropertiesMapper>();
			var keyMapper = new Mock<IKeyMapper>();
			collectionMapper.Setup(x => x.Key(It.IsAny<Action<IKeyMapper>>())).Callback<Action<IKeyMapper>>(
				x => x.Invoke(keyMapper.Object));

			var componentProperty = new PropertyPath(null, ForClass<Contact>.Property(x => x.Component1));
			var property = new PropertyPath(componentProperty, ForClass<MyComponent>.Property(x => x.PastPositions));
			applier.Apply(property, collectionMapper.Object);

			keyMapper.Verify(km => km.Column(It.Is<string>(s => s == "ContactComponent1PastPositions_key")));
		}
	}
}