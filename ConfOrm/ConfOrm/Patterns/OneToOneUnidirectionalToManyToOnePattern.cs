using System;
using System.Linq;
using System.Reflection;

namespace ConfOrm.Patterns
{
	public class OneToOneUnidirectionalToManyToOnePattern : IPattern<Relation>
	{
		private const BindingFlags PublicPropertiesOfClassHierarchy =
			BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

		private readonly IExplicitDeclarationsHolder declarationsHolder;

		public OneToOneUnidirectionalToManyToOnePattern(IExplicitDeclarationsHolder declarationsHolder)
		{
			if (declarationsHolder == null)
			{
				throw new ArgumentNullException("declarationsHolder");
			}
			this.declarationsHolder = declarationsHolder;
		}

		#region Implementation of IPattern<Relation>

		public bool Match(Relation subject)
		{
			return declarationsHolder.OneToOneRelations.Contains(subject) && IsUnidirectional(subject);
		}

		#endregion

		private bool IsUnidirectional(Relation relation)
		{
			return !HasPropertyOf(relation.To, relation.From);
		}

		protected bool HasPropertyOf(Type container, Type contained)
		{
			return container.GetProperties(PublicPropertiesOfClassHierarchy).Select(p => p.PropertyType).Any(t => t == contained);
		}
	}
}