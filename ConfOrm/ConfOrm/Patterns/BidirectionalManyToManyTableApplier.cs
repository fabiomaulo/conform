using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.Patterns
{
	public class BidirectionalManyToManyTableApplier : BidirectionalManyToManyPattern, IPatternApplier<MemberInfo, ICollectionPropertiesMapper>
	{
		public override bool Match(MemberInfo subject)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}
			return !IsCircularManyToMany(subject) && base.Match(subject);
		}

		protected bool IsCircularManyToMany(MemberInfo subject)
		{
			var propertyType = subject.GetPropertyOrFieldType();
			if (!propertyType.IsGenericCollection())
			{
				// can't determine relation for a no generic collection
				return false;
			}

			var fromMany = subject.DeclaringType;
			Type cadidateToMany = propertyType.DetermineCollectionElementType();
			if (cadidateToMany.IsGenericType && typeof (KeyValuePair<,>) == cadidateToMany.GetGenericTypeDefinition())
			{
				// many-to-many on map
				var dictionaryGenericArguments = cadidateToMany.GetGenericArguments();
				if (fromMany == dictionaryGenericArguments[0] || fromMany == dictionaryGenericArguments[1])
				{
					// circular many-to-many reference
					return true;
				}
			}
			else
			{
				// many-to-many on plain collection
				if (fromMany == cadidateToMany)
				{
					// circular many-to-many reference
					return true;
				}
			}
			return false;
		}

		#region Implementation of IPatternApplier<MemberInfo,ICollectionPropertiesMapper>

		public void Apply(MemberInfo subject, ICollectionPropertiesMapper applyTo)
		{
			var propertyType = subject.GetPropertyOrFieldType();

			var fromMany = subject.DeclaringType;
			Type cadidateToMany = propertyType.DetermineCollectionElementType();
			string tableName;
			if (cadidateToMany.IsGenericType && typeof(KeyValuePair<,>) == cadidateToMany.GetGenericTypeDefinition())
			{
				// many-to-many on map
				var dictionaryGenericArguments = cadidateToMany.GetGenericArguments();
				var toMany = dictionaryGenericArguments[1];
				if (HasCollectionOf(toMany, fromMany))
				{
					tableName = GetTableNameForRelation(fromMany, toMany);
				}
				else
				{
					toMany = dictionaryGenericArguments[0];
					tableName = GetTableNameForRelation(fromMany, toMany);
				}
			}
			else
			{
				// many-to-many on plain collection
				var toMany = cadidateToMany;
				tableName = GetTableNameForRelation(fromMany, toMany);
			}

			applyTo.Table(tableName);
		}


		#endregion

		protected virtual string GetTableNameForRelation(Type fromMany, Type toMany)
		{
			var names = (from t in (new[] { fromMany, toMany }) orderby t.Name select t.Name).ToArray();
			return names[0] + names[1];
		}
	}
}