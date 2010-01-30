using System;
using System.Linq.Expressions;

namespace ConfOrm
{
	public interface IObjectRelationalMapper
	{
		void TablePerClassHierarchy<TBaseEntity>() where TBaseEntity : class;
		void TablePerClass<TBaseEntity>() where TBaseEntity : class;
		void TablePerConcreteClass<TBaseEntity>() where TBaseEntity : class;
		void Component<TComponent>();
		void ManyToMany<TLeftEntity, TRigthEntity>();
		void ManyToOne<TLeftEntity, TRigthEntity>();
		void OneToOne<TLeftEntity, TRigthEntity>();
		void Set<TEntity>(Expression<Func<TEntity, object>> propertyGetter);
		void Bag<TEntity>(Expression<Func<TEntity, object>> propertyGetter);
		void List<TEntity>(Expression<Func<TEntity, object>> propertyGetter);
		void Array<TEntity>(Expression<Func<TEntity, object>> propertyGetter);
	}
}