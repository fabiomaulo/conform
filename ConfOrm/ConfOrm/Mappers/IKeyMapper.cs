namespace ConfOrm.Mappers
{
	public enum OnDeleteAction
	{
		NoAction,
		Cascade,
	}

	public interface IKeyMapper
	{
		void Column(string columnName);
		void OnDelete(OnDeleteAction deleteAction);
	}
}