using System;
using System.Collections.Generic;
using System.Linq;
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
			foreach (var type in types.Where(type => domainInspector.IsEntity(type) && domainInspector.IsRootEntity(type)))
			{
				if (domainInspector.IsTablePerClass(type))
				{
					
				}
			}
			return mapping;
		}
	}
}