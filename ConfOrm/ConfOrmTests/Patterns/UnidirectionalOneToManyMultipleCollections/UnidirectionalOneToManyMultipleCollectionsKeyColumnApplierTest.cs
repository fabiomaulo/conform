using System;
using System.Linq;
using System.Collections.Generic;
using ConfOrm;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Patterns;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns.UnidirectionalOneToManyMultipleCollections
{
	public class UnidirectionalOneToManyMultipleCollectionsKeyColumnApplierTest
	{
		public class MySingleUsage
		{
			public int Id { get; set; }
			public IEnumerable<JobRecord> CurrentPositions { get; set; }
		}

		public class Contact 
		{
			public int Id { get; set; }
			public IEnumerable<JobRecord> CurrentPositions { get; set; }
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
			orm.Setup(
				dm =>
				dm.IsEntity(It.Is<Type>(t => (new[] { typeof(MySingleUsage), typeof(Contact), typeof(JobRecord) }).Contains(t)))).
				Returns(true);
			orm.Setup(dm => dm.IsOneToMany(It.Is<Type>(t => (new[] { typeof(MySingleUsage), typeof(Contact) }).Contains(t)), typeof(JobRecord))).Returns(true);
			return orm;
		}

		[Test]
		public void WhenSingleUsageThenNoMatch()
		{
			var orm = GetDomainInspectorMockForBaseTests();

			var applier = new UnidirectionalOneToManyMultipleCollectionsKeyColumnApplier(orm.Object);
			var property = new PropertyPath(null, ForClass<MySingleUsage>.Property(x => x.CurrentPositions));
			applier.Match(property).Should().Be.False();
		}

		[Test]
		public void WhenMultipleUsageThenMatch()
		{
			var orm = GetDomainInspectorMockForBaseTests();

			var applier = new UnidirectionalOneToManyMultipleCollectionsKeyColumnApplier(orm.Object);
			var property1 = new PropertyPath(null, ForClass<Contact>.Property(x => x.CurrentPositions));
			applier.Match(property1).Should().Be.True();

			var property2 = new PropertyPath(null, ForClass<Contact>.Property(x => x.PastPositions));
			applier.Match(property2).Should().Be.True();
		}

		[Test]
		public void ApplyPropertyPathAndPostFixForCurrentPositions()
		{
			var orm = new Mock<IDomainInspector>();
			var applier = new UnidirectionalOneToManyMultipleCollectionsKeyColumnApplier(orm.Object);
			var collectionMapper = new Mock<ICollectionPropertiesMapper>();
			var keyMapper = new Mock<IKeyMapper>();
			collectionMapper.Setup(x => x.Key(It.IsAny<Action<IKeyMapper>>())).Callback<Action<IKeyMapper>>(
				x => x.Invoke(keyMapper.Object));

			var property = new PropertyPath(null, ForClass<Contact>.Property(x => x.CurrentPositions));
			applier.Apply(property, collectionMapper.Object);

			keyMapper.Verify(km => km.Column(It.Is<string>(s => s == "ContactCurrentPositions_key")));
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

			var property = new PropertyPath(null, ForClass<Contact>.Property(x => x.PastPositions));
			applier.Apply(property, collectionMapper.Object);

			keyMapper.Verify(km => km.Column(It.Is<string>(s => s == "ContactPastPositions_key")));
		}
	}
}