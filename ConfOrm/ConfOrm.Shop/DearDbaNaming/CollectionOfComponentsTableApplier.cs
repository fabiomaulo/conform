using System;
using System.Linq;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Shop.Appliers;

namespace ConfOrm.Shop.DearDbaNaming
{
	public class CollectionOfComponentsTableApplier : CollectionOfComponentsPattern,
																										IPatternApplier<PropertyPath, ICollectionPropertiesMapper>
	{
		public CollectionOfComponentsTableApplier(IDomainInspector domainInspector)
			: base(domainInspector)
		{
		}

		#region IPatternApplier<PropertyPath,ICollectionPropertiesMapper> Members

		public bool Match(PropertyPath subject)
		{
			return Match(subject.LocalMember);
		}

		public void Apply(PropertyPath subject, ICollectionPropertiesMapper applyTo)
		{
			applyTo.Table(GetTableName(subject));
		}

		#endregion

		protected virtual string GetTableName(PropertyPath subject)
		{
			Type entity = subject.GetContainerEntity(DomainInspector).GetBaseTypes().Single(t => DomainInspector.IsRootEntity(t));
			return string.Format("{0}_{1}", entity.GetPoidColumnName(), subject.ToColumnName().ToUpperInvariant());
		}
	}
}