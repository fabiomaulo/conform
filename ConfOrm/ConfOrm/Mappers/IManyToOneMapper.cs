namespace ConfOrm.Mappers
{
	public interface IManyToOneMapper : IEntityPropertyMapper, IColumnsMapper
	{
		void Cascade(Cascade cascadeStyle);
		void NotNullable(bool notnull);
		void Unique(bool unique);
		void UniqueKey(string uniquekeyName);
		void Index(string indexName);
	}
}