using System.Reflection;

namespace ConfOrm.Patterns
{
	public class HighLowPoidPattern : PoidIntPattern, IPatternValueGetter<MemberInfo, IPersistentIdStrategy>
	{
		private readonly object parameters;
		public HighLowPoidPattern() {}
		public HighLowPoidPattern(object parameters)
		{
			this.parameters = parameters;
		}

		#region Implementation of IPatternApplier<MemberInfo,IPersistentIdStrategy>

		public IPersistentIdStrategy Get(MemberInfo element)
		{
			return new HighLowIdStrategy {Params = parameters};
		}

		#endregion

		private class HighLowIdStrategy: IPersistentIdStrategy
		{
			#region Implementation of IPersistentIdStrategy

			public PoIdStrategy Strategy
			{
				get { return PoIdStrategy.HighLow; }
			}

			public object Params{get; set;}

			#endregion
		}
	}
}