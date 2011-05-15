using System;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.NH.TypeNamesUtilsTests
{
	public class MyEntity
	{
		
	}
	public class ClassNameTest
	{
		[Test]
		public void WhenTypeNullThenNull()
		{
			Type variableType = null;
			variableType.GetShortClassName(new HbmMapping()).Should().Be.Null();
		}

		[Test]
		public void WhenMapDocNullThenAssemblyQualifiedName()
		{
			typeof(MyEntity).GetShortClassName(null).Should().Be.EqualTo(typeof(MyEntity).AssemblyQualifiedName);
		}

		[Test]
		public void WhenMapDocDoesNotHaveDefaultsThenAssemblyQualifiedName()
		{
			var mapDoc = new HbmMapping();
			typeof(MyEntity).GetShortClassName(mapDoc).Should().Be.EqualTo(typeof(MyEntity).AssemblyQualifiedName);
		}

		[Test]
		public void WhenMapDocHaveDefaultAssemblyThenFullName()
		{
			var mapDoc = new HbmMapping();
			mapDoc.assembly = typeof (MyEntity).Assembly.FullName;
			typeof(MyEntity).GetShortClassName(mapDoc).Should().Be.EqualTo(typeof(MyEntity).FullName);
		}

		[Test]
		public void WhenMapDocHaveDefaultAssemblyNameThenFullName()
		{
			var mapDoc = new HbmMapping();
			mapDoc.assembly = typeof(MyEntity).Assembly.GetName().Name;
			typeof(MyEntity).GetShortClassName(mapDoc).Should().Be.EqualTo(typeof(MyEntity).FullName);
		}

		[Test]
		public void WhenMapDocHaveDefaultsThenName()
		{
			var mapDoc = new HbmMapping();
			mapDoc.assembly = typeof(MyEntity).Assembly.FullName;
			mapDoc.@namespace = typeof(MyEntity).Namespace;
			typeof(MyEntity).GetShortClassName(mapDoc).Should().Be.EqualTo(typeof(MyEntity).Name);
		}

		[Test]
		public void WhenMapDocDefaultsDoesNotMatchsThenAssemblyQualifiedName()
		{
			var mapDoc = new HbmMapping();
			mapDoc.assembly = "whatever";
			mapDoc.@namespace = "whatever";
			typeof(MyEntity).GetShortClassName(mapDoc).Should().Be.EqualTo(typeof(MyEntity).AssemblyQualifiedName);
		}

		[Test]
		public void WhenMatchNamespaceButNotAssemblyThenOnlyNameAndAssembly()
		{
			// strange but possible
			var mapDoc = new HbmMapping();
			mapDoc.assembly = "whatever";
			mapDoc.@namespace = typeof(MyEntity).Namespace;
			typeof(MyEntity).GetShortClassName(mapDoc).Should().StartWith(typeof(MyEntity).Name).And.EndWith(", " + typeof(MyEntity).Assembly.GetName().Name);
		}
	}
}