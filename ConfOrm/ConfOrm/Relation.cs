using System;

namespace ConfOrm
{
	public class Relation
	{
		private readonly int hashCode;
		public Relation(Type from, Type to)
		{
			if (from == null)
			{
				throw new ArgumentNullException("from");
			}
			if (to == null)
			{
				throw new ArgumentNullException("to");
			}
			From = from;
			To = to;
			hashCode = (37 * from.GetHashCode()) ^ to.GetHashCode();
		}

		public Type From { get; private set; }
		public Type To { get; private set; }

		public override bool Equals(object obj)
		{
			return Equals(obj as Relation);
		}

		public bool Equals(Relation that)
		{
			if(ReferenceEquals(null, that))
			{
				return false;
			}

			return From.Equals(that.From) && To.Equals(that.To);
		}

		public override int GetHashCode()
		{
			return hashCode;
		}

		public override string ToString()
		{
			return string.Format("From {0} To {1}", From.FullName, To.FullName);
		}
	}
}