using System;
using System.Reflection;

namespace ConfOrm.NH
{
	public class PropertyPath
	{
		private readonly int hashCode;
		private readonly MemberInfo localMember;
		private readonly PropertyPath prveiousPath;

		public PropertyPath(PropertyPath prveiousPath, MemberInfo localMember)
		{
			if (localMember == null)
			{
				throw new ArgumentNullException("localMember");
			}
			this.prveiousPath = prveiousPath;
			this.localMember = localMember;
			unchecked
			{
				hashCode = (localMember.GetHashCode() * 17) ^ (prveiousPath != null ? prveiousPath.GetHashCode() : 41);
			}
		}

		public PropertyPath PrveiousPath
		{
			get { return prveiousPath; }
		}

		public MemberInfo LocalMember
		{
			get { return localMember; }
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			return Equals(obj as PropertyPath);
		}

		public bool Equals(PropertyPath other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return hashCode == other.GetHashCode();
		}

		public override int GetHashCode()
		{
			return hashCode;
		}
	}
}