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
			Executing.This(() => new Mapper(null)).Should().Throw<ArgumentNullException>();
			var inspector = new Mock<IDomainInspector>();
			Executing.This(() => new Mapper(inspector.Object)).Should().NotThrow();
		}

		[Test]
		public void WhenCompileNullListThrows()
		{
			var inspector = new Mock<IDomainInspector>();
			var mapper=  new Mapper(inspector.Object);
			Executing.This(() => mapper.CompileMappingFor(null)).Should().Throw<ArgumentNullException>();
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