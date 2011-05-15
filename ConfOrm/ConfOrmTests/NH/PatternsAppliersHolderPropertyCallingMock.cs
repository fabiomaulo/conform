using System;
using System.Collections.Generic;
using System.Reflection;
using ConfOrm;
using NHibernate.Mapping.ByCode;
using ConfOrm.NH;

namespace ConfOrmTests.NH
{
	public class PatternsAppliersHolderPropertyCallingMock : IPatternsAppliersHolder
	{
		private readonly ICollection<IPatternApplier<MemberInfo, IAnyMapper>> any;

		private readonly ICollection<IPatternApplier<PropertyPath, IAnyMapper>> anyPath;

		private readonly ICollection<IPatternApplier<MemberInfo, IBagPropertiesMapper>> bag;

		private readonly ICollection<IPatternApplier<PropertyPath, IBagPropertiesMapper>> bagPath;
		private readonly ICollection<IPatternApplier<MemberInfo, ICollectionPropertiesMapper>> collection;

		private readonly ICollection<IPatternApplier<PropertyPath, ICollectionPropertiesMapper>> collectionPath;
		private readonly ICollection<IPatternApplier<Type, IComponentAttributesMapper>> component;

		private readonly ICollection<IPatternApplier<MemberInfo, IComponentParentMapper>> componentParent;

		private readonly ICollection<IPatternApplier<MemberInfo, IComponentAttributesMapper>> componentProperty;

		private readonly ICollection<IPatternApplier<PropertyPath, IComponentAttributesMapper>> componentPropertyPath;

		private readonly ICollection<IPatternApplier<MemberInfo, IElementMapper>> element;

		private readonly ICollection<IPatternApplier<PropertyPath, IElementMapper>> elementPath;
		private readonly ICollection<IPatternApplier<Type, IJoinedSubclassAttributesMapper>> joinedSubclass;
		private readonly ICollection<IPatternApplier<MemberInfo, IListPropertiesMapper>> list;

		private readonly ICollection<IPatternApplier<PropertyPath, IListPropertiesMapper>> listPath;
		private readonly ICollection<IPatternApplier<MemberInfo, IManyToManyMapper>> manyToMany;

		private readonly ICollection<IPatternApplier<PropertyPath, IManyToManyMapper>> manyToManyPath;
		private readonly ICollection<IPatternApplier<MemberInfo, IManyToOneMapper>> manyToOne;

		private readonly ICollection<IPatternApplier<PropertyPath, IManyToOneMapper>> manyToOnePath;
		private readonly ICollection<IPatternApplier<MemberInfo, IMapPropertiesMapper>> map;
		private readonly ICollection<IPatternApplier<MemberInfo, IMapKeyMapper>> mapKey;

		private readonly ICollection<IPatternApplier<MemberInfo, IMapKeyManyToManyMapper>> mapKeyManyToMany;

		private readonly ICollection<IPatternApplier<PropertyPath, IMapKeyManyToManyMapper>> mapKeyManyToManyPath;

		private readonly ICollection<IPatternApplier<PropertyPath, IMapKeyMapper>> mapKeyPath;
		private readonly ICollection<IPatternApplier<PropertyPath, IMapPropertiesMapper>> mapPath;
		private readonly ICollection<IPatternApplier<MemberInfo, IOneToManyMapper>> oneToMany;

		private readonly ICollection<IPatternApplier<PropertyPath, IOneToManyMapper>> oneToManyPath;
		private readonly ICollection<IPatternApplier<MemberInfo, IOneToOneMapper>> oneToOne;

		private readonly ICollection<IPatternApplier<PropertyPath, IOneToOneMapper>> oneToOnePath;
		private readonly ICollection<IPatternApplier<MemberInfo, IIdMapper>> poid;
		private readonly ICollection<IPatternApplier<MemberInfo, IPropertyMapper>> property;

		private readonly ICollection<IPatternApplier<PropertyPath, IPropertyMapper>> propertyPath;
		private readonly ICollection<IPatternApplier<Type, IClassAttributesMapper>> rootClass;
		private readonly ICollection<IPatternApplier<MemberInfo, ISetPropertiesMapper>> set;

