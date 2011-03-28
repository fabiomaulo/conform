using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public class KeyMapper: IKeyMapper
	{
		private readonly Type ownerEntityType;
		private readonly HbmKey mapping;

		public KeyMapper(Type ownerEntityType, HbmKey mapping)
		{
			this.ownerEntityType = ownerEntityType;
			this.mapping = mapping;
			this.mapping.column1 = DefaultColumnName(ownerEntityType);
		}

		public static string DefaultColumnName(Type ownerEntityType)
		{
			return ownerEntityType.Name.ToLowerInvariant() + "_key";
		}

		#region Implementation of IKeyMapper

		public void Column(Action<IColumnMapper> columnMapper) {
			if (mapping.Columns.Count() > 1)
			{
				throw new MappingException("Multi-columns property can't be mapped through singlr-column API.");
			}
			HbmColumn hbm = mapping.Columns.SingleOrDefault();
			hbm = hbm
						??
						new HbmColumn
						{
							name = mapping.column1,
							notnull = mapping.notnull,
							unique = mapping.unique,
							uniqueSpecified = mapping.unique,
						};
			columnMapper(new ColumnMapper(hbm, DefaultColumnName(ownerEntityType)));
			if (ColumnTagIsRequired(hbm))
			{
				mapping.column = new[] { hbm };
				ResetColumnPlainValues();
			}
			else
			{
				mapping.column1 = !DefaultColumnName(ownerEntityType).Equals(hbm.name) ? hbm.name : null;
				mapping.notnull = hbm.notnull;
				mapping.unique = hbm.unique;
			}
		}


		public void Columns(params Action<IColumnMapper>[] columnMapper) {
			ResetColumnPlainValues();
			int i = 1;
			var columns = new List<HbmColumn>(columnMapper.Length);
			foreach (var action in columnMapper)
			{
				var hbm = new HbmColumn();
				var defaultColumnName = DefaultColumnName(ownerEntityType) + i++;
				action(new ColumnMapper(hbm, defaultColumnName));
				columns.Add(hbm);
			}
			mapping.column = columns.ToArray();
		}

		public void Column(string name)
		{
			Column(x => x.Name(name));
		}

		public void OnDelete(OnDeleteAction deleteAction)
		{
			switch (deleteAction)
			{
				case OnDeleteAction.NoAction:
					mapping.ondelete = HbmOndelete.Noaction;
					break;
				case OnDeleteAction.Cascade:
					mapping.ondelete = HbmOndelete.Cascade;
					break;
			}
		}

		public void PropertyRef(MemberInfo property)
		{
			if (property == null)
			{
				mapping.propertyref = null;
				return;
			}
			if (!ownerEntityType.Equals(property.DeclaringType) && !ownerEntityType.Equals(property.ReflectedType))
			{
				throw new ArgumentOutOfRangeException("property", "Can't reference a property of another entity.");
			}
			mapping.propertyref = property.Name;
		}

		public void Update(bool consideredInUpdateQuery)
		{
			mapping.update = consideredInUpdateQuery;
			mapping.updateSpecified = true;
		}

		public void ForeignKey(string foreingKeyName)
		{
			if(foreingKeyName == null)
			{
				mapping.foreignkey = null;
				return;
			}
			string nameToAssign = foreingKeyName.Trim();
			if (string.Empty.Equals(nameToAssign))
			{
				mapping.foreignkey = "none";
				return;				
			}
			mapping.foreignkey = nameToAssign;
		}

		private bool ColumnTagIsRequired(HbmColumn hbm)
		{
			return hbm.uniquekey != null || hbm.sqltype != null || hbm.index != null || hbm.@default != null || hbm.check != null || hbm.length != null || hbm.precision != null || hbm.scale != null;
		}

		private void ResetColumnPlainValues()
		{
			mapping.column1 = null;
			mapping.notnull = false;
			mapping.unique = false;
		}

		#endregion
	}
}