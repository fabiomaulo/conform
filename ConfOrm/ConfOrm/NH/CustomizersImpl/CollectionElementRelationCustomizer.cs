using System;
using ConfOrm.Mappers;

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

		public void Element(Action<IElementMapper> mapping)
		{
			var collectionElementCustomizer = new CollectionElementCustomizer(propertyPath, customizersHolder);
			mapping(collectionElementCustomizer);
		}

		public void OneToMany(Action<IOneToManyMapper> mapping)
		{
			var oneToManyCustomizer = new OneToManyCustomizer(propertyPath, customizersHolder);
			mapping(oneToManyCustomizer);
		}

		public void ManyToMany(Action<IManyToManyMapper> mapping)
		{
			throw new NotImplementedException();
		}

		public void Component(Action<IComponentElementMapper<TElement>> mapping)
		{
			throw new NotImplementedException();
		}
	}
}