		private readonly ICollection<IPatternApplier<PropertyPath, ISetPropertiesMapper>> setPath;
		private readonly ICollection<IPatternApplier<Type, ISubclassAttributesMapper>> subclass;

		private readonly ICollection<IPatternApplier<Type, IUnionSubclassAttributesMapper>> unionSubclass;
		private readonly ICollection<IPatternApplier<MemberInfo, IVersionMapper>> version;

		public PatternsAppliersHolderPropertyCallingMock()
		{
			PropertiesGettersUsed = new HashSet<string>();

			rootClass = new List<IPatternApplier<Type, IClassAttributesMapper>>();
			joinedSubclass = new List<IPatternApplier<Type, IJoinedSubclassAttributesMapper>>();
			subclass = new List<IPatternApplier<Type, ISubclassAttributesMapper>>();
			unionSubclass = new List<IPatternApplier<Type, IUnionSubclassAttributesMapper>>();
			component = new List<IPatternApplier<Type, IComponentAttributesMapper>>();
			componentProperty = new List<IPatternApplier<MemberInfo, IComponentAttributesMapper>>();
			componentPropertyPath = new List<IPatternApplier<PropertyPath, IComponentAttributesMapper>>();

			poid = new List<IPatternApplier<MemberInfo, IIdMapper>>();
			property = new List<IPatternApplier<MemberInfo, IPropertyMapper>>();
			collection = new List<IPatternApplier<MemberInfo, ICollectionPropertiesMapper>>();
			propertyPath = new List<IPatternApplier<PropertyPath, IPropertyMapper>>();
			componentParent = new List<IPatternApplier<MemberInfo, IComponentParentMapper>>();

			collectionPath = new List<IPatternApplier<PropertyPath, ICollectionPropertiesMapper>>();
			bag = new List<IPatternApplier<MemberInfo, IBagPropertiesMapper>>();
			bagPath = new List<IPatternApplier<PropertyPath, IBagPropertiesMapper>>();
			set = new List<IPatternApplier<MemberInfo, ISetPropertiesMapper>>();
			setPath = new List<IPatternApplier<PropertyPath, ISetPropertiesMapper>>();
			list = new List<IPatternApplier<MemberInfo, IListPropertiesMapper>>();
			listPath = new List<IPatternApplier<PropertyPath, IListPropertiesMapper>>();
			map = new List<IPatternApplier<MemberInfo, IMapPropertiesMapper>>();
			mapPath = new List<IPatternApplier<PropertyPath, IMapPropertiesMapper>>();

			manyToOne = new List<IPatternApplier<MemberInfo, IManyToOneMapper>>();
			manyToOnePath = new List<IPatternApplier<PropertyPath, IManyToOneMapper>>();
			oneToOne = new List<IPatternApplier<MemberInfo, IOneToOneMapper>>();
			oneToOnePath = new List<IPatternApplier<PropertyPath, IOneToOneMapper>>();
			any = new List<IPatternApplier<MemberInfo, IAnyMapper>>();
			anyPath = new List<IPatternApplier<PropertyPath, IAnyMapper>>();
			manyToMany = new List<IPatternApplier<MemberInfo, IManyToManyMapper>>();
			manyToManyPath = new List<IPatternApplier<PropertyPath, IManyToManyMapper>>();
			element = new List<IPatternApplier<MemberInfo, IElementMapper>>();
			elementPath = new List<IPatternApplier<PropertyPath, IElementMapper>>();
			oneToMany = new List<IPatternApplier<MemberInfo, IOneToManyMapper>>();
			oneToManyPath = new List<IPatternApplier<PropertyPath, IOneToManyMapper>>();
			version = new List<IPatternApplier<MemberInfo, IVersionMapper>>();

			mapKey = new List<IPatternApplier<MemberInfo, IMapKeyMapper>>();
			mapKeyPath = new List<IPatternApplier<PropertyPath, IMapKeyMapper>>();
			mapKeyManyToMany = new List<IPatternApplier<MemberInfo, IMapKeyManyToManyMapper>>();
			mapKeyManyToManyPath = new List<IPatternApplier<PropertyPath, IMapKeyManyToManyMapper>>();
		}

