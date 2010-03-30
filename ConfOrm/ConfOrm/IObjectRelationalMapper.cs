using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ConfOrm
{
	public interface IObjectRelationalMapper
	{
		void TablePerClass(IEnumerable<Type> baseEntities);
		void TablePerClassHierarchy(IEnumerable<Type> baseEntities);
		void TablePerConcreteClass(IEnumerable<Type> baseEntities);
		void TablePerClassHierarchy<TBaseEntity>() where TBaseEntity : class;
		void TablePerClass<TBaseEntity>() where TBaseEntity : class;
		void TablePerConcreteClass<TBaseEntity>() where TBaseEntity : class;
		
		void Component<TComponent>();
		void Complex<TComplex>();

		/// <summary>
		/// Exclude (jump) a class from a entity's hierarchy
		/// </summary>
		/// <typeparam name="TClass">The class to exclude.</typeparam>
		/// <remarks>
		/// Use this method when you need to "jump" a implementation inside an entity hierarchy.
		/// </remarks>
		/// <example>
		/// You need to map a interface, as root, jumping the first implementation in the hierarchy.
		/// <code>
		/// </code>
		/// </example>
		void Exclude<TClass>();

		void Poid<TEntity>(Expression<Func<TEntity, object>> propertyGetter);
		void NaturalId<TBaseEntity>(params Expression<Func<TBaseEntity, object>>[] propertiesGetters) where TBaseEntity : class;
		void ManyToMany<TLeftEntity, TRigthEntity>();
		void ManyToOne<TLeftEntity, TRigthEntity>();
		void OneToOne<TLeftEntity, TRigthEntity>();
		void Set<TEntity>(Expression<Func<TEntity, object>> propertyGetter);
		void Bag<TEntity>(Expression<Func<TEntity, object>> propertyGetter);
		void List<TEntity>(Expression<Func<TEntity, object>> propertyGetter);
		void Array<TEntity>(Expression<Func<TEntity, object>> propertyGetter);
		void Dictionary<TEntity>(Expression<Func<TEntity, object>> propertyGetter);
		void Complex<TEntity>(Expression<Func<TEntity, object>> propertyGetter);
		void HeterogeneousAssociation<TEntity>(Expression<Func<TEntity, object>> propertyGetter);
	
		void Cascade<TFromEntity, TToEntity>(Cascade cascadeOptions);
		
		void PersistentProperty<TEntity>(Expression<Func<TEntity, object>> propertyGetter);
		void ExcludeProperty<TEntity>(Expression<Func<TEntity, object>> propertyGetter);
		void VersionProperty<TEntity>(Expression<Func<TEntity, object>> propertyGetter);
	}
}