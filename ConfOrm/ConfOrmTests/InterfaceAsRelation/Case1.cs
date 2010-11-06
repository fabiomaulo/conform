using System;
using System.Collections.Generic;
using System.Linq;
using ConfOrm;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.InterfaceAsRelation
{
	public class Case1
	{
		private class MyEntity
		{
			public int Id { get; set; }
			public IRelation Relation { get; set; }
			public Relation1 Relation1 { get; set; }
		}

		private interface IRelation
		{
		}

		private class MyRelation : IRelation
		{
			public int Id { get; set; }
		}
		private class Relation1
		{
		}

		private class MyRelation1 : Relation1
		{
			public int Id { get; set; }
		}

		[Test, Ignore("Not 'auto' supported.")]
		public void WhenInterfaceIsImplementedByEntityThenRecognizeManyToOneWithTheCorrectClass()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<MyEntity>();
			orm.TablePerClass<MyRelation>();

			var mapper = new Mapper(orm);
			var mapping = mapper.CompileMappingFor(new[] { typeof(MyEntity) });

			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("MyEntity"));
			rc.Properties.Where(p => p.Name == "Relation").Single().Should().Be.OfType<HbmManyToOne>()
				.And.ValueOf.Class.Should().Contain("MyRelation");
		}

		[Test]
		public void WhenAskForInterfaceThenGetFistEntityImplementingTheInterface()
		{
			var domainAnalyzer = new DomainAnalyzer();
			domainAnalyzer.Add(typeof(MyRelation));
			domainAnalyzer.Add(typeof(MyRelation1));
			domainAnalyzer.GetBaseImplementors(typeof(IRelation)).Single().Should().Be(typeof(MyRelation));
			domainAnalyzer.GetBaseImplementors(typeof(Relation1)).Single().Should().Be(typeof(MyRelation1));
		}
	}

	public class DomainAnalyzer
	{
		private ICollection<Type> domain = new HashSet<Type>();
		public IEnumerable<Type> GetBaseImplementors(Type ancestor)
		{
			foreach (var type in domain)
			{
				if(ancestor.IsAssignableFrom(type))
				{
					yield return type;
				}
			}
		}

		public void Add(Type type)
		{
			if (type == null)
			{
				return;
			}
			domain.Add(type);
		}
	}
}