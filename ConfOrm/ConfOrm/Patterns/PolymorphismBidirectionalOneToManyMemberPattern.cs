using System.Linq;
using System.Reflection;

namespace ConfOrm.Patterns
{
	/// <summary>
	/// Match only where the member is a generic-collection, the element has a property of the type of the collection container 
	/// applying polymorphism based on interface or classes.
	/// </summary>
	public class PolymorphismBidirectionalOneToManyMemberPattern : IPattern<MemberInfo>
	{
		private readonly IDomainInspector domainInspector;

		public PolymorphismBidirectionalOneToManyMemberPattern(IDomainInspector domainInspector)
		{
			this.domainInspector = domainInspector;
		}

		public bool Match(MemberInfo subject)
		{
			if (subject == null)
			{
				return false;
			}
			var declaredMany = subject.GetPropertyOrFieldType().DetermineCollectionElementType();
			if(declaredMany == null)
			{
				return false;
			}
			var implementorsOfMany = domainInspector.GetBaseImplementors(declaredMany).ToArray();
			if(implementorsOfMany.Length > 1 || implementorsOfMany[0] == declaredMany)
			{
				return false;
			}
			var many = implementorsOfMany[0];
			var one = subject.ReflectedType;

			return many.HasPublicPropertyOf(one);
		}
	}
}