using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Shop.Appliers;

namespace ConfOrm.Shop.CoolNaming
{
	public class CollectionOfElementsColumnApplier : CollectionOfElementsOnlyPattern, IPatternApplier<PropertyPath, IElementMapper>
	{
		public CollectionOfElementsColumnApplier(IDomainInspector domainInspector) : base(domainInspector) {}

		#region Implementation of IPattern<PropertyPath>

		public bool Match(PropertyPath subject)
		{
			return base.Match(subject.LocalMember);
		}

		#endregion

		#region IPatternApplier<PropertyPath,IElementMapper> Members

		public void Apply(PropertyPath subject, IElementMapper applyTo)
		{
			applyTo.Column(GetColumnName(subject));
		}

		#endregion

		protected virtual string GetColumnName(PropertyPath subject)
		{
			return subject.ToColumnName() + "Element";
		}
	}
}