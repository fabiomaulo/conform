using System;
using System.Linq;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Shop.Appliers;

namespace ConfOrm.Shop.CoolNaming
{
	public class ManyToManyTableApplier : ManyToManyPattern, IPatternApplier<PropertyPath, ICollectionPropertiesMapper>
	{
		public ManyToManyTableApplier(IDomainInspector domainInspector) : base(domainInspector) {}

		#region Implementation of IPattern<PropertyPath>

		public bool Match(PropertyPath subject)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}

			return Match(subject.LocalMember);
		}

		#endregion

		#region Implementation of IPatternApplier<PropertyPath,ICollectionPropertiesMapper>

		public void Apply(PropertyPath subject, ICollectionPropertiesMapper applyTo)
		{
			applyTo.Table(GetTableName(subject));
		}

		#endregion

		protected virtual string GetTableName(PropertyPath subject)
		{
			var entityType = subject.GetContainerEntity(DomainInspector);
			var relation = GetRelation(subject.LocalMember);

			bool fromIsMaster = DomainInspector.IsMasterManyToMany(relation.From, relation.To);
			bool toIsMaster = DomainInspector.IsMasterManyToMany(relation.To, relation.From);

			var fromMany = entityType;
			var toMany = relation.To;

			return GetTableNameForRelation(fromMany, toMany, fromIsMaster, toIsMaster);
		}

		protected virtual string GetTableNameForRelation(Type fromMany, Type toMany, bool fromIsMaster, bool toIsMaster)
		{
			string[] names;
			if (fromIsMaster == toIsMaster)
			{
				names = (from t in (new[] { fromMany, toMany }) orderby t.Name select t.Name).ToArray();
			}
			else
			{
				names = fromIsMaster ? new[] { fromMany.Name, toMany.Name } : new[] { toMany.Name, fromMany.Name };
			}
			return GetTableNameForRelation(names);
		}

		protected virtual string GetTableNameForRelation(string[] names)
		{
			return string.Format("{0}To{1}", names[0], names[1]);
		}
	}
}