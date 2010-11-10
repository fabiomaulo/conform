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

		public bool Match(MemberInfo subject)
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
			var ancestorsOfOne = one.GetBaseTypes();
			var candidateAncestorsOfOne = new List<Type>();
			foreach (var ancestor in ancestorsOfOne)
			{
				var implementors = domainInspector.GetBaseImplementors(ancestor);
				if (implementors.IsSingle() && implementors.Contains(one))
				{
					candidateAncestorsOfOne.Add(ancestor);
				}
			}

			bool isPolymorphicRelation = !declaredMany.Equals(many) || !declaredOne.Equals(one) || candidateAncestorsOfOne.Count > 0;
			return isPolymorphicRelation && (many.HasPublicPropertyOf(one) || HasPublicPropertyOf(many, candidateAncestorsOfOne));
		}

		protected bool HasPublicPropertyOf(Type many, IEnumerable<Type> candidateAncestors)
		{
			return candidateAncestors.Any(candidateAncestor => many.HasPublicPropertyOf(candidateAncestor));
		}
	}
}