using System;
using System.Reflection;
using ConfOrm.Mappers;

namespace ConfOrm.UsageExamples.TableAndColumnNaming
{
	public class PoidPropertyColumnNameApplier : IPatternApplier<MemberInfo, IIdMapper>
	{
		public bool Match(MemberInfo subject)
		{
			return true;
		}

		public void Apply(MemberInfo subject, IIdMapper applyTo)
		{
			applyTo.Column(subject.Name.Underscore());
		}
	}
}