using System;
using ConfOrm;
using ConfOrm.NH;
using NUnit.Framework;
using Moq;
using SharpTestsEx;

namespace ConfOrmTests.NH.MapperTests
{
	public class BaseFuncionality
	{
		[Test]
		public void CtorProtection()
		{
			ActionAssert.Throws<ArgumentNullException>(() => new Mapper(null));
			var inspector = new Mock<IDomainInspector>();
			ActionAssert.NotThrow(() => new Mapper(inspector.Object));
		}

		[Test]
		public void WhenCompileNullListThrows()
		{
			var inspector = new Mock<IDomainInspector>();
			var mapper=  new Mapper(inspector.Object);
			ActionAssert.Throws<ArgumentNullException>(() => mapper.CompileMappingFor(null));
		}

		[Test]
		public void CompilingEmptyListReturnEmptyMapping()
		{
			var inspector = new Mock<IDomainInspector>();
			var mapper=  new Mapper(inspector.Object);
			mapper.CompileMappingFor(new Type[0]).Should().Not.Be.Null();
		}
	}
}