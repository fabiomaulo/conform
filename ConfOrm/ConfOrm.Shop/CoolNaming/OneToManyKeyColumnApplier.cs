using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Shop.Appliers;

namespace ConfOrm.Shop.CoolNaming
{
	public class OneToManyKeyColumnApplier: OneToManyPattern, IPatternApplier<PropertyPath, ICollectionPropertiesMapper>
	{
		public OneToManyKeyColumnApplier(IDomainInspector domainInspector) : base(domainInspector) { }

		#region Implementation of IPattern<PropertyPath>

		public bool Match(PropertyPath subject)
		{
			return Match(subject.LocalMember);
		}

		#endregion

		#region Implementation of IPatternApplier<PropertyPath,ICollectionPropertiesMapper>

		public void Apply(PropertyPath subject, ICollectionPropertiesMapper applyTo)
		{
			applyTo.Key(km => km.Column(GetKeyColumnName(subject)));
		}

		#endregion

		protected virtual string GetKeyColumnName(PropertyPath subject)
		{
			var entity = subject.GetContainerEntity(DomainInspector);
			return string.Format("{0}Id", subject.PreviousPath == null ? entity.Name : entity.Name + subject.PreviousPath);
		}
	}
}