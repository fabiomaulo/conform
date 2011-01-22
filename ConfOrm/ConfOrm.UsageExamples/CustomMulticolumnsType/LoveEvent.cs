using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace ConfOrm.UsageExamples.CustomMulticolumnsType
{
	public class LoveEvent
	{
		public virtual Guid Id { get; set; }
		public virtual bool[] AllowedWeekDays { get; set; }
	}

	public class MulticolumnsBoolArrayType: IUserType, IParameterizedType
	{
		public const int DefaultFixedSize = 2;
		public const string DefaultFixedSizeParameterName = "ArraySize";
		public int fixedSize = DefaultFixedSize;

		public bool Equals(object x, object y)
		{
			bool[] xValue = x as bool[];
			bool[] yValue = y as bool[];
			if (object.Equals(xValue, yValue))
			{
				return true;
			}
			if (ReferenceEquals(null, xValue))
			{
				return false;
			}
			if (ReferenceEquals(null, yValue))
			{
				return false;
			}
			if(xValue.Length != yValue.Length)
			{
				return false;
			}
			return !xValue.Where((b, i) => b != yValue[i]).Any();
		}

		public int GetHashCode(object x)
		{
			bool[] xValue = x as bool[];
			if (xValue == null)
			{
				throw new ArgumentNullException("x", "The value is null or does not is an instance of bool[]");
			}
			int result = 13;
			for (int i = 0; i < xValue.Length; i++)
			{
				result ^= xValue[i].GetHashCode() * 17;
			}
			return result;
		}

		public object NullSafeGet(IDataReader rs, string[] names, object owner)
		{
			bool[] result = new bool[fixedSize];
			for (int i = 0; i < names.Length; i++)
			{
				int ordinal = rs.GetOrdinal(names[i]);
				if(rs.IsDBNull(ordinal))
				{
					result[i] = false;
				}
				else
				{
					result[i] = rs.GetBoolean(ordinal);
				}
			}
			return result;
		}

		public void NullSafeSet(IDbCommand cmd, object value, int index)
		{
			bool[] original = value as bool[];
			if (original == null)
			{
				for (int i = 0; i < fixedSize; i++)
				{
					((IDbDataParameter)cmd.Parameters[index + i]).Value = DBNull.Value;
				}
			}
			else
			{
				for (int i = 0; i < fixedSize; i++)
				{
					((IDbDataParameter)cmd.Parameters[index + i]).Value = original[i];
				}
			}
		}

		public object DeepCopy(object value)
		{
			bool[] original = value as bool[];
			bool[] result = new bool[fixedSize];
			if (original == null)
			{
				return result;
			}
			for (int i = 0; i < original.Length; i++)
			{
				result[i] = original[i];
			}
			return result;
		}

		public object Replace(object original, object target, object owner)
		{
			return DeepCopy(original);
		}

		public object Assemble(object cached, object owner)
		{
			return DeepCopy(cached);
		}

		public object Disassemble(object value)
		{
			return DeepCopy(value);
		}

		public SqlType[] SqlTypes
		{
			get { return Enumerable.Repeat(SqlTypeFactory.Boolean, fixedSize).ToArray(); }
		}

		public Type ReturnedType
		{
			get { return typeof(bool[]); }
		}

		public bool IsMutable
		{
			get { return true; }
		}

		public void SetParameterValues(IDictionary<string, string> parameters)
		{
			if (parameters == null)
			{
				return;
			}

			string mappedFixedSize;
			if (parameters.TryGetValue(DefaultFixedSizeParameterName, out mappedFixedSize))
			{
				if (!string.IsNullOrEmpty(mappedFixedSize))
				{
					fixedSize = int.Parse(mappedFixedSize);
				}
			}
		}
	}
}