using System;
using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.Patterns
{
	public class PolymorphismBidirectionalOneToManyCascadeApplier: PolymorphismBidirectionalOneToManyMemberPattern, IPatternApplier<MemberInfo, ICollectionPropertiesMapper>
	{
		public PolymorphismBidirectionalOneToManyCascadeApplier(IDomainInspector domainInspector) : base(domainInspector) {}
		
		public override bool Match(MemberInfo subject)
		{
			return base.Match(subject);
		}
		
		public void Apply(MemberInfo subject, ICollectionPropertiesMapper applyTo)
		{
			applyTo.Cascade(Cascade.All | Cascade.DeleteOrphans);
		}
	}
}