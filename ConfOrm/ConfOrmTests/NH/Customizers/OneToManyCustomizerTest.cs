using System;
using System.Collections.Generic;
using NHibernate.Mapping.ByCode;
using ConfOrm.NH;
using ConfOrm.NH.CustomizersImpl;
using Moq;
using NUnit.Framework;

namespace ConfOrmTests.NH.Customizers
{
	public class OneToManyCustomizerTest
	{
		private class MyClass
		{
			public IEnumerable<IRelated> MyCollection { get; set; }
		}
		private interface IRelated
		{

		}

		private class Related: IRelated
		{
			
		}

		[Test]
		public void InvokeDirectMethods()
		{
			var propertyPath = new PropertyPath(null, ConfOrm.ForClass<MyClass>.Property(x => x.MyCollection));
			var customizersHolder = new CustomizersHolder();
			var customizer = new OneToManyCustomizer(propertyPath, customizersHolder);
			var elementMapper = new Mock<IOneToManyMapper>();

			customizer.Class(typeof (Related));
			customizer.EntityName("something");
			customizer.NotFound(NotFoundMode.Ignore);

			customizersHolder.InvokeCustomizers(propertyPath, elementMapper.Object);

			elementMapper.Verify(x => x.Class(It.Is<Type>(v => v == typeof (Related))), Times.Once());
			elementMapper.Verify(x => x.EntityName(It.Is<string>(v => v == "something")), Times.Once());
			elementMapper.Verify(x => x.NotFound(It.Is<NotFoundMode>(v => v == NotFoundMode.Ignore)), Times.Once());
		}
	}
}