using System;
using System.Collections.Generic;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Shop.CoolNaming;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrm.ShopTests.CoolNamingsAppliersTests
{
	public class ListIndexAsPropertyPosColumnNameApplierTest
	{
		private class MyClass
		{
			public IList<int> Numbers { get; set; }
		}

		[Test]
		public void MatchAnyValidPropertyPath()
		{
			var applier = new ListIndexAsPropertyPosColumnNameApplier();
			applier.Match(null).Should().Be.False();
			var ppath = new PropertyPath(null, ForClass<MyClass>.Property(mc => mc.Numbers));
			applier.Match(ppath).Should().Be.True();
		}

		[Test]
		public void ApplyPropertyPathWithPosPostfix()
		{
			var applier = new ListIndexAsPropertyPosColumnNameApplier();

			var mapper = new Mock<IListPropertiesMapper>();
			var idxMapper = new Mock<IListIndexMapper>();
			mapper.Setup(x => x.Index(It.IsAny<Action<IListIndexMapper>>())).Callback<Action<IListIndexMapper>>(
				x => x.Invoke(idxMapper.Object));
			var path = new PropertyPath(null, ForClass<MyClass>.Property(p => p.Numbers));

			applier.Apply(path, mapper.Object);
			idxMapper.Verify(km => km.Column(It.Is<string>(s => s == "NumbersPos")));
		}
	}
}