using System.Linq;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH
{
	public class AnyMapperTest
	{
		private class MyClass
		{
			public int Id { get; set; }
			public MyReferenceClass MyReferenceClass { get; set; }
		}

		private class MyReferenceClass
		{
			public int Id { get; set; }			
		}

		[Test]
		public void AtCreationSetIdType()
		{
			var hbmAny = new HbmAny();
			new AnyMapper(null, typeof (int), hbmAny);
			hbmAny.idtype.Should().Be("Int32");
		}

		[Test]
		public void AtCreationSetTheTwoRequiredColumnsNodes()
		{
			var hbmAny = new HbmAny();
			new AnyMapper(null, typeof(int), hbmAny);
			hbmAny.Columns.Should().Have.Count.EqualTo(2);
			hbmAny.Columns.Select(c => c.name).All(n => n.Satisfy(name=> !string.IsNullOrEmpty(name)));
		}
	}
}