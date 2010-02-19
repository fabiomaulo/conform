using ConfOrm.Mappers;

namespace ConfOrm.NH
{
	public static class Generators
	{
		static Generators()
		{
			Native = new NativeGeneratorDef();
			HighLow = new HighLowGeneratorDef();
			Guid = new GuidGeneratorDef();
			GuidComb = new GuidCombGeneratorDef();
			Sequence = new SequenceGeneratorDef();
			Identity = new IdentityGeneratorDef();
		}

		public static IGeneratorDef Native { get; private set; }
		public static IGeneratorDef HighLow { get; private set; }
		public static IGeneratorDef Guid { get; private set; }
		public static IGeneratorDef GuidComb { get; private set; }
		public static IGeneratorDef Sequence { get; private set; }
		public static IGeneratorDef Identity { get; private set; }
	}

	public class NativeGeneratorDef: IGeneratorDef
	{
		#region Implementation of IGeneratorDef

		public string Class
		{
			get { return "native"; }
		}

		public object Params
		{
			get { return null; }
		}

		#endregion
	}
	public class HighLowGeneratorDef : IGeneratorDef
	{
		#region Implementation of IGeneratorDef

		public string Class
		{
			get { return "hilo"; }
		}

		public object Params
		{
			get { return null; }
		}

		#endregion
	}
	public class GuidGeneratorDef : IGeneratorDef
	{
		#region Implementation of IGeneratorDef

		public string Class
		{
			get { return "guid"; }
		}

		public object Params
		{
			get { return null; }
		}

		#endregion
	}
	public class GuidCombGeneratorDef : IGeneratorDef
	{
		#region Implementation of IGeneratorDef

		public string Class
		{
			get { return "guid.comb"; }
		}

		public object Params
		{
			get { return null; }
		}

		#endregion
	}
	public class SequenceGeneratorDef : IGeneratorDef
	{
		#region Implementation of IGeneratorDef

		public string Class
		{
			get { return "sequence"; }
		}

		public object Params
		{
			get { return null; }
		}

		#endregion
	}
	public class IdentityGeneratorDef : IGeneratorDef
	{
		#region Implementation of IGeneratorDef

		public string Class
		{
			get { return "identity"; }
		}

		public object Params
		{
			get { return null; }
		}

		#endregion
	}

}