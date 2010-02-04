using System.Reflection;

namespace ConfOrm.Patterns
{
	public class NativePoidPattern : PoidIntPattern, IPatternApplier<MemberInfo, IPersistentIdStrategy>
	{
		#region Implementation of IPatternApplier<MemberInfo,IPersistentIdStrategy>

		public IPersistentIdStrategy Apply(MemberInfo subject)
		{
			return new NativeIdStrategy();
		}

		#endregion

		private class NativeIdStrategy : IPersistentIdStrategy
		{
			#region Implementation of IPersistentIdStrategy

			public PoIdStrategy Strategy
			{
				get { return PoIdStrategy.Native; }
			}

			public object Params
			{
				get { return null; }
			}

			#endregion
		}
	}
}