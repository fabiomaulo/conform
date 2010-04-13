using System;
using System.Collections.Generic;
using System.Reflection;
using ConfOrm.Mappers;
using ConfOrm.NH;

namespace ConfOrm.Shop.CoolNaming
{
	public class CollectionOfElementsKeyColumnApplier: IPatternApplier<PropertyPath, ICollectionPropertiesMapper>
	{
				private readonly IDomainInspector domainInspector;

		public CollectionOfElementsKeyColumnApplier(IDomainInspector domainInspector)
		{
			if (domainInspector == null)
			{
				throw new ArgumentNullException("domainInspector");
			}
			this.domainInspector = domainInspector;
		}

		#region Implementation of IPattern<PropertyPath>

		public bool Match(PropertyPath subject)
		{
			MemberInfo localMember = subject.LocalMember;
			var memberType = localMember.GetPropertyOrFieldType();
			if (!memberType.IsGenericCollection())
			{
				return false;
			}
			var manyType = memberType.DetermineCollectionElementType();
			if (manyType.IsGenericType && typeof(KeyValuePair<,>) == manyType.GetGenericTypeDefinition())
			{
				var mapKey = memberType.DetermineDictionaryKeyType();
				if(domainInspector.IsManyToMany(localMember.ReflectedType, mapKey))
				{
					return false;
				}
				var mapValue = memberType.DetermineDictionaryValueType();
				if (domainInspector.IsManyToMany(localMember.ReflectedType, mapValue) || domainInspector.IsOneToMany(localMember.ReflectedType, mapValue))
				{
					return false;
				}
				if(domainInspector.IsComponent(mapKey) || domainInspector.IsComponent(mapValue))
				{
					return false;
				}
			}

			return !domainInspector.IsManyToMany(localMember.ReflectedType, manyType) && !domainInspector.IsOneToMany(localMember.ReflectedType, manyType)
				&& !domainInspector.IsComponent(manyType);
		}

		#endregion

		#region Implementation of IPatternApplier<PropertyPath,ICollectionPropertiesMapper>

		public void Apply(PropertyPath subject, ICollectionPropertiesMapper applyTo)
		{
			applyTo.Key(km => km.Column(GetKeyColumnName(subject)));
		}

		#endregion

		protected virtual string GetKeyColumnName(PropertyPath subject)
		{
			string baseName = GetBaseName(subject);
			return string.Format("{0}Id", baseName);
		}

		protected virtual string GetBaseName(PropertyPath subject)
		{
			var entity = subject.GetContainerEntity(domainInspector);
			return entity.Name;
		}
	}
}