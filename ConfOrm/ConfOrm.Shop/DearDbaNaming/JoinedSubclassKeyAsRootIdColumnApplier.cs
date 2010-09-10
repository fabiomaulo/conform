using System;
using System.Linq;
using ConfOrm.Mappers;

namespace ConfOrm.Shop.DearDbaNaming
{
	public class JoinedSubclassKeyAsRootIdColumnApplier: IPatternApplier<Type, IJoinedSubclassAttributesMapper>
	{
		private readonly PoidColumnNameApplier rootEntityPoidApplier;
		private readonly IDomainInspector domainInspector;

		public JoinedSubclassKeyAsRootIdColumnApplier(IDomainInspector domainInspector)
		{
			if (domainInspector == null)
			{
				throw new ArgumentNullException("domainInspector");
			}
			this.domainInspector = domainInspector;
			rootEntityPoidApplier = new PoidColumnNameApplier();
		}


		public virtual bool Match(Type subject)
		{
			// this applier is called only for joined-subclasses
			return subject != null;
		}

		public void Apply(Type subject, IJoinedSubclassAttributesMapper applyTo)
		{
			var rootEntity = subject.GetBaseTypes().Single(t => domainInspector.IsRootEntity(t));
			applyTo.Key(km => km.Column(rootEntityPoidApplier.GetPoidColumnName(rootEntity)));
		}
	}
}