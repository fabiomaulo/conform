using System;
using System.Collections.Generic;
using System.Reflection;
using NHibernate.Mapping.ByCode;
using ConfOrm.NH;
using ConfOrm.Shop.InflectorNaming;
using ConfOrm.Shop.Inflectors;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrm.ShopTests.InflectorNamingTests
{
	public class ManyToManyPluralizedTableApplierTest
	{
		private class MyClass
		{
			public ICollection<MyBidirect> MyBidirects { get; set; }
		}

		private class MyBidirect
		{
			public ICollection<MyClass> MyClasses { get; set; }
		}

		private class Person
		{
			public ICollection<Book> OwnedBooks { get; set; }
			public ICollection<Book> Favorites { get; set; }
		}

		private class Book
		{
			public ICollection<Person> OwnedBy { get; set; }
			public ICollection<Person> FavoriteBy { get; set; }
		}

		[Test]
		public void WhenManyToManyCollectionWithOutMasterThenApplyTableAlphabetical()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyClass)), It.Is<Type>(t => t == typeof(MyBidirect)))).Returns(true);
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(MyBidirect)), It.Is<Type>(t => t == typeof(MyClass)))).Returns(true);
			var inflector = new Mock<IInflector>();

			var pattern = new ManyToManyPluralizedTableApplier(orm.Object, inflector.Object);
			var path = new PropertyPath(null, ForClass<MyClass>.Property(x => x.MyBidirects));
			var collectionMapper = new Mock<ICollectionPropertiesMapper>();

			pattern.Match(path).Should().Be.True();
			pattern.Apply(path, collectionMapper.Object);

			inflector.Verify(x => x.Pluralize("MyBidirect"));
			inflector.Verify(x => x.Pluralize("MyClass"));
		}

		[Test]
		public void WhenManyToManyCollectionOnPropertyWithMasterThenApplyTableWithMasterToSlaveWithProperty()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(Person)), It.Is<Type>(t => t == typeof(Book)))).Returns(true);
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(Book)), It.Is<Type>(t => t == typeof(Person)))).Returns(true);
			orm.Setup(x => x.IsMasterManyToMany(It.Is<Type>(t => t == typeof(Person)), It.Is<Type>(t => t == typeof(Book)))).Returns(true);
			orm.Setup(x => x.GetBidirectionalMember(It.Is<Type>(t => t == typeof(Person)), It.Is<MemberInfo>(m => m == ForClass<Person>.Property(c => c.OwnedBooks)), It.Is<Type>(t => t == typeof(Book)))).Returns(ForClass<Book>.Property(c => c.OwnedBy));
			orm.Setup(x => x.GetBidirectionalMember(It.Is<Type>(t => t == typeof(Book)), It.Is<MemberInfo>(m => m == ForClass<Book>.Property(c => c.OwnedBy)), It.Is<Type>(t => t == typeof(Person)))).Returns(ForClass<Person>.Property(c => c.OwnedBooks));
			orm.Setup(x => x.GetBidirectionalMember(It.Is<Type>(t => t == typeof(Person)), It.Is<MemberInfo>(m => m == ForClass<Person>.Property(c => c.Favorites)), It.Is<Type>(t => t == typeof(Book)))).Returns(ForClass<Book>.Property(c => c.FavoriteBy));
			orm.Setup(x => x.GetBidirectionalMember(It.Is<Type>(t => t == typeof(Book)), It.Is<MemberInfo>(m => m == ForClass<Book>.Property(c => c.FavoriteBy)), It.Is<Type>(t => t == typeof(Person)))).Returns(ForClass<Person>.Property(c => c.Favorites));
			var inflector = new Mock<IInflector>();
			inflector.Setup(x => x.Pluralize("Person")).Returns("People");
			inflector.Setup(x => x.Pluralize("Book")).Returns("Books");

			var pattern = new ManyToManyPluralizedTableApplier(orm.Object, inflector.Object);
			var path = new PropertyPath(null, ForClass<Person>.Property(x => x.OwnedBooks));
			var collectionMapper = new Mock<ICollectionPropertiesMapper>();

			pattern.Match(path).Should().Be.True();
			pattern.Apply(path, collectionMapper.Object);

			collectionMapper.Verify(x => x.Table("PeopleOwnedBooks"));
		}

		[Test]
		public void WhenManyToManyCollectionOnPropertyWithMasterThenApplyTableWithMasterToSlaveWithPropertyAndPluralSlave()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(Person)), It.Is<Type>(t => t == typeof(Book)))).Returns(true);
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(Book)), It.Is<Type>(t => t == typeof(Person)))).Returns(true);
			orm.Setup(x => x.IsMasterManyToMany(It.Is<Type>(t => t == typeof(Person)), It.Is<Type>(t => t == typeof(Book)))).Returns(true);
			orm.Setup(x => x.GetBidirectionalMember(It.Is<Type>(t => t == typeof(Person)), It.Is<MemberInfo>(m => m == ForClass<Person>.Property(c => c.OwnedBooks)), It.Is<Type>(t => t == typeof(Book)))).Returns(ForClass<Book>.Property(c => c.OwnedBy));
			orm.Setup(x => x.GetBidirectionalMember(It.Is<Type>(t => t == typeof(Book)), It.Is<MemberInfo>(m => m == ForClass<Book>.Property(c => c.OwnedBy)), It.Is<Type>(t => t == typeof(Person)))).Returns(ForClass<Person>.Property(c => c.OwnedBooks));
			orm.Setup(x => x.GetBidirectionalMember(It.Is<Type>(t => t == typeof(Person)), It.Is<MemberInfo>(m => m == ForClass<Person>.Property(c => c.Favorites)), It.Is<Type>(t => t == typeof(Book)))).Returns(ForClass<Book>.Property(c => c.FavoriteBy));
			orm.Setup(x => x.GetBidirectionalMember(It.Is<Type>(t => t == typeof(Book)), It.Is<MemberInfo>(m => m == ForClass<Book>.Property(c => c.FavoriteBy)), It.Is<Type>(t => t == typeof(Person)))).Returns(ForClass<Person>.Property(c => c.Favorites));
			var inflector = new Mock<IInflector>();
			inflector.Setup(x => x.Pluralize("Person")).Returns("People");
			inflector.Setup(x => x.Pluralize("Book")).Returns("Books");

			var pattern = new ManyToManyPluralizedTableApplier(orm.Object, inflector.Object);
			var path = new PropertyPath(null, ForClass<Person>.Property(x => x.Favorites));
			var collectionMapper = new Mock<ICollectionPropertiesMapper>();

			pattern.Match(path).Should().Be.True();
			pattern.Apply(path, collectionMapper.Object);

			collectionMapper.Verify(x => x.Table("PeopleFavoritesBooks"));
		}

		[Test]
		public void WhenManyToManyCollectionOnPropertyWithNoMasterThenApplyTableAlphabeticalWithProperty()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(Person)), It.Is<Type>(t => t == typeof(Book)))).Returns(true);
			orm.Setup(x => x.IsManyToMany(It.Is<Type>(t => t == typeof(Book)), It.Is<Type>(t => t == typeof(Person)))).Returns(true);
			orm.Setup(x => x.GetBidirectionalMember(It.Is<Type>(t => t == typeof(Person)), It.Is<MemberInfo>(m => m == ForClass<Person>.Property(c => c.OwnedBooks)), It.Is<Type>(t => t == typeof(Book)))).Returns(ForClass<Book>.Property(c => c.OwnedBy));
			orm.Setup(x => x.GetBidirectionalMember(It.Is<Type>(t => t == typeof(Book)), It.Is<MemberInfo>(m => m == ForClass<Book>.Property(c => c.OwnedBy)), It.Is<Type>(t => t == typeof(Person)))).Returns(ForClass<Person>.Property(c => c.OwnedBooks));
			orm.Setup(x => x.GetBidirectionalMember(It.Is<Type>(t => t == typeof(Person)), It.Is<MemberInfo>(m => m == ForClass<Person>.Property(c => c.Favorites)), It.Is<Type>(t => t == typeof(Book)))).Returns(ForClass<Book>.Property(c => c.FavoriteBy));
			orm.Setup(x => x.GetBidirectionalMember(It.Is<Type>(t => t == typeof(Book)), It.Is<MemberInfo>(m => m == ForClass<Book>.Property(c => c.FavoriteBy)), It.Is<Type>(t => t == typeof(Person)))).Returns(ForClass<Person>.Property(c => c.Favorites));
			var inflector = new Mock<IInflector>();
			inflector.Setup(x => x.Pluralize("Person")).Returns("People");
			inflector.Setup(x => x.Pluralize("Book")).Returns("Books");

			var pattern = new ManyToManyPluralizedTableApplier(orm.Object, inflector.Object);
			var path = new PropertyPath(null, ForClass<Person>.Property(x => x.OwnedBooks));
			var collectionMapper = new Mock<ICollectionPropertiesMapper>();

			pattern.Match(path).Should().Be.True();
			pattern.Apply(path, collectionMapper.Object);

			collectionMapper.Verify(x => x.Table("BooksOwnedByPeople"));
		}

	}
}