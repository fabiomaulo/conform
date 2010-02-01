using System;

namespace ConfOrm.Mappers
{
	public interface ICollectionElementRelation
	{
		void Element();
		void OneToMany();
		void ManyToMany();
		void Component(Action<IComponentElementMapper> mapping);
	}
}