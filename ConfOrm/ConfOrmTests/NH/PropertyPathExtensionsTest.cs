using ConfOrm;
using ConfOrm.NH;
using Moq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class PropertyPathExtensionsTest
	{
		private class MainEntity
		{
			public int Id { get; set; }
		}
		private class InheritedEntity
		{
			public MyComponent Component { get; set; }
		}

		private class MyComponent
		{
			public int Something { get; set; }
			public MyComponent1 Component1 { get; set; }
		}

		private class MyComponent1
		{
			public int Something { get; set; }
		}

		[Test]
		public void GetContainerEntityWhenPropertyIsInComponentThenReturnEntity()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsEntity(typeof(MainEntity))).Returns(true);
			orm.Setup(x => x.IsRootEntity(typeof(MainEntity))).Returns(true);
			orm.Setup(x => x.IsEntity(typeof(InheritedEntity))).Returns(true);
			var level0 = new PropertyPath(null, ConfOrm.ForClass<InheritedEntity>.Property(p => p.Component));
			var path = new PropertyPath(level0, ConfOrm.ForClass<MyComponent>.Property(p => p.Something));

			path.GetContainerEntity(orm.Object).Should().Be(typeof (InheritedEntity));
		}

		[Test]
		public void GetContainerEntityWhenPropertyIsInDeppComponentThenReturnEntity()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsEntity(typeof(MainEntity))).Returns(true);
			orm.Setup(x => x.IsRootEntity(typeof(MainEntity))).Returns(true);
			orm.Setup(x => x.IsEntity(typeof(InheritedEntity))).Returns(true);
			var level0 = new PropertyPath(null, ConfOrm.ForClass<InheritedEntity>.Property(p => p.Component));
			var level1 = new PropertyPath(level0, ConfOrm.ForClass<MyComponent>.Property(p => p.Component1));
			var path = new PropertyPath(level1, ConfOrm.ForClass<MyComponent1>.Property(p => p.Something));

			path.GetContainerEntity(orm.Object).Should().Be(typeof(InheritedEntity));
		}

		[Test]
		public void ToColumnNameWithCustomSeparator()
		{
			var level0 = new PropertyPath(null, ConfOrm.ForClass<InheritedEntity>.Property(p => p.Component));
			var level1 = new PropertyPath(level0, ConfOrm.ForClass<MyComponent>.Property(p => p.Component1));
			var path = new PropertyPath(level1, ConfOrm.ForClass<MyComponent1>.Property(p => p.Something));
			path.ToColumnName("_").Should().Be("Component_Component1_Something");
		}

		[Test]
		public void WhenCustomSeparatorIsNullThenConcat()
		{
			var level0 = new PropertyPath(null, ConfOrm.ForClass<InheritedEntity>.Property(p => p.Component));
			var level1 = new PropertyPath(level0, ConfOrm.ForClass<MyComponent>.Property(p => p.Component1));
			var path = new PropertyPath(level1, ConfOrm.ForClass<MyComponent1>.Property(p => p.Something));
			path.ToColumnName(null).Should().Be("ComponentComponent1Something");
		}
	}
}