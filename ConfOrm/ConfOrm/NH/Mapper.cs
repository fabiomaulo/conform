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
				if (domainInspector.IsTablePerClass(type))
				{
					var classMapper = new ClassMapper(type, mapping, GetPoidPropertyOrField(type));
					new IdMapper(classMapper.Id);
					MapProperties(type, classMapper);
				}
			}
			// Map joined-subclass
			foreach (var type in types.Where(type => domainInspector.IsEntity(type) && !domainInspector.IsRootEntity(type)))
			{
				if (domainInspector.IsTablePerClass(type))
				{
					var classMapper = new JoinedSubclassMapper(type, mapping);
					MapProperties(type, classMapper);
				}
			}
			return mapping;
		}

		private void MapProperties(Type type, IPropertyContainerMapper propertiesContainer)
		{
			var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
			foreach (var property in properties.Where(p => domainInspector.IsPersistentProperty(p) && !domainInspector.IsPersistentId(p)))
			{
				propertiesContainer.Property(property);
			}
		}

		private MemberInfo GetPoidPropertyOrField(Type type)
		{
			return type.GetProperties().Cast<MemberInfo>().Concat(type.GetFields()).FirstOrDefault(mi=> domainInspector.IsPersistentId(mi));
		}
	}
}