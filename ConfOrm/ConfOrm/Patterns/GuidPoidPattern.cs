using System.Reflection;

namespace ConfOrm.Patterns
{
	public class GuidPoidPattern: PoidGuidPattern, IPatternApplier<MemberInfo, IPersistentIdStrategy>
	{
		#region Implementation of IPatternApplier<MemberInfo,IPersistentIdStrategy>

		public IPersistentIdStrategy Apply(MemberInfo subject)
		{
			return new GuidIdStrategy();
		}

		#endregion

		private class GuidIdStrategy : IPersistentIdStrategy
		{
			#region Implementation of IPersistentIdStrategy

			public PoIdStrategy Strategy
			{
				get { return PoIdStrategy.Guid; }
			}

			public object Params
			{
				get { return null; }
			}

			#endregion
		}
	}
}