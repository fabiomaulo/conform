namespace ConfOrm
{
	public interface IPersistentIdStrategy
	{
		string CodeName { get; }

		/// <summary>
		/// Any object as "property value" container
		/// </summary>
		object Params { get; }
	}
}