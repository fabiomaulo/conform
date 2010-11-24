using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConfOrm;
using ConfOrm.NH;
using Iesi.Collections.Generic;
using Moq;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests.DoubleBidirectionalTests
{
	public class OneToManyTests
	{
		public class Match
		{
			public int Id { get; set; }
			public Team HomeTeam { get; set; }
			public Team RoadTeam { get; set; }
		}

		public class Team
		{
			private readonly ISet<Match> matchesAtHome = new HashedSet<Match>();
			private readonly ISet<Match> matchesOnRoad = new HashedSet<Match>();

			public int Id { get; set; }
			public IEnumerable<Match> MatchesAtHome
			{
				get { return matchesAtHome; }
			}

			public IEnumerable<Match> MatchesOnRoad
			{
				get { return matchesOnRoad; }
			}
		}

		private Mock<IDomainInspector> GetMockedDomainInspector()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.Is<Type>(t => typeof(Match) == t ||typeof(Team) == t))).Returns(true);
			orm.Setup(m => m.IsRootEntity(It.Is<Type>(t => typeof(Match) == t || typeof(Team) == t))).Returns(true);
			orm.Setup(m => m.IsTablePerClass(It.IsAny<Type>())).Returns(true);
			orm.Setup(m => m.IsPersistentId(It.Is<MemberInfo>(mi => mi.Name == "Id"))).Returns(true);
			orm.Setup(m => m.IsPersistentProperty(It.Is<MemberInfo>(mi => mi.Name != "Id"))).Returns(true);
			orm.Setup(m => m.IsSet(It.Is<MemberInfo>(mi => mi.Equals(ForClass<Team>.Property(x => x.MatchesAtHome))))).Returns(true);
			orm.Setup(m => m.IsSet(It.Is<MemberInfo>(mi => mi.Equals(ForClass<Team>.Property(x => x.MatchesOnRoad))))).Returns(true);
			orm.Setup(m => m.IsOneToMany(typeof(Team), typeof(Match))).Returns(true);
			orm.Setup(m => m.IsManyToOne(typeof(Match), typeof(Team))).Returns(true);

			orm.Setup(m => m.GetBidirectionalMember(typeof(Team), It.Is<MemberInfo>(mi => mi.Equals(ForClass<Team>.Property(x => x.MatchesAtHome))), typeof(Match))).Returns(ForClass<Match>.Property(x => x.HomeTeam));
			orm.Setup(m => m.GetBidirectionalMember(typeof(Team), It.Is<MemberInfo>(mi => mi.Equals(ForClass<Team>.Property(x => x.MatchesOnRoad))), typeof(Match))).Returns(ForClass<Match>.Property(x => x.RoadTeam));
			return orm;
		}

		private HbmMapping GetMapping(IDomainInspector domainInspector)
		{
			var mapper = new Mapper(domainInspector);
			return mapper.CompileMappingFor(new[] { typeof(Team), typeof(Match) });
		}

		[Test]
		public void WhenExplicitlyDeclaredThenEachCollectionMapToItsCorrectParentPropertyThroughMock()
		{
			Mock<IDomainInspector> orm = GetMockedDomainInspector();

			var domainInspector = orm.Object;
			HbmMapping mapping = GetMapping(domainInspector);
			VerifyBagsHasTheCorrectKey(mapping);
		}

		private void VerifyBagsHasTheCorrectKey(HbmMapping mapping)
		{
			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("Team"));
			var relation1 = (HbmSet)rc.Properties.First(p => p.Name == "MatchesAtHome");
			var hbmKey1 = relation1.Key;
			hbmKey1.column1.Should().Contain("HomeTeam");
			var relation2 = (HbmSet)rc.Properties.First(p => p.Name == "MatchesOnRoad");
			var hbmKey2 = relation2.Key;
			hbmKey2.column1.Should().Contain("RoadTeam");
		}

		[Test]
		public void WhenExplicitlyDeclaredThenEachCollectionMapToItsCorrectParentPropertyIntegration()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<Team>();
			orm.TablePerClass<Match>();
			orm.Bidirectional<Team, Match>(t => t.MatchesAtHome, m => m.HomeTeam);
			orm.Bidirectional<Match, Team>(m => m.RoadTeam, t => t.MatchesOnRoad);

			HbmMapping mapping = GetMapping(orm);

			VerifyBagsHasTheCorrectKey(mapping);
		}
	}
}