using System.Reflection;
using NHibernate.Mapping.ByCode;

namespace ConfOrm.Patterns
{
	public class PolymorphismBidirectionalOneToManyCascadeApplier: PolymorphismBidirectionalOneToManyMemberPattern, IPatternApplier<MemberInfo, ICollectionPropertiesMapper>
	{
		public PolymorphismBidirectionalOneToManyCascadeApplier(IDomainInspector domainInspector) : base(domainInspector) {}
		
		public void Apply(MemberInfo subject, ICollectionPropertiesMapper applyTo)
		{
			CascadeOn? explicitPolymorphismCascade = GetExplicitPolymorphismCascade(subject);
			if(explicitPolymorphismCascade.HasValue)
			{
				applyTo.Cascade(explicitPolymorphismCascade.Value.ToCascade());								
			}
			else
			{
				applyTo.Cascade(Cascade.All | Cascade.DeleteOrphans);				
			}
		}
	}
}