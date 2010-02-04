using System.Reflection;

namespace ConfOrm.Patterns
{
	public class SequencePoidPattern: PoidIntPattern, IPatternApplier<MemberInfo, IPersistentIdStrategy>
	{
		private readonly object parameters;
		public SequencePoidPattern() {}
		public SequencePoidPattern(object parameters)
		{
			this.parameters = parameters;
		}

		#region Implementation of IPatternApplier<MemberInfo,IPersistentIdStrategy>

		public IPersistentIdStrategy Apply(MemberInfo subject)
		{
			return new SequenceIdStrategy { Params = parameters };
		}

		#endregion

		private class SequenceIdStrategy : IPersistentIdStrategy
		{
			#region Implementation of IPersistentIdStrategy

			public PoIdStrategy Strategy
			{
				get { return PoIdStrategy.Sequence; }
			}

			public object Params{get; set;}

			#endregion
		}
	}
}