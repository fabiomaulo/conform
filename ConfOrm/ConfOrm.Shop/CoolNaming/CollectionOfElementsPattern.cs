using System;
using System.Collections.Generic;
using System.Reflection;

namespace ConfOrm.Shop.CoolNaming
{
	public class CollectionOfElementsPattern : IPattern<MemberInfo>
	{
		private readonly IDomainInspector domainInspector;

		public CollectionOfElementsPattern(IDomainInspector domainInspector)
		{
			if (domainInspector == null)
			{
				throw new ArgumentNullException("domainInspector");
			}
			this.domainInspector = domainInspector;
		}

		public IDomainInspector DomainInspector
		{
			get { return domainInspector; }
		}

		#region Implementation of IPattern<MemberInfo>

		public virtual bool Match(MemberInfo subject)
		{
			var memberType = subject.GetPropertyOrFieldType();
			if (!memberType.IsGenericCollection())
			{
				return false;
			}
			var manyType = memberType.DetermineCollectionElementType();
			if (manyType.IsGenericType && typeof(KeyValuePair<,>) == manyType.GetGenericTypeDefinition())
			{
				var mapKey = memberType.DetermineDictionaryKeyType();
				if (domainInspector.IsManyToMany(subject.ReflectedType, mapKey))
				{
					return false;
				}
				var mapValue = memberType.DetermineDictionaryValueType();
				if (domainInspector.IsManyToMany(subject.ReflectedType, mapValue) || domainInspector.IsOneToMany(subject.ReflectedType, mapValue))
				{
					return false;
				}
				if (domainInspector.IsComponent(mapKey) || domainInspector.IsComponent(mapValue))
				{
					return false;
				}
			}

			return !domainInspector.IsManyToMany(subject.ReflectedType, manyType) && !domainInspector.IsOneToMany(subject.ReflectedType, manyType)
				&& !domainInspector.IsComponent(manyType);
		}

		#endregion
	}
}