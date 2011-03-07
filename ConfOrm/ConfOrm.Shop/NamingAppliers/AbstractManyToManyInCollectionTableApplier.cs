using System;
using System.Collections.Generic;
using System.Linq;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Shop.Appliers;

namespace ConfOrm.Shop.NamingAppliers
{
	public abstract class AbstractManyToManyInCollectionTableApplier : ManyToManyPattern,
	                                                  IPatternApplier<PropertyPath, ICollectionPropertiesMapper>
	{
		protected AbstractManyToManyInCollectionTableApplier(IDomainInspector domainInspector) : base(domainInspector) {}

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
			var explicitBidirectionalMember = DomainInspector.GetBidirectionalMember(fromMany, subject.LocalMember, toMany);
			if (explicitBidirectionalMember == null)
			{
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
			else
			{
				RelationOn fromRelation;
				RelationOn toRelation;
				if (fromIsMaster == toIsMaster)
				{
					var orderedByEntityClassName = (from t in (new[] { new RelationOn(fromMany, subject.LocalMember, toMany), new RelationOn(toMany, explicitBidirectionalMember, fromMany) }) orderby t.From.Name select t).ToArray();
					fromRelation = orderedByEntityClassName[0];
					toRelation = orderedByEntityClassName[1];
				}
				else
				{
					if(fromIsMaster)
					{
						fromRelation = new RelationOn(fromMany, subject.LocalMember, toMany, Declared.Explicit);
						toRelation = new RelationOn(toMany, explicitBidirectionalMember, fromMany, Declared.Implicit);
					}
					else
					{
						fromRelation = new RelationOn(toMany, explicitBidirectionalMember, fromMany, Declared.Explicit);
						toRelation = new RelationOn(fromMany, subject.LocalMember, toMany, Declared.Implicit);
					}
				}
				return GetTableNameForRelationOnProperty(fromRelation, toRelation);
			}
		}

		public abstract string GetTableNameForRelation(string[] names);
		public abstract string GetTableNameForRelationOnProperty(RelationOn fromRelation, RelationOn toRelation);
	}
}