using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.NH.CustomizersImpl;
using Moq;
using NHibernate.Persister.Entity;
using NUnit.Framework;

namespace ConfOrmTests.NH.Customizers
{
	public class JoinedSubclassCustomizerTest
	{
		private class MyClass
		{

		}

		[Test]
		public void InvokeSetOfPersister()
		{
			var customizersHolder = new CustomizersHolder();
			var customizer = new JoinedSubclassCustomizer<MyClass>(customizersHolder);
			var classMapper = new Mock<IJoinedSubclassAttributesMapper>();

			customizer.Persister<JoinedSubclassEntityPersister>();
			customizersHolder.InvokeCustomizers(typeof(MyClass), classMapper.Object);

			classMapper.Verify(x => x.Persister<JoinedSubclassEntityPersister>());
		}
	}
}