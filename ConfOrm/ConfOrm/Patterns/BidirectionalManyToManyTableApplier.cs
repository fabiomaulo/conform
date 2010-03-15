using System;
using System.Linq;
using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.Patterns
{
	public class BidirectionalManyToManyTableApplier : BidirectionalManyToManyPattern,
	                                                   IPatternApplier<MemberInfo, ICollectionPropertiesMapper>
	{
		#region IPatternApplier<MemberInfo,ICollectionPropertiesMapper> Members

		public override bool Match(MemberInfo subject)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}
			return !IsCircularManyToMany(subject) && base.Match(subject);
		}

		public void Apply(MemberInfo subject, ICollectionPropertiesMapper applyTo)
		{
			Relation relation = GetRelation(subject);
			string tableName = GetTableNameForRelation(relation.From, relation.To);
			applyTo.Table(tableName);
		}

		#endregion

		protected virtual string GetTableNameForRelation(Type fromMany, Type toMany)
		{
			string[] names = (from t in (new[] {fromMany, toMany}) orderby t.Name select t.Name).ToArray();
			return names[0] + names[1];
		}
	}
}