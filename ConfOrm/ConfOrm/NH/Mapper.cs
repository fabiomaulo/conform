using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public class Mapper
	{
		private readonly HbmMapping mapping;
		private readonly IDomainInspector domainInspector;

		public Mapper(IDomainInspector domainInspector)
		{
			if (domainInspector == null)
			{
				throw new ArgumentNullException("domainInspector");
			}
			this.domainInspector = domainInspector;
			mapping = new HbmMapping();
		}

		public HbmMapping CompileMappingFor(IEnumerable<Type> types)
		{
			if (types == null)
			{
				throw new ArgumentNullException("types");
			}
			// Map root classes
			foreach (var type in types.Where(type => domainInspector.IsEntity(type) && domainInspector.IsRootEntity(type)))
			{
				var classMapper = new ClassMapper(type, mapping, GetPoidPropertyOrField(type));
				new IdMapper(classMapper.Id);
				if (domainInspector.IsTablePerClassHierarchy(type))
				{
					classMapper.Discriminator();
				}
				MapProperties(type, classMapper);
			}
			// Map subclasses
			foreach (var type in types.Where(type => domainInspector.IsEntity(type) && !domainInspector.IsRootEntity(type)))
			{
				IPropertyContainerMapper propertiesContainer = null;
				if (domainInspector.IsTablePerClass(type))
				{
					var classMapper = new JoinedSubclassMapper(type, mapping);
					propertiesContainer = classMapper;
				}
				else if (domainInspector.IsTablePerClassHierarchy(type))
				{
					var classMapper = new SubclassMapper(type, mapping);
					propertiesContainer = classMapper;
				}
				else if (domainInspector.IsTablePerConcreteClass(type))
				{
					var classMapper = new UnionSubclassMapper(type, mapping);
					propertiesContainer = classMapper;
				}
				MapProperties(type, propertiesContainer);
			}
			return mapping;
		}

		private void MapProperties(Type type, IPropertyContainerMapper propertiesContainer)
		{
			var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
			foreach (var property in properties.Where(p => domainInspector.IsPersistentProperty(p) && !domainInspector.IsPersistentId(p)))
			{
				var propertyType = property.GetPropertyOrFieldType();
				if(domainInspector.IsManyToOne(type, propertyType))
				{
					propertiesContainer.ManyToOne(property);
				}
				else if (domainInspector.IsOneToOne(type, propertyType))
				{
					propertiesContainer.OneToOne(property, x => { });
				}
				else if (domainInspector.IsComponent(propertyType))
				{
					propertiesContainer.Component(property, x =>
						{
							MapProperties(property.GetPropertyOrFieldType(), x);
						});
				}
				else if (domainInspector.IsSet(property))
				{
					propertiesContainer.Set(property, x => { }, x => { });
				}
				else if (domainInspector.IsArray(property))
				{
				}
				else if (domainInspector.IsList(property))
				{
				}
				else if (domainInspector.IsBag(property))
				{
				}
				else
				{
					propertiesContainer.Property(property);
				}
			}
		}

		private MemberInfo GetPoidPropertyOrField(Type type)
		{
			return type.GetProperties().Cast<MemberInfo>().Concat(type.GetFields()).FirstOrDefault(mi=> domainInspector.IsPersistentId(mi));
		}
	}
}