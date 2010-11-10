using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConfOrm.Patterns
{
	/// <summary>
	/// Match only where the member is a generic-collection, the element has a property of the type of the collection container 
	/// applying polymorphism based on interface or classes.
	/// </summary>
	public class PolymorphismBidirectionalOneToManyMemberPattern : IPattern<MemberInfo>
	{
		private readonly IDomainInspector domainInspector;

		public PolymorphismBidirectionalOneToManyMemberPattern(IDomainInspector domainInspector)
		{
			this.domainInspector = domainInspector;
		}

		protected IDomainInspector DomainInspector
		{
			get { return domainInspector; }
		}

		public virtual bool Match(MemberInfo subject)
		{
			if (subject == null)
			{
				return false;
			}
			var declaredMany = subject.GetPropertyOrFieldType().DetermineCollectionElementType();
			if (declaredMany == null)
			{
				return false;
			}
			
			var implementorsOfMany = domainInspector.GetBaseImplementors(declaredMany).ToArray();
			if (implementorsOfMany.Length != 1)
			{
				// short
				return false;
			}

			var declaredOne = subject.ReflectedType;
			var implementorsOfOne = domainInspector.GetBaseImplementors(declaredOne).ToArray();
			if (implementorsOfOne.Length != 1)
			{
				return false;
			}

			var many = implementorsOfMany[0];
			var one = implementorsOfOne[0];
			List<Type> candidateAncestorsOfOne = GetCandidateAncestorsOf(one).ToList();

			bool isPolymorphicRelation = !declaredMany.Equals(many) || !declaredOne.Equals(one) || candidateAncestorsOfOne.Count > 0;
			return isPolymorphicRelation && (many.HasPublicPropertyOf(one) || HasPublicPropertyOf(many, candidateAncestorsOfOne));
		}

		protected IEnumerable<Type> GetCandidateAncestorsOf(Type one)
		{
			return from ancestor in one.GetBaseTypes()
			       let implementors = domainInspector.GetBaseImplementors(ancestor)
			       where implementors.IsSingle(t => t.Equals(one))
			       select ancestor;
		}

		protected bool HasPublicPropertyOf(Type many, IEnumerable<Type> candidateAncestors)
		{
			return candidateAncestors.Any(candidateAncestor => many.HasPublicPropertyOf(candidateAncestor));
		}
	}
}