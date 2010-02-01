using System;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public class CollectionElementRelation : ICollectionElementRelation
	{
		private readonly Type collectionElementType;
		private readonly HbmMapping mapDoc;
		private readonly Action<object> elementRelationshipAssing;

		public CollectionElementRelation(Type collectionElementType, HbmMapping mapDoc, Action<object> elementRelationshipAssing)
		{
			this.collectionElementType = collectionElementType;
			this.mapDoc = mapDoc;
			this.elementRelationshipAssing = elementRelationshipAssing;
		}

		#region Implementation of ICollectionElementRelation

		public void Element()
		{
			var hbm = new HbmElement { type1 = collectionElementType.GetTypeName() };
			elementRelationshipAssing(hbm);
		}

		public void OneToMany()
		{
			var hbm = new HbmOneToMany { @class = collectionElementType.AssemblyQualifiedName };
			elementRelationshipAssing(hbm);
		}

		public void ManyToMany()
		{
			var hbm = new HbmManyToMany { @class = collectionElementType.AssemblyQualifiedName };
			elementRelationshipAssing(hbm);
		}

		public void Component(Action<IComponentElementMapper> mapping)
		{
			var hbm = new HbmCompositeElement { @class = collectionElementType.AssemblyQualifiedName };
			mapping(new ComponentElementMapper(collectionElementType, mapDoc, hbm));
			elementRelationshipAssing(hbm);
		}

		#endregion
	}
}