using System;
using System.Collections.Generic;
using System.Linq;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public class ManyToManyMapper: IManyToManyMapper
	{
		private readonly Type elementType;
		private readonly HbmManyToMany manyToMany;

		public ManyToManyMapper(Type elementType, HbmManyToMany manyToMany)
		{
			if (elementType == null)
			{
				throw new ArgumentNullException("elementType");
			}
			if (manyToMany == null)
			{
				throw new ArgumentNullException("manyToMany");
			}
			this.elementType = elementType;
			this.manyToMany = manyToMany;
		}

		#region Implementation of IColumnsMapper

		public void Column(Action<IColumnMapper> columnMapper)
		{
			if (manyToMany.Columns.Count() > 1)
			{
				throw new MappingException("Multi-columns property can't be mapped through single-column API.");
			}
			HbmColumn hbm = manyToMany.Columns.SingleOrDefault();
			hbm = hbm
						??
						new HbmColumn
						{
							name = manyToMany.column,
							unique = manyToMany.unique,
							uniqueSpecified = manyToMany.unique,
						};
			var defaultColumnName = elementType.Name;
			columnMapper(new ColumnMapper(hbm, defaultColumnName));
			if (ColumnTagIsRequired(hbm))
			{
				manyToMany.Items = new[] { hbm };
				ResetColumnPlainValues();
			}
			else
			{
				manyToMany.column = defaultColumnName == null || !defaultColumnName.Equals(hbm.name) ? hbm.name : null;
				manyToMany.unique = hbm.unique;
			}
		}

		private bool ColumnTagIsRequired(HbmColumn hbm)
		{
			return hbm.length != null || hbm.precision != null || hbm.scale != null || hbm.notnull || hbm.uniquekey != null
			       || hbm.sqltype != null || hbm.index != null || hbm.@default != null || hbm.check != null;
		}

		private void ResetColumnPlainValues()
		{
			manyToMany.column = null;
			manyToMany.unique = false;
		}

		public void Columns(params Action<IColumnMapper>[] columnMapper)
		{
			ResetColumnPlainValues();
			int i = 1;
			var columns = new List<HbmColumn>(columnMapper.Length);
			foreach (var action in columnMapper)
			{
				var hbm = new HbmColumn();
				var defaultColumnName = elementType.Name + i++;
				action(new ColumnMapper(hbm, defaultColumnName));
				columns.Add(hbm);
			}
			manyToMany.Items = columns.ToArray();
		}

		public void Column(string name)
		{
			Column(x => x.Name(name));
		}

		#endregion
	}
}