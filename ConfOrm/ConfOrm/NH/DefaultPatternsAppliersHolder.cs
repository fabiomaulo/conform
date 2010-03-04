using System;
using System.Collections.Generic;
using System.Reflection;
using ConfOrm.Mappers;
using ConfOrm.Patterns;

namespace ConfOrm.NH
{
	public class DefaultPatternsAppliersHolder: IPatternsAppliersHolder
	{
		private readonly List<IPatternApplier<MemberInfo, IIdMapper>> poid;
		private readonly List<IPatternApplier<MemberInfo, IPropertyMapper>> property;
		private readonly List<IPatternApplier<MemberInfo, ICollectionPropertiesMapper>> collection;
		private readonly List<IPatternApplier<PropertyPath, IPropertyMapper>> propertyPath;
		private readonly List<IPatternApplier<PropertyPath, ICollectionPropertiesMapper>> collectionPath;

		public DefaultPatternsAppliersHolder(IDomainInspector domainInspector)
		{
			if (domainInspector == null)
			{
				throw new ArgumentNullException("domainInspector");
			}
			poid = new List<IPatternApplier<MemberInfo, IIdMapper>>
			                       	{
			                       		new NoSetterPoidToFieldAccessorApplier(),
																new NoPoidGuidApplier()
			                       	};
			property = new List<IPatternApplier<MemberInfo, IPropertyMapper>>
			                           	{
			                           		new ReadOnlyPropertyAccessorApplier(),
			                           		new NoSetterPropertyToFieldAccessorApplier(),
			                           		new PropertyToFieldAccessorApplier()
			                           	};
			collection = new List<IPatternApplier<MemberInfo, ICollectionPropertiesMapper>>
			                             	{
			                             		new ReadOnlyCollectionPropertyAccessorApplier(),
			                             		new NoSetterCollectionPropertyToFieldAccessorApplier(),
			                             		new CollectionPropertyToFieldAccessorApplier(),
			                             		new BidirectionalOneToManyApplier(domainInspector),
			                             		new BidirectionalOneToManyOnDeleteConstraintApplier(domainInspector),
			                             		new BidirectionalManyToManyTableApplier(),
			                             	};
			propertyPath = new List<IPatternApplier<PropertyPath, IPropertyMapper>>();
			collectionPath = new List<IPatternApplier<PropertyPath, ICollectionPropertiesMapper>>();
		}

		#region Implementation of IPatternsAppliersHolder

		public ICollection<IPatternApplier<MemberInfo, IIdMapper>> Poid
		{
			get { return poid; }
		}

		public ICollection<IPatternApplier<MemberInfo, IPropertyMapper>> Property
		{
			get { return property; }
		}

		public ICollection<IPatternApplier<MemberInfo, ICollectionPropertiesMapper>> Collection
		{
			get { return collection; }
		}

		public ICollection<IPatternApplier<PropertyPath, IPropertyMapper>> PropertyPath
		{
			get { return propertyPath; }
		}

		public ICollection<IPatternApplier<PropertyPath, ICollectionPropertiesMapper>> CollectionPath
		{
			get { return collectionPath; }
		}

		#endregion
	}
}