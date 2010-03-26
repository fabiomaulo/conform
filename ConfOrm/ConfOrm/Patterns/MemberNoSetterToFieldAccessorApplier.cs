using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.Patterns
{
	public class MemberNoSetterToFieldAccessorApplier<TApplyTo> : NoSetterPropertyToFieldPattern,
	                                                              IPatternApplier<MemberInfo, TApplyTo>
		where TApplyTo : IAccessorPropertyMapper
	{
		#region Implementation of IPatternApplier<MemberInfo,TApplyTo>

		public void Apply(MemberInfo subject, TApplyTo applyTo)
		{
			applyTo.Access(Accessor.NoSetter);
		}

		#endregion
	}
}