using System.Linq;
using ConfOrm.NH;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class DefaultCandidatePersistentMembersProviderTest
	{
		private class Entity
		{
			private int poid;
			public int Id { get; set; }
		}
		private class VersionedEntity : Entity
		{
			public int Version { get; set; }
		}
		private class MyEntity : VersionedEntity
		{
			private string aField;
			public string Name { get; set; }
		}
		private class MyInheritedEntityLevel1: MyEntity
		{
			public int PropOfLevel1 { get; set; }
		}
		private class MyInheritedEntityLevel2 : MyInheritedEntityLevel1
		{
			public int PropOfLevel2 { get; set; }
		}

		private interface IEntity<T>
		{
			T Id { get; }
		}

		private interface IVersionedEntity : IEntity<int>
		{
			int Version { get; }
		}

		private interface IMyEntity : IVersionedEntity
		{
			string Description { get; set; }
		}

		private class MyBaseComponent
		{
			public string Something { get; set; }
		}

		private class MyComponent : MyBaseComponent
		{
			public string SomethingElse { get; set; }
		}

		[Test]
		public void WhenRootClassThenShouldIncludePropertiesOfSuperClasses()
		{
			var memberProvider = new DefaultCandidatePersistentMembersProvider();
			var properties = memberProvider.GetRootEntityMembers(typeof (MyEntity));
			properties.Should().Have.Count.EqualTo(3);
			properties.Select(p => p.Name).Should().Have.SameValuesAs("Id", "Version", "Name");
		}

		[Test]
		public void WhenClassForPoidThenShouldIncludePropertiesAndFiedsOfSuperClasses()
		{
			var memberProvider = new DefaultCandidatePersistentMembersProvider();
			var properties = memberProvider.GetEntityMembersForPoid(typeof(MyEntity)).ToArray();
			// the Intersect is to check only fields/properties I'm interested in this test
			properties.Select(p => p.Name).Intersect(new[]{"poid", "Id", "Version", "Name", "aField"})
				.Should().Have.SameValuesAs("poid", "Id", "Version", "Name", "aField");
		}

		[Test]
		public void WhenInterfaceForPoidThenShouldIncludePropertiesOfSuperInterfaces()
		{
			var memberProvider = new DefaultCandidatePersistentMembersProvider();
			var properties = memberProvider.GetEntityMembersForPoid(typeof(IMyEntity));
			properties.Should().Have.Count.EqualTo(3);
			properties.Select(p => p.Name).Should().Have.SameValuesAs("Id", "Version", "Description");
		}

		[Test]
		public void WhenRootClassIsInterfaceThenShouldIncludePropertiesOfSuperInterfaces()
		{
			var memberProvider = new DefaultCandidatePersistentMembersProvider();
			var properties = memberProvider.GetRootEntityMembers(typeof(IMyEntity));
			properties.Should().Have.Count.EqualTo(3);
			properties.Select(p => p.Name).Should().Have.SameValuesAs("Id", "Version", "Description");
		}

		[Test]
		public void WhenSubclassThenShouldIncludePropertiesOfSubClassAndSkipedSuperClasses()
		{
			var memberProvider = new DefaultCandidatePersistentMembersProvider();
			var properties = memberProvider.GetSubEntityMembers(typeof(MyInheritedEntityLevel2), typeof(MyEntity));
			properties.Should().Have.Count.EqualTo(2);
			properties.Select(p => p.Name).Should().Have.SameValuesAs("PropOfLevel2", "PropOfLevel1");
		}

		[Test]
		public void WhenSubclassThenShouldIncludePropertiesOnlyOfSubClassIfNoSkipedSuperClasses()
		{
			var memberProvider = new DefaultCandidatePersistentMembersProvider();
			var properties = memberProvider.GetSubEntityMembers(typeof(MyInheritedEntityLevel2), typeof(MyInheritedEntityLevel1));
			properties.Should().Have.Count.EqualTo(1);
			properties.Select(p => p.Name).Should().Have.SameValuesAs("PropOfLevel2");
		}

		[Test]
		public void WhenComponentThenShouldIncludePropertiesOfSuperClasses()
		{
			var memberProvider = new DefaultCandidatePersistentMembersProvider();
			var properties = memberProvider.GetComponentMembers(typeof(MyComponent));
			properties.Should().Have.Count.EqualTo(2);
			properties.Select(p => p.Name).Should().Have.SameValuesAs("Something", "SomethingElse");
		}
	}
}