		public HashSet<string> PropertiesGettersUsed { get; set; }

		#region IPatternsAppliersHolder Members

		public ICollection<IPatternApplier<Type, IClassAttributesMapper>> RootClass
		{
			get
			{
				PropertiesGettersUsed.Add("RootClass");
				return rootClass;
			}
		}

		public ICollection<IPatternApplier<Type, IJoinedSubclassAttributesMapper>> JoinedSubclass
		{
			get
			{
				PropertiesGettersUsed.Add("JoinedSubclass");
				return joinedSubclass;
			}
		}

		public ICollection<IPatternApplier<Type, ISubclassAttributesMapper>> Subclass
		{
			get
			{
				PropertiesGettersUsed.Add("Subclass");
				return subclass;
			}
		}

		public ICollection<IPatternApplier<Type, IUnionSubclassAttributesMapper>> UnionSubclass
		{
			get
			{
				PropertiesGettersUsed.Add("UnionSubclass");
				return unionSubclass;
			}
		}

		public ICollection<IPatternApplier<Type, IComponentAttributesMapper>> Component
		{
			get
			{
				PropertiesGettersUsed.Add("Component");
				return component;
			}
		}

		public ICollection<IPatternApplier<MemberInfo, IIdMapper>> Poid
		{
			get
			{
				PropertiesGettersUsed.Add("Poid");
				return poid;
			}
		}

		public ICollection<IPatternApplier<MemberInfo, IVersionMapper>> Version
		{
			get
			{
				PropertiesGettersUsed.Add("Version");
				return version;
			}
		}

		public ICollection<IPatternApplier<MemberInfo, IPropertyMapper>> Property
		{
			get
			{
				PropertiesGettersUsed.Add("Property");
				return property;
			}
		}

		public ICollection<IPatternApplier<PropertyPath, IPropertyMapper>> PropertyPath
		{
			get
			{
				PropertiesGettersUsed.Add("PropertyPath");
				return propertyPath;
			}
		}

		public ICollection<IPatternApplier<MemberInfo, IManyToOneMapper>> ManyToOne
		{
			get
			{
				PropertiesGettersUsed.Add("ManyToOne");
				return manyToOne;
			}
		}

		public ICollection<IPatternApplier<PropertyPath, IManyToOneMapper>> ManyToOnePath
		{
			get
			{
				PropertiesGettersUsed.Add("ManyToOnePath");
				return manyToOnePath;
			}
		}

		public ICollection<IPatternApplier<MemberInfo, IOneToOneMapper>> OneToOne
		{
			get
			{
				PropertiesGettersUsed.Add("OneToOne");
				return oneToOne;
			}
		}

		public ICollection<IPatternApplier<PropertyPath, IOneToOneMapper>> OneToOnePath
		{
			get
			{
				PropertiesGettersUsed.Add("OneToOnePath");
				return oneToOnePath;
			}
		}

		public ICollection<IPatternApplier<MemberInfo, IAnyMapper>> Any
		{
			get
			{
				PropertiesGettersUsed.Add("Any");
				return any;
			}
		}

		public ICollection<IPatternApplier<PropertyPath, IAnyMapper>> AnyPath
		{
			get
			{
				PropertiesGettersUsed.Add("AnyPath");
				return anyPath;
			}
		}

		public ICollection<IPatternApplier<MemberInfo, ICollectionPropertiesMapper>> Collection
		{
			get
			{
				PropertiesGettersUsed.Add("Collection");
				return collection;
			}
		}

		public ICollection<IPatternApplier<PropertyPath, ICollectionPropertiesMapper>> CollectionPath
		{
			get
			{
				PropertiesGettersUsed.Add("CollectionPath");
				return collectionPath;
			}
		}

		public ICollection<IPatternApplier<MemberInfo, IBagPropertiesMapper>> Bag
		{
			get
			{
				PropertiesGettersUsed.Add("Bag");
				return bag;
			}
		}

		public ICollection<IPatternApplier<PropertyPath, IBagPropertiesMapper>> BagPath
		{
			get
			{
				PropertiesGettersUsed.Add("BagPath");
				return bagPath;
			}
		}

