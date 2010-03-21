using System.Reflection;
using ConfOrm;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.ObjectRelationalMapperTests
{
	public class VersionTest
	{
		private class MyClass
		{
			public int Id { get; set; }
			public int Version { get; set; }
		}

		[Test]
		public void WhenExplicitDeclaredThenRecognizeVerion()
		{
			var orm = new ObjectRelationalMapper();
			orm.VersionProperty<MyClass>(myclass => myclass.Version);
			
			orm.IsVersion(typeof (MyClass).GetProperty("Version")).Should().Be.True();
		}

		[Test]
		public void WhenPatternMatchThenRecognizeVerion()
		{
			var versionPropertyInfo = typeof(MyClass).GetProperty("Version");
			var orm = new ObjectRelationalMapper();
			var versionPattern = new Mock<IPattern<MemberInfo>>();
			versionPattern.Setup(vp => vp.Match(It.Is<MemberInfo>(mi => mi == versionPropertyInfo))).Returns(true);
			
			orm.Patterns.Versions.Add(versionPattern.Object);

			orm.IsVersion(versionPropertyInfo).Should().Be.True();
		}
	}
}