using System.Collections.Generic;
using NHibernate.Mapping.ByCode;
using ConfOrm.NH;
using ConfOrm.Shop.InflectorNaming;
using ConfOrm.Shop.Inflectors;
using Moq;
using NUnit.Framework;

namespace ConfOrm.ShopTests.InflectorNamingTests
{
	public class CollectionOfElementsColumnApplierTest
	{
		private class Person
		{
			public IEnumerable<string> Addresses { get; set; }
		}

		[Test]
		public void WhenApplyThenCallInflector()
		{
			var orm = new Mock<IDomainInspector>();
			var inflector = new Mock<IInflector>();
			inflector.Setup(i => i.Singularize("Addresses")).Returns("Address");
			var applier = new CollectionOfElementsColumnApplier(orm.Object, inflector.Object);
			var mapper = new Mock<IElementMapper>();
			var path = new PropertyPath(null, ForClass<Person>.Property(p => p.Addresses));

			applier.Apply(path, mapper.Object);

			mapper.Verify(m => m.Column("Address"));
		}
	}
}