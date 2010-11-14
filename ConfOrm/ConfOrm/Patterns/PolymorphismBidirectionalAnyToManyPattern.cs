using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConfOrm.Patterns
{
	public class PolymorphismBidirectionalAnyToManyPattern: IPattern<MemberInfo>
	{
		private readonly IDomainInspector domainInspector;

		public PolymorphismBidirectionalAnyToManyPattern(IDomainInspector domainInspector)
		{
			if (domainInspector == null)
			{
				throw new ArgumentNullException("domainInspector");
			}
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
			if(!domainInspector.IsEntity(declaredMany))
			{
				return false;
			}
			var declaredOne = subject.ReflectedType;
			List<Type> ancestorsOfOne = declaredOne.GetBaseTypes().ToList();
			var cadidatedBidirectional = declaredMany.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
				.Where(p => ancestorsOfOne.Contains(p.PropertyType)).FirstOrDefault(p=> domainInspector.IsHeterogeneousAssociation(p));
			return cadidatedBidirectional != null;
		}
	}
}