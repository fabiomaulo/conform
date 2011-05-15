using NHibernate.Mapping.ByCode;
using ConfOrm.NH;
using ConfOrm.NH.CustomizersImpl;
using Moq;
using NHibernate.Persister.Entity;
using NUnit.Framework;

namespace ConfOrmTests.NH.Customizers
{
	public class SubclassCustomizerTest
	{
		private class MyClass
		{

		}

		[Test]
		public void InvokeSetOfPersister()
		{
			var customizersHolder = new CustomizersHolder();
			var customizer = new SubclassCustomizer<MyClass>(customizersHolder);
			var classMapper = new Mock<ISubclassMapper>();

			customizer.Persister<SingleTableEntityPersister>();
			customizersHolder.InvokeCustomizers(typeof(MyClass), classMapper.Object);

			classMapper.Verify(x => x.Persister<SingleTableEntityPersister>());
		}
	}
}