using System;
using System.Collections.Generic;
using System.Reflection;
using NHibernate.Mapping.ByCode;
using ConfOrm.NH;
using ConfOrm.Shop.CoolNaming;
using Iesi.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace ConfOrm.ShopTests.CoolNamingsAppliersTests
{
	public class DoubleBidirectionalOneToManyKeyColumnApplierTest
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

		[Test]
		public void WhenParentChildThenApplyPropertyNameIdOfChildInFirstRelation()
		{
			Mock<IDomainInspector> orm = GetOrmMock();

			var pattern = new OneToManyKeyColumnApplier(orm.Object);

			var mapper = new Mock<ICollectionPropertiesMapper>();
			var keyMapper = new Mock<IKeyMapper>();
			mapper.Setup(x => x.Key(It.IsAny<Action<IKeyMapper>>())).Callback<Action<IKeyMapper>>(
				x => x.Invoke(keyMapper.Object));
			var path = new PropertyPath(null, ForClass<Team>.Property(p => p.MatchesAtHome));

			pattern.Apply(path, mapper.Object);
			keyMapper.Verify(km => km.Column(It.Is<string>(s => s == "HomeTeamId")));
		}

		private Mock<IDomainInspector> GetOrmMock()
		{
			var orm = new Mock<IDomainInspector>();
			orm.Setup(m => m.IsEntity(It.Is<Type>(t => typeof (Match) == t || typeof (Team) == t))).Returns(true);
			orm.Setup(m => m.IsOneToMany(typeof (Team), typeof (Match))).Returns(true);
			orm.Setup(m => m.IsManyToOne(typeof (Match), typeof (Team))).Returns(true);
			orm.Setup(m => m.GetBidirectionalMember(typeof (Team), It.Is<MemberInfo>(mi => mi.Equals(ForClass<Team>.Property(x => x.MatchesAtHome))), typeof (Match))).Returns(
				ForClass<Match>.Property(x => x.HomeTeam));
			orm.Setup(m => m.GetBidirectionalMember(typeof (Team), It.Is<MemberInfo>(mi => mi.Equals(ForClass<Team>.Property(x => x.MatchesOnRoad))), typeof (Match))).Returns(
				ForClass<Match>.Property(x => x.RoadTeam));
			return orm;
		}

		[Test]
		public void WhenParentChildThenApplyPropertyNameIdOfChildInSeconfRelation()
		{
			Mock<IDomainInspector> orm = GetOrmMock();
			var pattern = new OneToManyKeyColumnApplier(orm.Object);

			var mapper = new Mock<ICollectionPropertiesMapper>();
			var keyMapper = new Mock<IKeyMapper>();
			mapper.Setup(x => x.Key(It.IsAny<Action<IKeyMapper>>())).Callback<Action<IKeyMapper>>(
				x => x.Invoke(keyMapper.Object));
			var path = new PropertyPath(null, ForClass<Team>.Property(p => p.MatchesOnRoad));

			pattern.Apply(path, mapper.Object);
			keyMapper.Verify(km => km.Column(It.Is<string>(s => s == "RoadTeamId")));
		}

	}
}