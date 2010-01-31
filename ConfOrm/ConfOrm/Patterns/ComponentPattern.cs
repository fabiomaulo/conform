using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConfOrm.Patterns
{
	public class ComponentPattern : IPattern<Type>
	{
		private const BindingFlags DefaultBinding =
			BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

		private readonly List<IPattern<MemberInfo>> poidPatterns;

		public ComponentPattern()
		{
			poidPatterns = new List<IPattern<MemberInfo>> {new PoIdPattern()};
		}

		#region Implementation of IPattern<Type>

		public bool Match(Type subject)
		{
			return !subject.Namespace.StartsWith("System") /* hack */ &&
				subject.GetProperties(DefaultBinding).Cast<MemberInfo>().Concat(subject.GetFields(DefaultBinding)).All(
					mi => !poidPatterns.Any(p => p.Match(mi)));
		}

		#endregion
	}
}