using System.Reflection;

namespace ConfOrm.Patterns
{
	public class GuidOptimizedPoidPattern : PoidGuidPattern, IPatternApplier<MemberInfo, IPersistentIdStrategy>
	{
		#region Implementation of IPatternApplier<MemberInfo,IPersistentIdStrategy>

		public IPersistentIdStrategy Apply(MemberInfo subject)
		{
			return new GuidOptimizedIdStrategy();
		}

		#endregion

		private class GuidOptimizedIdStrategy : IPersistentIdStrategy
		{
			#region Implementation of IPersistentIdStrategy

			public PoIdStrategy Strategy
			{
				get { return PoIdStrategy.GuidOptimized; }
			}

			public object Params
			{
				get { return null; }
			}

			#endregion
		}
	}
}