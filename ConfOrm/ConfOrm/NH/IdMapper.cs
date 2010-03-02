using System;
using System.Linq;
using System.Reflection;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public class IdMapper : IIdMapper
	{
		private readonly IAccessorPropertyMapper accessorMapper;
		private readonly HbmId hbmId;

		private class NoMemberPropertyMapper : IEntityPropertyMapper
		{
			public void Access(Accessor accessor) { }

			public void Access(Type accessorType) { }
		}

		public IdMapper(HbmId hbmId)
			: this(null, hbmId)
		{
		}

		public IdMapper(MemberInfo member, HbmId hbmId)
		{
			this.hbmId = hbmId;
			if (member != null)
			{
				var idType = member.GetPropertyOrFieldType();
				hbmId.name = member.Name;
				hbmId.type1 = idType.GetNhTypeName();
				accessorMapper = new AccessorPropertyMapper(member.DeclaringType, member.Name, x => hbmId.access = x);
			}
			else
			{
				accessorMapper = new NoMemberPropertyMapper();
			}
		}

		#region Implementation of IIdMapper

		public void Generator(IGeneratorDef generator)
		{
			Generator(generator, x => { });
		}

		private void ApplyGenerator(IGeneratorDef generator)
		{
			var hbmGenerator = new HbmGenerator { @class = generator.Class };
			object generatorParameters = generator.Params;
			if (generatorParameters != null)
			{
				hbmGenerator.param = (from pi in generatorParameters.GetType().GetProperties()
															let pname = pi.Name
															let pvalue = pi.GetValue(generatorParameters, null)
															select
															 new HbmParam { name = pname, Text = new[] { ReferenceEquals(pvalue, null) ? "null" : pvalue.ToString() } }).
				ToArray();
			}
			else
			{
				hbmGenerator.param = null;
			}
			hbmId.generator = hbmGenerator;
		}

		public void Generator(IGeneratorDef generator, Action<IGeneratorMapper> generatorMapping)
		{
			ApplyGenerator(generator);
			generatorMapping(new GeneratorMapper(hbmId.generator));
		}

		public void Access(Accessor accessor)
		{
			accessorMapper.Access(accessor);
		}

		public void Access(Type accessorType)
		{
			accessorMapper.Access(accessorType);
		}

		#endregion
	}
}