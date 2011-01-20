using System;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.NH.CustomizersImpl;
using Moq;
using NHibernate.Persister.Entity;
using NUnit.Framework;

namespace ConfOrmTests.NH.Customizers
{
	public class ClassCustomizerTest
	{
		private class MyClass
		{
			
		}

		[Test]
		public void InvokeFilterMapping()
		{
			var customizersHolder = new CustomizersHolder();
			var customizer = new ClassCustomizer<MyClass>(customizersHolder);
			var classMapper = new Mock<IClassAttributesMapper>();
			var filterMapper = new Mock<IFilterMapper>();
			classMapper.Setup(x => x.Filter(It.IsAny<string>(), It.IsAny<Action<IFilterMapper>>())).Callback<string, Action<IFilterMapper>>(
				(name, x) => x.Invoke(filterMapper.Object));

			customizer.Filter("pizza",x => x.Condition("any condition"));
			customizersHolder.InvokeCustomizers(typeof(MyClass), classMapper.Object);

			filterMapper.Verify(x => x.Condition(It.Is<string>(v => v == "any condition")));
		}

		[Test]
		public void InvokeSetOfSchemaAction()
		{
			var customizersHolder = new CustomizersHolder();
			var customizer = new ClassCustomizer<MyClass>(customizersHolder);
			var classMapper = new Mock<IClassAttributesMapper>();

			customizer.SchemaAction(SchemaAction.None);
			customizersHolder.InvokeCustomizers(typeof(MyClass), classMapper.Object);

			classMapper.Verify(x => x.SchemaAction(SchemaAction.None));
		}

		[Test]
		public void InvokeSetOfPersister()
		{
			var customizersHolder = new CustomizersHolder();
			var customizer = new ClassCustomizer<MyClass>(customizersHolder);
			var classMapper = new Mock<IClassAttributesMapper>();

			customizer.Persister<SingleTableEntityPersister>();
			customizersHolder.InvokeCustomizers(typeof(MyClass), classMapper.Object);

			classMapper.Verify(x => x.Persister<SingleTableEntityPersister>());
		}
	}
}