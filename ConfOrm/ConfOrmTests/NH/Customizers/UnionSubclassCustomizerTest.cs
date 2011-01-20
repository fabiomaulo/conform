using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.NH.CustomizersImpl;
using Moq;
using NHibernate.Persister.Entity;
using NUnit.Framework;

namespace ConfOrmTests.NH.Customizers
{
	public class UnionSubclassCustomizerTest
	{
		private class MyClass
		{

		}

		[Test]
		public void InvokeSetOfPersister()
		{
			var customizersHolder = new CustomizersHolder();
			var customizer = new UnionSubclassCustomizer<MyClass>(customizersHolder);
			var classMapper = new Mock<IUnionSubclassAttributesMapper>();

			customizer.Persister<UnionSubclassEntityPersister>();
			customizersHolder.InvokeCustomizers(typeof(MyClass), classMapper.Object);

			classMapper.Verify(x => x.Persister<UnionSubclassEntityPersister>());
		}
	}
}