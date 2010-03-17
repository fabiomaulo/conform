using System;
using System.Collections.Generic;
using System.Reflection;
using ConfOrm.Mappers;
using ConfOrm.Patterns;

namespace ConfOrm.NH
{
	public class DefaultPatternsAppliersHolder: IPatternsAppliersHolder
	{
		private readonly List<IPatternApplier<Type, IClassAttributesMapper>> rootClass;
		private readonly List<IPatternApplier<Type, IJoinedSubclassAttributesMapper>> joinedSubclass;
		private readonly List<IPatternApplier<Type, ISubclassAttributesMapper>> subclass;
		private readonly List<IPatternApplier<Type, IUnionSubclassAttributesMapper>> unionSubclass;

		private readonly List<IPatternApplier<MemberInfo, IIdMapper>> poid;
		private readonly List<IPatternApplier<MemberInfo, IPropertyMapper>> property;
		private readonly List<IPatternApplier<MemberInfo, IManyToOneMapper>> manyToOne;
		private readonly List<IPatternApplier<MemberInfo, IOneToOneMapper>> oneToOne;
		private readonly List<IPatternApplier<MemberInfo, ICollectionPropertiesMapper>> collection;
		private readonly List<IPatternApplier<PropertyPath, IPropertyMapper>> propertyPath;
		private readonly List<IPatternApplier<PropertyPath, IManyToOneMapper>> manyToOnePath;
		private readonly List<IPatternApplier<PropertyPath, ICollectionPropertiesMapper>> collectionPath;
		private readonly List<IPatternApplier<PropertyPath, IOneToOneMapper>> oneToOnePath;

		public DefaultPatternsAppliersHolder(IDomainInspector domainInspector)
		{
			if (domainInspector == null)
			{
				throw new ArgumentNullException("domainInspector");
			}
			rootClass = new List<IPatternApplier<Type, IClassAttributesMapper>>();
			joinedSubclass = new List<IPatternApplier<Type, IJoinedSubclassAttributesMapper>>();
			subclass = new List<IPatternApplier<Type, ISubclassAttributesMapper>>();
			unionSubclass = new List<IPatternApplier<Type, IUnionSubclassAttributesMapper>>();
			poid = new List<IPatternApplier<MemberInfo, IIdMapper>>
			       	{
			       		new NoSetterPoidToFieldAccessorApplier(),
			       		new NoPoidGuidApplier(),
			       		new BidirectionalOneToOneAssociationPoidApplier(domainInspector)
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
																			new BidirectionalManyToManyInverseApplier(domainInspector),
			                             	};
			propertyPath = new List<IPatternApplier<PropertyPath, IPropertyMapper>>
			               	{new ComponentMultiUsagePropertyColumnNameApplier(),};

			collectionPath = new List<IPatternApplier<PropertyPath, ICollectionPropertiesMapper>>();
			manyToOne = new List<IPatternApplier<MemberInfo, IManyToOneMapper>>
			            	{
			            		new BidirectionalForeignKeyAssociationManyToOneApplier(domainInspector),
			            		new UnidirectionalOneToOneUniqueCascadeApplier(domainInspector)
			            	};
			manyToOnePath = new List<IPatternApplier<PropertyPath, IManyToOneMapper>>
			                	{new ComponentMultiUsageManyToOneColumnNameApplier()};
			oneToOne = new List<IPatternApplier<MemberInfo, IOneToOneMapper>>
			           	{
			           		new BidirectionalForeignKeyAssociationOneToOneApplier(domainInspector),
										new BidirectionalPrimaryKeyAssociationMasterOneToOneApplier(domainInspector),
										new BidirectionalPrimaryKeyAssociationSlaveOneToOneApplier(domainInspector)
			           	};
			oneToOnePath = new List<IPatternApplier<PropertyPath, IOneToOneMapper>>();
		}

		#region Implementation of IPatternsAppliersHolder

		public ICollection<IPatternApplier<Type, IClassAttributesMapper>> RootClass
		{
			get { return rootClass; }
		}

		public ICollection<IPatternApplier<Type, IJoinedSubclassAttributesMapper>> JoinedSubclass
		{
			get { return joinedSubclass; }
		}

		public ICollection<IPatternApplier<Type, ISubclassAttributesMapper>> Subclass
		{
			get { return subclass; }
		}

		public ICollection<IPatternApplier<Type, IUnionSubclassAttributesMapper>> UnionSubclass
		{
			get { return unionSubclass; }
		}

		public ICollection<IPatternApplier<MemberInfo, IIdMapper>> Poid
		{
			get { return poid; }
		}

		public ICollection<IPatternApplier<MemberInfo, IPropertyMapper>> Property
		{
			get { return property; }
		}

		public ICollection<IPatternApplier<MemberInfo, IManyToOneMapper>> ManyToOne
		{
			get { return manyToOne; }
		}

		public ICollection<IPatternApplier<MemberInfo, ICollectionPropertiesMapper>> Collection
		{
			get { return collection; }
		}

		public ICollection<IPatternApplier<PropertyPath, IPropertyMapper>> PropertyPath
		{
			get { return propertyPath; }
		}

		public ICollection<IPatternApplier<PropertyPath, IManyToOneMapper>> ManyToOnePath
		{
			get { return manyToOnePath; }
		}

		public ICollection<IPatternApplier<PropertyPath, ICollectionPropertiesMapper>> CollectionPath
		{
			get { return collectionPath; }
		}

		public ICollection<IPatternApplier<MemberInfo, IOneToOneMapper>> OneToOne
		{
			get { return oneToOne; }
		}

		public ICollection<IPatternApplier<PropertyPath, IOneToOneMapper>> OneToOnePath
		{
			get { return oneToOnePath; }
		}

		#endregion
	}
}