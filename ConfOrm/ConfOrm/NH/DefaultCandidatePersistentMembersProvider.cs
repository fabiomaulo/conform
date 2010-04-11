using System;
using System.Collections.Generic;
using System.Reflection;

namespace ConfOrm.NH
{
	public class DefaultCandidatePersistentMembersProvider: ICandidatePersistentMembersProvider
	{
		#region Implementation of ICandidatePersistentMembersProvider

		public IEnumerable<MemberInfo> GetRootEntityMembers(Type entityClass)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<MemberInfo> GetSubEntityMembers(Type entityClass)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<MemberInfo> GetComponentMembers(Type componentClass)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}