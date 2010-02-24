using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ConfOrm.Mappers;

namespace ConfOrm.NH
{
	public class PersistentClassCustomizer<TPersistent> : IPersistentClassCustomizer<TPersistent> where TPersistent : class
	{
		private readonly ICustomizersHolder customizersHolder;

		public PersistentClassCustomizer(ICustomizersHolder customizersHolder)
		{
			this.customizersHolder = customizersHolder;
		}

		#region Implementation of IPersistentClassCustomizer<TPersistent>

		public void Property<TProperty>(Expression<Func<TPersistent, TProperty>> property, Action<IPropertyMapper> mapping)
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(property);
			customizersHolder.AddCustomizer(member,mapping);
		}

		public void ManyToOne<TProperty>(Expression<Func<TPersistent, TProperty>> property, Action<IManyToOneMapper> mapping) where TProperty : class
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(property);
			customizersHolder.AddCustomizer(member,mapping);
			
		}

		public void OneToOne<TProperty>(Expression<Func<TPersistent, TProperty>> property, Action<IOneToOneMapper> mapping) where TProperty : class
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(property);
			customizersHolder.AddCustomizer(member,mapping);
		}

		public void Collection<TElement>(Expression<Func<TPersistent, IEnumerable<TElement>>> property, Action<ICollectionPropertiesMapper> mapping)
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(property);
			customizersHolder.AddCustomizer(member, mapping);
		}

		#endregion
	}
}