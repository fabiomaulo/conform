using System;
using System.Reflection;
using NHibernate.Mapping.ByCode;
using ConfOrm.Shop.Appliers;
using ConfOrm.Shop.CoolNaming;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrm.ShopTests.AppliersTests
{
	public class JoinedSubclassKeyAsRootIdColumnApplierTest
	{
		private class MyClass
		{
			public int Id { get; set; }
			public int PoId { get; set; }
		}

		private class Inherited : MyClass
		{
		}

		[Test]
		public void AlwaysMatch()
		{
			var inspector = new Mock<IDomainInspector>();
			var pattern = new JoinedSubclassKeyAsRootIdColumnApplier(inspector.Object);
			pattern.Match(null).Should().Be.True();
		}

		[Test]
		public void WhenBaseIsIdThenUseId()
		{
			var inspector = new Mock<IDomainInspector>();
			inspector.Setup(x => x.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			var pattern = new JoinedSubclassKeyAsRootIdColumnApplier(inspector.Object);
			var mapper = new Mock<IJoinedSubclassAttributesMapper>();
			var keyMapper = new Mock<IKeyMapper>();
			mapper.Setup(x => x.Key(It.IsAny<Action<IKeyMapper>>())).Callback<Action<IKeyMapper>>(
				x => x.Invoke(keyMapper.Object));

			pattern.Apply(typeof (Inherited), mapper.Object);

			keyMapper.Verify(km => km.Column(It.Is<string>(s => s == "Id")));
		}

		[Test]
		public void WhenBaseIsPoIdThenUsePoId()
		{
			var inspector = new Mock<IDomainInspector>();
			inspector.Setup(x => x.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "PoId"))).Returns(true);
			var pattern = new JoinedSubclassKeyAsRootIdColumnApplier(inspector.Object);
			var mapper = new Mock<IJoinedSubclassAttributesMapper>();
			var keyMapper = new Mock<IKeyMapper>();
			mapper.Setup(x => x.Key(It.IsAny<Action<IKeyMapper>>())).Callback<Action<IKeyMapper>>(
				x => x.Invoke(keyMapper.Object));

			pattern.Apply(typeof(Inherited), mapper.Object);

			keyMapper.Verify(km => km.Column(It.Is<string>(s => s == "PoId")));
		}

		[Test]
		public void WhenNoPoidMemberThenDoesNotSet()
		{
			var inspector = new Mock<IDomainInspector>();
			var pattern = new JoinedSubclassKeyAsRootIdColumnApplier(inspector.Object);
			var mapper = new Mock<IJoinedSubclassAttributesMapper>();
			var keyMapper = new Mock<IKeyMapper>();
			mapper.Setup(x => x.Key(It.IsAny<Action<IKeyMapper>>())).Callback<Action<IKeyMapper>>(
				x => x.Invoke(keyMapper.Object));

			pattern.Apply(typeof(Inherited), mapper.Object);

			keyMapper.Verify(km => km.Column(It.IsAny<string>()), Times.Never());
		}
	}
}