		public ICollection<IPatternApplier<MemberInfo, ISetPropertiesMapper>> Set
		{
			get
			{
				PropertiesGettersUsed.Add("Set");
				return set;
			}
		}

		public ICollection<IPatternApplier<PropertyPath, ISetPropertiesMapper>> SetPath
		{
			get
			{
				PropertiesGettersUsed.Add("SetPath");
				return setPath;
			}
		}

		public ICollection<IPatternApplier<MemberInfo, IListPropertiesMapper>> List
		{
			get
			{
				PropertiesGettersUsed.Add("List");
				return list;
			}
		}

		public ICollection<IPatternApplier<PropertyPath, IListPropertiesMapper>> ListPath
		{
			get
			{
				PropertiesGettersUsed.Add("ListPath");
				return listPath;
			}
		}

		public ICollection<IPatternApplier<MemberInfo, IMapPropertiesMapper>> Map
		{
			get
			{
				PropertiesGettersUsed.Add("Map");
				return map;
			}
		}

		public ICollection<IPatternApplier<PropertyPath, IMapPropertiesMapper>> MapPath
		{
			get
			{
				PropertiesGettersUsed.Add("MapPath");
				return mapPath;
			}
		}

		public ICollection<IPatternApplier<MemberInfo, IComponentParentMapper>> ComponentParent
		{
			get
			{
				PropertiesGettersUsed.Add("ComponentParent");
				return componentParent;
			}
		}

		public ICollection<IPatternApplier<MemberInfo, IComponentAttributesMapper>> ComponentProperty
		{
			get
			{
				PropertiesGettersUsed.Add("ComponentProperty");
				return componentProperty;
			}
		}

		public ICollection<IPatternApplier<PropertyPath, IComponentAttributesMapper>> ComponentPropertyPath
		{
			get
			{
				PropertiesGettersUsed.Add("ComponentPropertyPath");
				return componentPropertyPath;
			}
		}

		public ICollection<IPatternApplier<MemberInfo, IManyToManyMapper>> ManyToMany
		{
			get
			{
				PropertiesGettersUsed.Add("ManyToMany");
				return manyToMany;
			}
		}

		public ICollection<IPatternApplier<PropertyPath, IManyToManyMapper>> ManyToManyPath
		{
			get
			{
				PropertiesGettersUsed.Add("ManyToManyPath");
				return manyToManyPath;
			}
		}

		public ICollection<IPatternApplier<MemberInfo, IElementMapper>> Element
		{
			get
			{
				PropertiesGettersUsed.Add("Element");
				return element;
			}
		}

		public ICollection<IPatternApplier<PropertyPath, IElementMapper>> ElementPath
		{
			get
			{
				PropertiesGettersUsed.Add("ElementPath");
				return elementPath;
			}
		}

		public ICollection<IPatternApplier<MemberInfo, IOneToManyMapper>> OneToMany
		{
			get
			{
				PropertiesGettersUsed.Add("OneToMany");
				return oneToMany;
			}
		}

		public ICollection<IPatternApplier<PropertyPath, IOneToManyMapper>> OneToManyPath
		{
			get
			{
				PropertiesGettersUsed.Add("OneToManyPath");
				return oneToManyPath;
			}
		}

		public ICollection<IPatternApplier<MemberInfo, IMapKeyManyToManyMapper>> MapKeyManyToMany
		{
			get
			{
				PropertiesGettersUsed.Add("MapKeyManyToMany");
				return mapKeyManyToMany;
			}
		}

		public ICollection<IPatternApplier<PropertyPath, IMapKeyManyToManyMapper>> MapKeyManyToManyPath
		{
			get
			{
				PropertiesGettersUsed.Add("MapKeyManyToManyPath");
				return mapKeyManyToManyPath;
			}
		}

		public ICollection<IPatternApplier<MemberInfo, IMapKeyMapper>> MapKey
		{
			get
			{
				PropertiesGettersUsed.Add("MapKey");
				return mapKey;
			}
		}

		public ICollection<IPatternApplier<PropertyPath, IMapKeyMapper>> MapKeyPath
		{
			get
			{
				PropertiesGettersUsed.Add("MapKeyPath");
				return mapKeyPath;
			}
		}

		#endregion
	}
}