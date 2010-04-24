using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ConfOrm.Mappers
{
	public interface IComponentMapKeyMapper
	{
		void Property(MemberInfo property, Action<IKeyPropertyMapper> mapping);

		void ManyToOne(MemberInfo property, Action<IKeyManyToOneMapper> mapping);
	}

	public interface IComponentMapKeyMapper<TComponent>
	{
		void Property<TProperty>(Expression<Func<TComponent, TProperty>> property, Action<IKeyPropertyMapper> mapping);

		void ManyToOne<TProperty>(Expression<Func<TComponent, TProperty>> property, Action<IKeyManyToOneMapper> mapping) where TProperty : class;
	}
}