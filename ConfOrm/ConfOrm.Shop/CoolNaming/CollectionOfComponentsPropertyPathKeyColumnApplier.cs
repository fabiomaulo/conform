using ConfOrm.NH;

namespace ConfOrm.Shop.CoolNaming
{
	public class CollectionOfComponentsPropertyPathKeyColumnApplier : CollectionOfComponentsKeyColumnApplier
	{
		public CollectionOfComponentsPropertyPathKeyColumnApplier(IDomainInspector domainInspector) : base(domainInspector)
		{
		}

		protected override string GetBaseName(PropertyPath subject)
		{
			return subject.GetRootMember().DeclaringType.Name + subject.ToColumnName();
		}
	}
}