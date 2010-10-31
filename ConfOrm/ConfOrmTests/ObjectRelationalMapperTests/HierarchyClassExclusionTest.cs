using System;
using ConfOrm;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.ObjectRelationalMapperTests
{
	public class HierarchyClassExclusionTest
	{
		private interface IEntity
		{
			int Id { get; set; }

		}
		private interface IMyEntity : IEntity
		{
			string Name { get; set; }
		}
		private class BaseEntity : IEntity
		{
			public int Id { get; set; }
		}
		private class ToExcludeImplEntity : BaseEntity, IMyEntity
		{
			#region Implementation of IMyEntity

			public string Name
			{
				get { throw new NotImplementedException(); }
				set { throw new NotImplementedException(); }
			}

			#endregion
		}
		private class ValidImplEntity : ToExcludeImplEntity
		{
		}

		[Test]
		public void WhenExplicitExcludedThenNotIncludeInHierarchy()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<IMyEntity>();
			orm.Exclude<ToExcludeImplEntity>();

			orm.IsRootEntity(typeof(IMyEntity)).Should().Be.True();
			orm.IsRootEntity(typeof(ToExcludeImplEntity)).Should().Be.False();

			orm.IsEntity(typeof(IMyEntity)).Should().Be.True();
			orm.IsEntity(typeof(ValidImplEntity)).Should().Be.True();
			orm.IsEntity(typeof(ToExcludeImplEntity)).Should().Be.False();
		}

		[Test]
		public void WhenExplicitExcludedThenNotRootEntity()
		{
			// To prevent inconsistence
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<ToExcludeImplEntity>();
			orm.Exclude<ToExcludeImplEntity>();

			orm.IsRootEntity(typeof(ToExcludeImplEntity)).Should().Be.False();
		}

		[Test]
		public void WhenExplicitExcludedThenNotTablePerClass()
		{
			// To prevent inconsistence
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<ToExcludeImplEntity>();
			orm.Exclude<ToExcludeImplEntity>();

			orm.IsTablePerClass(typeof(ToExcludeImplEntity)).Should().Be.False();
		}

		[Test]
		public void WhenExplicitExcludedThenNotTablePerClassHierarchy()
		{
			// To prevent inconsistence
			var orm = new ObjectRelationalMapper();
			orm.TablePerClassHierarchy<ToExcludeImplEntity>();
			orm.Exclude<ToExcludeImplEntity>();

			orm.IsTablePerClassHierarchy(typeof(ToExcludeImplEntity)).Should().Be.False();
		}

		[Test]
		public void WhenExplicitExcludedThenNotTablePerConcreteClass()
		{
			// To prevent inconsistence
			var orm = new ObjectRelationalMapper();
			orm.TablePerConcreteClass<ToExcludeImplEntity>();
			orm.Exclude<ToExcludeImplEntity>();

			orm.IsTablePerConcreteClass(typeof(ToExcludeImplEntity)).Should().Be.False();
		}
	}
}