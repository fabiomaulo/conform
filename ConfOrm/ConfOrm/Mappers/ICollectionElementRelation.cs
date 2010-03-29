using System;

namespace ConfOrm.Mappers
{
	public interface ICollectionElementRelation
	{
		void Element();
		void OneToMany();
		void ManyToMany(Action<IManyToManyMapper> mapping);
		void Component(Action<IComponentElementMapper> mapping);
	}

	public interface ICollectionElementRelation<TElement>
	{
		void Element();
		void OneToMany();
		void ManyToMany(Action<IManyToManyMapper> mapping);
		void Component(Action<IComponentElementMapper<TElement>> mapping);
	}
}