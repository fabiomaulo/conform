using System;
using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.Patterns
{
	public class BidirectionalOneToManyOnDeleteConstraintApplier : BidirectionalOneToManyPattern, IPatternApplier<MemberInfo, ICollectionPropertiesMapper>
	{
		// TODO : should check that is a real one-to-many and avoid many-to-many (it need IDomainInspector injection)
		public bool Match(MemberInfo subject)
		{
			var propertyType = subject.GetPropertyOrFieldType();
			Type many = propertyType.DetermineCollectionElementType();
			Type one = subject.DeclaringType;
			if (many == null)
			{
				return false;
			}
			if (one.Equals(many))
			{
				// Circular references
				return false;
			} 
			return base.Match(new Relation(one, many));
		}

		#region Implementation of IPatternApplier<MemberInfo,ICollectionPropertiesMapper>

		public void Apply(MemberInfo subject, ICollectionPropertiesMapper applyTo)
		{
			applyTo.Key(km => km.OnDelete(OnDeleteAction.Cascade));
		}

		#endregion
	}
}