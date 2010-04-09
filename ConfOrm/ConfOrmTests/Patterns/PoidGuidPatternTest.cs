using System;
using ConfOrm.Patterns;
using NUnit.Framework;
using TypeExtensions = ConfOrm.TypeExtensions;
using SharpTestsEx;

namespace ConfOrmTests.Patterns
{
	public class PoidGuidPatternTest
	{
		private interface IEntity<TIdentity>
		{
			TIdentity GuidProp { get; }
		}

		private interface IMyEntity : IEntity<Guid>
		{
			
		}

		private class MyClass
		{
			public Guid GuidProp { get; set; }
			public string StringProp { get; set; }
			public object ObjectProp { get; set; }
		}

		[Test]
		public void MatchWithGuidMember()
		{
			var pattern = new PoidGuidPattern();
			pattern.Match(TypeExtensions.DecodeMemberAccessExpression<MyClass>(m => m.GuidProp)).Should().Be(true);
		}

		[Test]
		public void NoMatchWithOthersTypes()
		{
			var pattern = new PoidGuidPattern();
			pattern.Match(TypeExtensions.DecodeMemberAccessExpression<MyClass>(m => m.StringProp)).Should().Be(false);
			pattern.Match(TypeExtensions.DecodeMemberAccessExpression<MyClass>(m => m.ObjectProp)).Should().Be(false);
		}

		[Test]
		public void NoMatchWithNullMember()
		{
			var pattern = new PoidGuidPattern();
			pattern.Match(null).Should().Be(false);
		}

		[Test]
		public void MatchWithGuidMemberOfInheritedInterface()
		{
			var pattern = new PoidGuidPattern();
			pattern.Match(TypeExtensions.DecodeMemberAccessExpression<IMyEntity>(m => m.GuidProp)).Should().Be(true);
			pattern.Match(TypeExtensions.DecodeMemberAccessExpressionOf<IMyEntity>(m => m.GuidProp)).Should().Be(true);
		}
	}
}