using System;
using System.Collections.Generic;
using System.Linq;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Shop.Appliers;

namespace ConfOrm.Shop.CoolNaming
{
	public class ManyToManyInCollectionTableApplier : ManyToManyPattern,
	                                                  IPatternApplier<PropertyPath, ICollectionPropertiesMapper>
	{
		public ManyToManyInCollectionTableApplier(IDomainInspector domainInspector) : base(domainInspector) {}

		#region Implementation of IPattern<PropertyPath>

		public bool Match(PropertyPath subject)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}
			bool isManyToMany = Match(subject.LocalMember);
			if (isManyToMany)
			{
				Type propertyType = subject.LocalMember.GetPropertyOrFieldType();
				Type cadidateToMany = propertyType.DetermineCollectionElementType();
				if (cadidateToMany.IsGenericType && typeof (KeyValuePair<,>) == cadidateToMany.GetGenericTypeDefinition())
				{
					// does not match for on Dictionary
					// A Dictionary may have a relation with 3 entities, how manage the table-name will be matter of another applier
					return false;
				}
			}
			return isManyToMany;
		}

		#endregion

		#region Implementation of IPatternApplier<PropertyPath,ICollectionPropertiesMapper>

		public void Apply(PropertyPath subject, ICollectionPropertiesMapper applyTo)
		{
			applyTo.Table(GetManyToManyTableName(subject));
		}

		#endregion

		protected virtual string GetManyToManyTableName(PropertyPath subject)
		{
			var manyToManyRelation = GetRelation(subject.LocalMember);
			var fromMany = manyToManyRelation.From;
			var toMany = manyToManyRelation.To;
			bool fromIsMaster = DomainInspector.IsMasterManyToMany(fromMany, toMany);
			bool toIsMaster = DomainInspector.IsMasterManyToMany(toMany, fromMany);
			fromMany = subject.GetContainerEntity(DomainInspector);
			string[] names;
			if (fromIsMaster == toIsMaster)
			{
				names = (from t in (new[] {fromMany, toMany}) orderby t.Name select t.Name).ToArray();
			}
			else
			{
				names = fromIsMaster ? new[] {fromMany.Name, toMany.Name} : new[] {toMany.Name, fromMany.Name};
			}
			return GetTableNameForRelation(names);
		}

		protected virtual string GetTableNameForRelation(string[] names)
		{
			return string.Format("{0}To{1}", names[0], names[1]);
		}
	}
}