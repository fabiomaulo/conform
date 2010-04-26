using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using ConfOrm;
using ConfOrm.NH;
using Moq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH.MapperTests
{
	public class TypeDefTest
	{
		private class MyClass
		{
			public int Id { get; set; }
			public MonetaryAmount Amount { get; set; }
			public ICollection<MonetaryAmount> Amounts { get; set; }
			public IDictionary<MonetaryAmount, string > AmountsMapKey { get; set; }
			public IDictionary<string, MonetaryAmount> AmountsMapValue { get; set; }
		}

		private class MonetaryAmount
		{
			private readonly decimal value;
			private readonly string currency;

			public MonetaryAmount(decimal value, string currency)
			{
				this.value = value;
				this.currency = currency;
			}

			public string Currency
			{
				get { return currency; }
			}

			public decimal Value
			{
				get { return value; }
			}
		}

		private class MonetaryAmountUserType: IUserType
		{
			public bool Equals(object x, object y)
			{
				throw new NotImplementedException();
			}

			public int GetHashCode(object x)
			{
				throw new NotImplementedException();
			}

			public object NullSafeGet(IDataReader rs, string[] names, object owner)
			{
				throw new NotImplementedException();
			}

			public void NullSafeSet(IDbCommand cmd, object value, int index)
			{
				throw new NotImplementedException();
			}

			public object DeepCopy(object value)
			{
				throw new NotImplementedException();
			}

			public object Replace(object original, object target, object owner)
			{
				throw new NotImplementedException();
			}

			public object Assemble(object cached, object owner)
			{
				throw new NotImplementedException();
			}

			public object Disassemble(object value)
			{
				throw new NotImplementedException();
			}

			public SqlType[] SqlTypes
			{
				get { throw new NotImplementedException(); }
			}

			public Type ReturnedType
			{
				get { throw new NotImplementedException(); }
			}

			public bool IsMutable
			{
				get { throw new NotImplementedException(); }
			}
		}
		private Mock<IDomainInspector> GetMockedDomainInspector()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.Is<Type>(t => t == typeof(MyClass)))).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => t == typeof(MyClass)))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.IsComplex(It.Is<MemberInfo>(mi => mi == typeof(MyClass).GetProperty("Amount")))).Returns(true);
			orm.Setup(m => m.IsBag(It.Is<MemberInfo>(mi => mi == ForClass<MyClass>.Property(x=> x.Amounts)))).Returns(true);
			orm.Setup(m => m.IsDictionary(It.Is<MemberInfo>(mi => mi == ForClass<MyClass>.Property(x => x.AmountsMapKey)))).Returns(true);
			orm.Setup(m => m.IsDictionary(It.Is<MemberInfo>(mi => mi == ForClass<MyClass>.Property(x=> x.AmountsMapValue)))).Returns(true);
			return orm;
		}

		private HbmMapping GetMapping(IDomainInspector domainInspector)
		{
			var mapper = new Mapper(domainInspector);
			mapper.TypeDef<MonetaryAmount, MonetaryAmountUserType>();
			return mapper.CompileMappingFor(new[] { typeof(MyClass) });
		}

		[Test]
		public void WhenUserTypeUsedInPropertyThenApplyUserType()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();

			var domainInspector = orm.Object;
			HbmMapping mapping = GetMapping(domainInspector);

			var rc = mapping.RootClasses.Single();
			rc.Properties.OfType<HbmProperty>().Single().type1.Should().Contain("MonetaryAmountUserType");
		}

		[Test]
		public void WhenUserTypeUsedInCollectionElementThenApplyUserType()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();
			var domainInspector = orm.Object;
			HbmMapping mapping = GetMapping(domainInspector);

			var rc = mapping.RootClasses.Single();
			var elementRelation = (HbmElement)rc.Properties.OfType<HbmBag>().Single().ElementRelationship;
			elementRelation.type1.Should().Contain("MonetaryAmountUserType");
		}

		[Test]
		public void WhenUserTypeUsedInMapKeyThenApplyUserType()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();
			var domainInspector = orm.Object;
			HbmMapping mapping = GetMapping(domainInspector);

			var rc = mapping.RootClasses.Single();
			var keyRelation = (HbmMapKey)rc.Properties.OfType<HbmMap>().Where(me => me.Name == "AmountsMapKey").Single().Item;
			keyRelation.type.Should().Contain("MonetaryAmountUserType");
		}

		[Test]
		public void WhenUserTypeUsedInMapKValueThenApplyUserType()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();
			var domainInspector = orm.Object;
			HbmMapping mapping = GetMapping(domainInspector);

			var rc = mapping.RootClasses.Single();
			var elementRelation = (HbmElement)rc.Properties.OfType<HbmMap>().Where(me => me.Name == "AmountsMapValue").Single().ElementRelationship;
			elementRelation.type1.Should().Contain("MonetaryAmountUserType");
		}
	}
}