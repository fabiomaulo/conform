using System;
using System.Collections.Generic;
using System.Reflection;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Shop.CoolNaming;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrm.ShopTests.AppliersTests
{
	public class DoubleManyToManyInCollectionTableTest
	{
		private class Person
		{
			public ICollection<Book> OwnedBooks { get; set; }
			public ICollection<Book> FavoritesBooks { get; set; }
		}

		private class Book
		{
			public ICollection<Person> OwnedBy { get; set; }
			public ICollection<Person> FavoriteBy { get; set; }
		}

		[Test]
		public void WhenManyToManyCollectionWithBidirectionalSpecifiedThenApplyTableFromMasterEntityWithPropertyName()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(Person)), It.Is<Type>(t => t == typeof(Book)))).Returns(true);
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(Book)), It.Is<Type>(t => t == typeof(Person)))).Returns(true);
			orm.Setup(x => x.IsMasterManyToMany(It.Is<Type>(t => t == typeof(Person)), It.Is<Type>(t => t == typeof(Book)))).Returns(true);
			orm.Setup(x => x.GetBidirectionalMember(It.Is<Type>(t => t == typeof(Person)), It.Is<MemberInfo>(m => m == ForClass<Person>.Property(c => c.OwnedBooks)), It.Is<Type>(t => t == typeof(Book)))).Returns(ForClass<Book>.Property(c => c.OwnedBy));
			orm.Setup(x => x.GetBidirectionalMember(It.Is<Type>(t => t == typeof(Book)), It.Is<MemberInfo>(m => m == ForClass<Book>.Property(c => c.OwnedBy)), It.Is<Type>(t => t == typeof(Person)))).Returns(ForClass<Person>.Property(c => c.OwnedBooks));
			orm.Setup(x => x.GetBidirectionalMember(It.Is<Type>(t => t == typeof(Person)), It.Is<MemberInfo>(m => m == ForClass<Person>.Property(c => c.FavoritesBooks)), It.Is<Type>(t => t == typeof(Book)))).Returns(ForClass<Book>.Property(c => c.FavoriteBy));
			orm.Setup(x => x.GetBidirectionalMember(It.Is<Type>(t => t == typeof(Book)), It.Is<MemberInfo>(m => m == ForClass<Book>.Property(c => c.FavoriteBy)), It.Is<Type>(t => t == typeof(Person)))).Returns(ForClass<Person>.Property(c => c.FavoritesBooks));

			var pattern = new ManyToManyInCollectionTableApplier(orm.Object);
			var path = new PropertyPath(null, ForClass<Person>.Property(x => x.OwnedBooks));
			var collectionMapper = new Mock<ICollectionPropertiesMapper>();

			pattern.Apply(path, collectionMapper.Object);

			collectionMapper.Verify(x => x.Table(It.Is<string>(tableName => tableName == "PersonOwnedBooks")));

			var bipath = new PropertyPath(null, ForClass<Book>.Property(x => x.OwnedBy));
			var bicollectionMapper = new Mock<ICollectionPropertiesMapper>();

			pattern.Match(path).Should().Be.True();
			pattern.Apply(bipath, bicollectionMapper.Object);

			bicollectionMapper.Verify(x => x.Table(It.Is<string>(tableName => tableName == "PersonOwnedBooks")));
		}

		[Test]
		public void WhenNoMasterManyToManyCollectionWithBidirectionalSpecifiedThenApplyTableAlphabeticEntityWithPropertiesNames()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(Person)), It.Is<Type>(t => t == typeof(Book)))).Returns(true);
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(Book)), It.Is<Type>(t => t == typeof(Person)))).Returns(true);
			orm.Setup(x => x.GetBidirectionalMember(It.Is<Type>(t => t == typeof(Person)), It.Is<MemberInfo>(m => m == ForClass<Person>.Property(c => c.OwnedBooks)), It.Is<Type>(t => t == typeof(Book)))).Returns(ForClass<Book>.Property(c => c.OwnedBy));
			orm.Setup(x => x.GetBidirectionalMember(It.Is<Type>(t => t == typeof(Book)), It.Is<MemberInfo>(m => m == ForClass<Book>.Property(c => c.OwnedBy)), It.Is<Type>(t => t == typeof(Person)))).Returns(ForClass<Person>.Property(c => c.OwnedBooks));
			orm.Setup(x => x.GetBidirectionalMember(It.Is<Type>(t => t == typeof(Person)), It.Is<MemberInfo>(m => m == ForClass<Person>.Property(c => c.FavoritesBooks)), It.Is<Type>(t => t == typeof(Book)))).Returns(ForClass<Book>.Property(c => c.FavoriteBy));
			orm.Setup(x => x.GetBidirectionalMember(It.Is<Type>(t => t == typeof(Book)), It.Is<MemberInfo>(m => m == ForClass<Book>.Property(c => c.FavoriteBy)), It.Is<Type>(t => t == typeof(Person)))).Returns(ForClass<Person>.Property(c => c.FavoritesBooks));

			var pattern = new ManyToManyInCollectionTableApplier(orm.Object);
			var path = new PropertyPath(null, ForClass<Person>.Property(x => x.OwnedBooks));
			var collectionMapper = new Mock<ICollectionPropertiesMapper>();

			pattern.Apply(path, collectionMapper.Object);

			collectionMapper.Verify(x => x.Table(It.Is<string>(tableName => tableName == "BookOwnedByPersonOwnedBooks")));

			var bipath = new PropertyPath(null, ForClass<Book>.Property(x => x.OwnedBy));
			var bicollectionMapper = new Mock<ICollectionPropertiesMapper>();

			pattern.Match(path).Should().Be.True();
			pattern.Apply(bipath, bicollectionMapper.Object);

			bicollectionMapper.Verify(x => x.Table(It.Is<string>(tableName => tableName == "BookOwnedByPersonOwnedBooks")));
		}
	}
}