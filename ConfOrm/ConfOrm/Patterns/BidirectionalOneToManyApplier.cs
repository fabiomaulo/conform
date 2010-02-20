using System;
using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.Patterns
{
	public class BidirectionalOneToManyApplier : BidirectionalOneToManyPattern, IPatternApplier<MemberInfo, ICollectionPropertiesMapper>
	{
		#region Implementation of IPattern<MemberInfo>

		public bool Match(MemberInfo subject)
		{
			var propertyType = subject.GetPropertyOrFieldType();
			Type many = propertyType.DetermineCollectionElementType();
			Type one = subject.DeclaringType;
			if(many == null)
			{
				return false;
			}
			return base.Match(new Relation(one, many));
		}

		#endregion

		#region Implementation of IPatternApplier<MemberInfo,ICollectionPropertiesMapper>

		public void Apply(MemberInfo subject, ICollectionPropertiesMapper applyTo)
		{
			applyTo.Inverse(true);
			applyTo.Key(km => km.OnDelete(OnDeleteAction.Cascade));
		}

		#endregion
	}
}