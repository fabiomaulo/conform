using System;
using System.Linq;
using System.Reflection;

namespace ConfOrm.Patterns
{
	public class BidirectionalRelationPattern: IPattern<Relation>
	{
		#region Implementation of IPattern<Relation>

		public bool Match(Relation subject)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}
			var fromHasRelationWithTo = subject.From.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy).Select(p => p.PropertyType).Any(t => t.IsAssignableFrom(subject.To));
			var toHasRelationWithFrom = subject.To.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy).Select(p => p.PropertyType).Any(t => t.IsAssignableFrom(subject.From));
			return fromHasRelationWithTo && toHasRelationWithFrom;
		}

		#endregion
	}
}