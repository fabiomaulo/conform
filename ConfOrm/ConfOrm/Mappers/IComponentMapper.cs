using System.Reflection;

namespace ConfOrm.Mappers
{
	public interface IComponentMapper : IPropertyContainerMapper
	{
		void Parent(MemberInfo parent);
	}
}