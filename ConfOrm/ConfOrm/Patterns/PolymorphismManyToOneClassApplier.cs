using System.Linq;
using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.Patterns
{
	public class PolymorphismManyToOneClassApplier : IPatternApplier<MemberInfo, IManyToOneMapper>
	{
		private readonly IDomainInspector domainInspector;

		public PolymorphismManyToOneClassApplier(IDomainInspector domainInspector)
		{
			this.domainInspector = domainInspector;
		}

		public bool Match(MemberInfo subject)
		{
			// apply only when there is just one solution
			var polymorphismResolver = domainInspector.PolymorphismResolver;
			return polymorphismResolver != null && polymorphismResolver.GetBaseImplementors(subject.GetPropertyOrFieldType()).IsSingle();
		}

		public void Apply(MemberInfo subject, IManyToOneMapper applyTo)
		{
			applyTo.Class(domainInspector.PolymorphismResolver.GetBaseImplementors(subject.GetPropertyOrFieldType()).Single());
		}
	}
}