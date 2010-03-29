using System;

namespace ConfOrm.Mappers
{
	public interface IManyToManyMapper
	{
		void Column(Action<IColumnMapper> columnMapper);
		void Column(string name);
	}
}