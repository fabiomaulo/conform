using System;
using System.Reflection;

namespace ConfOrm.Patterns
{
	public class BidirectionalOneToManyMemberPattern : BidirectionalOneToManyPattern, IPattern<MemberInfo>
	{
		public BidirectionalOneToManyMemberPattern(IDomainInspector domainInspector) : base(domainInspector) {}

		#region Implementation of IPattern<MemberInfo>

		public bool Match(MemberInfo subject)
		{
			var propertyType = subject.GetPropertyOrFieldType();
			Type many = propertyType.DetermineCollectionElementType();
			Type one = subject.DeclaringType;
			if (many == null)
			{
				return false;
			}
			return base.Match(new Relation(one, many));
		}

		#endregion
	}
}