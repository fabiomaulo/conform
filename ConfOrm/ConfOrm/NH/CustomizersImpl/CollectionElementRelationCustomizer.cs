using System;
using NHibernate.Mapping.ByCode;

namespace ConfOrm.NH.CustomizersImpl
{
	public class CollectionElementRelationCustomizer<TElement> : ICollectionElementRelation<TElement>
	{
		private readonly PropertyPath propertyPath;
		private readonly ICustomizersHolder customizersHolder;

		public CollectionElementRelationCustomizer(PropertyPath propertyPath, ICustomizersHolder customizersHolder)
		{
			this.propertyPath = propertyPath;
			this.customizersHolder = customizersHolder;
		}

		public void Element()
		{
			Element(x => { });
		}

		public void Element(Action<IElementMapper> mapping)
		{
			var collectionElementCustomizer = new CollectionElementCustomizer(propertyPath, customizersHolder);
			mapping(collectionElementCustomizer);
		}

		public void OneToMany()
		{
			OneToMany(x => { });
		}

		public void OneToMany(Action<IOneToManyMapper> mapping)
		{
			var oneToManyCustomizer = new OneToManyCustomizer(propertyPath, customizersHolder);
			mapping(oneToManyCustomizer);
		}

		public void ManyToMany()
		{
			ManyToMany(x => { });
		}

		public void ManyToMany(Action<IManyToManyMapper> mapping)
		{
			var manyToManyCustomizer = new ManyToManyCustomizer(propertyPath, customizersHolder);
			mapping(manyToManyCustomizer);
		}

		public void Component(Action<IComponentElementMapper<TElement>> mapping)
		{
			var componetElementCustomizer = new ComponentElementCustomizer<TElement>(propertyPath, customizersHolder);
			mapping(componetElementCustomizer);
		}

		public void ManyToAny(Type idTypeOfMetaType, Action<IManyToAnyMapper> mapping)
		{
			if (mapping == null)
			{
				throw new ArgumentNullException("mapping");
			}
			var manyToAnyCustomizer = new ManyToAnyCustomizer(propertyPath, customizersHolder);
			mapping(manyToAnyCustomizer);
		}

		public void ManyToAny<TIdTypeOfMetaType>(Action<IManyToAnyMapper> mapping)
		{
			ManyToAny(typeof(TIdTypeOfMetaType), mapping);
		}
	}
}