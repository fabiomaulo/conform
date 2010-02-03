using System.Reflection;

namespace ConfOrm.Patterns
{
	public class HighLowPoidPattern: IPatternApplier<MemberInfo, IPersistentIdStrategy>
	{
		private readonly object parameters;
		public HighLowPoidPattern() {}
		public HighLowPoidPattern(object parameters)
		{
			this.parameters = parameters;
		}

		#region Implementation of IPattern<MemberInfo>

		public bool Match(MemberInfo subject)
		{
			var propertyOrFieldType = subject.GetPropertyOrFieldType();
			return propertyOrFieldType == typeof (int) || propertyOrFieldType == typeof (long);
		}

		#endregion

		#region Implementation of IPatternApplier<MemberInfo,IPersistentIdStrategy>

		public IPersistentIdStrategy Apply(MemberInfo subject)
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