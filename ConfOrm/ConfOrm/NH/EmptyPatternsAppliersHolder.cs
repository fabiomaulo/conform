using System;
using System.Collections.Generic;
using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.NH
{
	public class EmptyPatternsAppliersHolder : IPatternsAppliersHolder
	{
		protected List<IPatternApplier<Type, IClassAttributesMapper>> rootClass;
		protected List<IPatternApplier<Type, IJoinedSubclassAttributesMapper>> joinedSubclass;
		protected List<IPatternApplier<Type, ISubclassAttributesMapper>> subclass;
		protected List<IPatternApplier<Type, IUnionSubclassAttributesMapper>> unionSubclass;
		protected List<IPatternApplier<MemberInfo, IIdMapper>> poid;
		protected List<IPatternApplier<MemberInfo, IPropertyMapper>> property;
		protected List<IPatternApplier<MemberInfo, IManyToOneMapper>> manyToOne;
		protected List<IPatternApplier<MemberInfo, IOneToOneMapper>> oneToOne;
		protected List<IPatternApplier<MemberInfo, ICollectionPropertiesMapper>> collection;
		protected List<IPatternApplier<PropertyPath, IPropertyMapper>> propertyPath;
		protected List<IPatternApplier<PropertyPath, IManyToOneMapper>> manyToOnePath;
		protected List<IPatternApplier<PropertyPath, ICollectionPropertiesMapper>> collectionPath;
		protected List<IPatternApplier<PropertyPath, IOneToOneMapper>> oneToOnePath;
		protected List<IPatternApplier<MemberInfo, IAnyMapper>> any;
		protected List<IPatternApplier<PropertyPath, IAnyMapper>> anyPath;
		protected List<IPatternApplier<MemberInfo, IManyToManyMapper>> manyToMany;
		protected List<IPatternApplier<PropertyPath, IManyToManyMapper>> manyToManyPath;

		public EmptyPatternsAppliersHolder()
		{
			rootClass = new List<IPatternApplier<Type, IClassAttributesMapper>>();
			joinedSubclass = new List<IPatternApplier<Type, IJoinedSubclassAttributesMapper>>();
			subclass = new List<IPatternApplier<Type, ISubclassAttributesMapper>>();
			unionSubclass = new List<IPatternApplier<Type, IUnionSubclassAttributesMapper>>();
			poid = new List<IPatternApplier<MemberInfo, IIdMapper>>();
			property = new List<IPatternApplier<MemberInfo, IPropertyMapper>>();
			collection = new List<IPatternApplier<MemberInfo, ICollectionPropertiesMapper>>();
			propertyPath = new List<IPatternApplier<PropertyPath, IPropertyMapper>>();

			collectionPath = new List<IPatternApplier<PropertyPath, ICollectionPropertiesMapper>>();
			manyToOne = new List<IPatternApplier<MemberInfo, IManyToOneMapper>>();
			manyToOnePath = new List<IPatternApplier<PropertyPath, IManyToOneMapper>>();
			oneToOne = new List<IPatternApplier<MemberInfo, IOneToOneMapper>>();
			oneToOnePath = new List<IPatternApplier<PropertyPath, IOneToOneMapper>>();
			any = new List<IPatternApplier<MemberInfo, IAnyMapper>>();
			anyPath = new List<IPatternApplier<PropertyPath, IAnyMapper>>();
			manyToMany = new List<IPatternApplier<MemberInfo, IManyToManyMapper>>();
			manyToManyPath = new List<IPatternApplier<PropertyPath, IManyToManyMapper>>();
		}

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

		public ICollection<IPatternApplier<MemberInfo, IAnyMapper>> Any
		{
			get { return any; }
		}

		public ICollection<IPatternApplier<PropertyPath, IAnyMapper>> AnyPath
		{
			get { return anyPath; }
		}

		public ICollection<IPatternApplier<MemberInfo, IManyToManyMapper>> ManyToMany
		{
			get { return manyToMany; }
		}

		public ICollection<IPatternApplier<PropertyPath, IManyToManyMapper>> ManyToManyPath
		{
			get { return manyToManyPath; }
		}
	}
}