using System;
using System.Linq;
using System.Collections.Generic;
using ConfOrm;
using ConfOrm.Patterns;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.Patterns
{
	public class UnidirectionalOneToManyMemberPatternTest
	{
		private class MyClass
		{
			public string Something { get; set; }
			public IEnumerable<Related> Relateds { get; set; }
			public IEnumerable<Bidirectional> Children { get; set; }
			public IEnumerable<Component> Components { get; set; }
			public IEnumerable<string > Elements { get; set; }
			public IDictionary<string, Related> DicRelateds { get; set; }
			public IDictionary<string, Bidirectional> DicChildren { get; set; }
		}

		private class Related
		{
			
		}

		private class Bidirectional
		{
			public MyClass MyClass { get; set; }
		}

		private class Component
		{
		}

		[Test]
		public void CtorProtection()
		{
			Executing.This(() => new UnidirectionalOneToManyMemberPattern(null)).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void WhenNoPropertyThenNoMatch()
		{
			var orm = new Mock<IDomainInspector>();
			var pattern = new UnidirectionalOneToManyMemberPattern(orm.Object);
			pattern.Match(null).Should().Be.False();
		}

		[Test]
		public void WhenNoCollectionPropertyThenNoMatch()
		{
			var orm = GetDomainInspectorMock();
			var pattern = new UnidirectionalOneToManyMemberPattern(orm.Object);
			pattern.Match(ForClass<MyClass>.Property(mc=> mc.Something)).Should().Be.False();
		}

		[Test]
		public void WhenCollectionOfComponentsThenNoMatch()
		{
			var orm = GetDomainInspectorMock();

			var pattern = new UnidirectionalOneToManyMemberPattern(orm.Object);
			pattern.Match(ForClass<MyClass>.Property(mc => mc.Components)).Should().Be.False();
		}

		private Mock<IDomainInspector> GetDomainInspectorMock()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(
				dm =>
				dm.IsEntity(It.Is<Type>(t => (new[] {typeof (MyClass), typeof (Related), typeof (Bidirectional)}).Contains(t)))).Returns(true);
			orm.Setup(dm =>dm.IsComponent(It.Is<Type>(t =>t == typeof(Component)))).Returns(true);
			orm.Setup(dm => dm.IsOneToMany(typeof(MyClass), It.Is<Type>(t => (new[] { typeof(Related), typeof(Bidirectional) }).Contains(t)))).Returns(true);
			return orm;
		}

		[Test]
		public void WhenCollectionBidirectionalThenNoMatch()
		{
			var orm = GetDomainInspectorMock();
			var pattern = new UnidirectionalOneToManyMemberPattern(orm.Object);
			pattern.Match(ForClass<MyClass>.Property(mc => mc.Children)).Should().Be.False();
		}

		[Test]
		public void WhenCollectionOfElementsThenNoMatch()
		{
			var orm = GetDomainInspectorMock();
			var pattern = new UnidirectionalOneToManyMemberPattern(orm.Object);
			pattern.Match(ForClass<MyClass>.Property(mc => mc.Elements)).Should().Be.False();
		}

		[Test]
		public void WhenCollectionUnidirectionalThenMatch()
		{
			var orm = GetDomainInspectorMock();

			var pattern = new UnidirectionalOneToManyMemberPattern(orm.Object);
			pattern.Match(ForClass<MyClass>.Property(mc => mc.Relateds)).Should().Be.True();
		}

		[Test]
		public void WhenDictionaryBidirectionalThenNoMatch()
		{
			var orm = GetDomainInspectorMock();
			var pattern = new UnidirectionalOneToManyMemberPattern(orm.Object);
			pattern.Match(ForClass<MyClass>.Property(mc => mc.DicChildren)).Should().Be.False();
		}

		[Test]
		public void WhenDictionaryUnidirectionalThenMatch()
		{
			var orm = GetDomainInspectorMock();

			var pattern = new UnidirectionalOneToManyMemberPattern(orm.Object);
			pattern.Match(ForClass<MyClass>.Property(mc => mc.DicRelateds)).Should().Be.True();
		}
	}
}