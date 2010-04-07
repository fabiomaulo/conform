using System;
using System.Collections.Generic;
using System.Linq;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Type;
using NHibernate.UserTypes;

namespace ConfOrm.NH
{
	public class ElementMapper: IElementMapper
	{
		private const string DefaultColumnName = "element";
		private readonly Type elementType;
		private readonly HbmElement elementMapping;

		public ElementMapper(Type elementType, HbmElement elementMapping)
		{
			if (elementType == null)
			{
				throw new ArgumentNullException("elementType");
			}
			if (elementMapping == null)
			{
				throw new ArgumentNullException("elementMapping");
			}
			this.elementType = elementType;
			this.elementMapping = elementMapping;
			this.elementMapping.type1 = elementType.GetNhTypeName();
		}

		#region Implementation of IColumnsMapper

		public void Column(Action<IColumnMapper> columnMapper)
		{
			if (elementMapping.Columns.Count() > 1)
			{
				throw new MappingException("Multi-columns property can't be mapped through singlr-column API.");
			}
			HbmColumn hbm = elementMapping.Columns.SingleOrDefault();
			hbm = hbm
						??
						new HbmColumn
						{
							name = elementMapping.column,
							length = elementMapping.length,
							scale = elementMapping.scale,
							precision = elementMapping.precision,
							notnull = elementMapping.notnull,
							unique = elementMapping.unique,
							uniqueSpecified = elementMapping.unique,
						};
			columnMapper(new ColumnMapper(hbm, DefaultColumnName));
			if (ColumnTagIsRequired(hbm))
			{
				elementMapping.Items = new[] { hbm };
				ResetColumnPlainValues();
			}
			else
			{
				elementMapping.column = !DefaultColumnName.Equals(hbm.name) ? hbm.name : null;
				elementMapping.length = hbm.length;
				elementMapping.precision = hbm.precision;
				elementMapping.scale = hbm.scale;
				elementMapping.notnull = hbm.notnull;
				elementMapping.unique = hbm.unique;
			}
		}

		private bool ColumnTagIsRequired(HbmColumn hbm)
		{
			return hbm.uniquekey != null || hbm.sqltype != null || hbm.index != null || hbm.@default != null || hbm.check != null;
		}

		private void ResetColumnPlainValues()
		{
			elementMapping.column = null;
			elementMapping.length = null;
			elementMapping.precision = null;
			elementMapping.scale = null;
			elementMapping.notnull = false;
			elementMapping.unique = false;
		}

		public void Columns(params Action<IColumnMapper>[] columnMapper)
		{
			ResetColumnPlainValues();
			int i = 1;
			var columns = new List<HbmColumn>(columnMapper.Length);
			foreach (var action in columnMapper)
			{
				var hbm = new HbmColumn();
				var defaultColumnName = DefaultColumnName + i++;
				action(new ColumnMapper(hbm, defaultColumnName));
				columns.Add(hbm);
			}
			elementMapping.Items = columns.ToArray();
		}

		public void Column(string name)
		{
			Column(x => x.Name(name));
		}

		#endregion

		#region Implementation of IElementMapper

		public void Type(IType persistentType)
		{
			if (persistentType != null)
			{
				elementMapping.type1 = persistentType.Name;
				elementMapping.type = null;
			}
		}

		public void Type<TPersistentType>() where TPersistentType : IUserType
		{
			Type(typeof(TPersistentType), null);
		}

		public void Type<TPersistentType>(object parameters) where TPersistentType : IUserType
		{
			Type(typeof(TPersistentType), parameters);
		}

		public void Type(Type persistentType, object parameters)
		{
			if (persistentType == null)
			{
				throw new ArgumentNullException("persistentType");
			}
			if (!typeof(IUserType).IsAssignableFrom(persistentType))
			{
				throw new ArgumentOutOfRangeException("persistentType", "Expected type implementing IUserType");
			}
			if (parameters != null)
			{
				elementMapping.type1 = null;
				var hbmType = new HbmType
				{
					name = persistentType.AssemblyQualifiedName,
					param = (from pi in parameters.GetType().GetProperties()
									 let pname = pi.Name
									 let pvalue = pi.GetValue(parameters, null)
									 select
										new HbmParam { name = pname, Text = new[] { ReferenceEquals(pvalue, null) ? "null" : pvalue.ToString() } })
						.ToArray()
				};
				elementMapping.type = hbmType;
			}
			else
			{
				elementMapping.type1 = persistentType.AssemblyQualifiedName;
				elementMapping.type = null;
			}
		}

		public void Length(int length)
		{
			Column(x => x.Length(length));
		}

		public void Precision(short precision)
		{
			Column(x => x.Precision(precision));
		}

		public void Scale(short scale)
		{
			Column(x => x.Scale(scale));
		}

		public void NotNullable(bool notnull)
		{
			Column(x => x.NotNullable(notnull));
		}

		public void Unique(bool unique)
		{
			Column(x => x.Unique(unique));
		}

		#endregion
	}
}