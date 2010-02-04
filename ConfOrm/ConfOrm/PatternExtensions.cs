using System.Collections.Generic;
using System.Linq;

namespace ConfOrm
{
	public static class PatternExtensions
	{
		public static TResult ApplyFirstMatch<TSubject, TResult>(this IEnumerable<IPatternApplier<TSubject, TResult>> appliers, TSubject subject)
		{
			if (appliers == null)
			{
				return default(TResult);
			}
			var patternApplier = appliers.Reverse().FirstOrDefault(a => a.Match(subject));
			return patternApplier != null ? patternApplier.Apply(subject) : default(TResult);
		}

		public static bool Match<T>(this IEnumerable<IPattern<T>> patterns, T subject)
		{
			if (patterns == null)
			{
				return false;
			}
			return patterns.Any(p => p.Match(subject));
		}
	}
}