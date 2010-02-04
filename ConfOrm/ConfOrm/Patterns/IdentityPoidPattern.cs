using System.Reflection;

namespace ConfOrm.Patterns
{
	public class IdentityPoidPattern : PoidIntPattern, IPatternApplier<MemberInfo, IPersistentIdStrategy>
	{
		#region Implementation of IPatternApplier<MemberInfo,IPersistentIdStrategy>

		public IPersistentIdStrategy Apply(MemberInfo subject)
		{
			return new IdentityIdStrategy();
		}

		#endregion

		private class IdentityIdStrategy : IPersistentIdStrategy
		{
			#region Implementation of IPersistentIdStrategy

			public PoIdStrategy Strategy
			{
				get { return PoIdStrategy.Identity; }
			}

			public object Params
			{
				get { return null; }
			}

			#endregion
		}
	}
}