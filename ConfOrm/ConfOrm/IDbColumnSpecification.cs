using System.Data;

namespace ConfOrm
{
	public interface IDbColumnSpecification
	{
		string Name { get; }
		int Lenth { get; }
		int Precision { get; }
		int Scale { get; }
		DbType Type { get; }
		bool NotNullable { get; }
	}
}