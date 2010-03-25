using ConfOrm;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.ObjectRelationalMapperTests
{
	public class HeterogeneousAssociationTest
	{
		private class MyClass
		{
			public int Id { get; set; }
			public object MyReferenceClass { get; set; }
		}

		private class MyReferenceClass
		{
			public int Id { get; set; }
		}

		[Test]
		public void ExplicitDeclaration()
		{
			var orm = new ObjectRelationalMapper();
			orm.HeterogeneousAssociation<MyClass>(mc => mc.MyReferenceClass);

			orm.IsHeterogeneousAssociation(typeof(MyClass).GetProperty("MyReferenceClass")).Should().Be.True();
		}
	}
}