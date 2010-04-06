using System.Linq;
using ConfOrm;
using ConfOrm.NH;
using ConfOrm.Patterns;

namespace ConfOrmExample
{
	public class DefaultDomainMapper: IDomainMapper
	{
		private readonly ObjectRelationalMapper orm;
		private readonly Mapper mapper;
		public DefaultDomainMapper()
		{
			orm = new ObjectRelationalMapper();

			// Remove one of not required patterns
			orm.Patterns.ManyToOneRelations.Remove(
				orm.Patterns.ManyToOneRelations.Single(p => p.GetType() == typeof (OneToOneUnidirectionalToManyToOnePattern)));

			orm.Patterns.PoidStrategies.Add(new NativePoidPattern());
			mapper = new Mapper(orm);
		}

		#region Implementation of IDomainMapper

		public ObjectRelationalMapper DomainDefinition
		{
			get { return orm; }
		}

		public Mapper Mapper
		{
			get { return mapper; }
		}

		#endregion
	}
}