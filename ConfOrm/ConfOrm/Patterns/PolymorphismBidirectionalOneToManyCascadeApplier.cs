using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.Patterns
{
	public class PolymorphismBidirectionalOneToManyCascadeApplier: PolymorphismBidirectionalOneToManyMemberPattern, IPatternApplier<MemberInfo, ICollectionPropertiesMapper>
	{
		public PolymorphismBidirectionalOneToManyCascadeApplier(IDomainInspector domainInspector) : base(domainInspector) {}
		
		public void Apply(MemberInfo subject, ICollectionPropertiesMapper applyTo)
		{
			var explicitPolymorphismCascade = (from relation in GetRelations(subject) 
							 let cascade = DomainInspector.ApplyCascade(relation.From, subject, relation.To)
							 where cascade.HasValue
							 select cascade).FirstOrDefault();
			if(explicitPolymorphismCascade.HasValue)
			{
				applyTo.Cascade(explicitPolymorphismCascade.Value);								
			}
			else
			{
				applyTo.Cascade(Cascade.All | Cascade.DeleteOrphans);				
			}
		}

		protected IEnumerable<Relation> GetRelations(MemberInfo subject)
		{
			var declaredMany = subject.GetPropertyOrFieldType().DetermineCollectionElementType();
			var declaredOne = subject.ReflectedType;
			yield return new Relation(declaredOne, declaredMany);
			var implementorsOfMany = DomainInspector.GetBaseImplementors(declaredMany).ToArray();
			var implementorsOfOne = DomainInspector.GetBaseImplementors(declaredOne).ToArray();

			var many = implementorsOfMany[0];
			var one = implementorsOfOne[0];
			if (declaredOne != one || declaredMany != many)
			{
				yield return new Relation(one, many);
			}
			foreach (var type in GetCandidateAncestorsOf(one))
			{
				yield return new Relation(type, declaredMany);
				yield return new Relation(type, many);
			}
		}

	}
}