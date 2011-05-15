using System.Linq;
using System.Reflection;
using NHibernate.Mapping.ByCode;

namespace ConfOrm.Patterns
{
	public class PolymorphismBidirectionalOneToManyOnDeleteConstraintApplier: PolymorphismBidirectionalOneToManyMemberPattern, IPatternApplier<MemberInfo, ICollectionPropertiesMapper>
	{
		public PolymorphismBidirectionalOneToManyOnDeleteConstraintApplier(IDomainInspector domainInspector) : base(domainInspector) {}

		public override bool Match(MemberInfo subject)
		{
			if(!base.Match(subject))
			{
				return false;
			}

			var concreteRelation = GetConcreteRelation(subject);
			if(concreteRelation == null)
			{
				return false;
			}
			var one = concreteRelation.From;
			var many = concreteRelation.To;
			if (one.Equals(many))
			{
				// Circular references
				return false;
			}

			CascadeOn? applyCascade = GetExplicitPolymorphismCascade(subject);
			if (applyCascade.HasValue && !applyCascade.Value.Has(CascadeOn.DeleteOrphans)
				&& !applyCascade.Value.Has(CascadeOn.Remove) && !applyCascade.Value.Has(CascadeOn.All))
			{
				return false;
			}
			if (!DomainInspector.IsEntity(many))
			{
				return false;
			}
			if (DomainInspector.IsTablePerClass(many) && !DomainInspector.IsRootEntity(many))
			{
				return false;
			}

			return true;
		}

		public void Apply(MemberInfo subject, ICollectionPropertiesMapper applyTo)
		{
			applyTo.Key(km => km.OnDelete(OnDeleteAction.Cascade));
		}

		protected Relation GetConcreteRelation(MemberInfo subject)
		{
			var declaredMany = subject.GetPropertyOrFieldType().DetermineCollectionElementType();
			if (declaredMany == null)
			{
				return null;
			}

			var implementorsOfMany = DomainInspector.GetBaseImplementors(declaredMany).ToArray();
			if (implementorsOfMany.Length != 1)
			{
				// short
				return null;
			}

			var declaredOne = subject.ReflectedType;
			var implementorsOfOne = DomainInspector.GetBaseImplementors(declaredOne).ToArray();
			if (implementorsOfOne.Length != 1)
			{
				return null;
			}

			return new Relation(implementorsOfOne[0], implementorsOfMany[0]);
		}
	}
}