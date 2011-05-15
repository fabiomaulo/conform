using NHibernate.Mapping.ByCode;
using ConfOrm.NH;

namespace ConfOrm.Shop.CoolNaming
{
	public class ManyToOneColumnApplier: IPatternApplier<PropertyPath, IManyToOneMapper>
	{
		#region Implementation of IPattern<PropertyPath>

		public bool Match(PropertyPath subject)
		{
			return true;
		}

		#endregion

		#region Implementation of IPatternApplier<PropertyPath,IManyToOneMapper>

		public void Apply(PropertyPath subject, IManyToOneMapper applyTo)
		{
			applyTo.Column(GetRelationColumnName(subject));
		}

		#endregion

		protected virtual string GetRelationColumnName(PropertyPath subject)
		{
			return subject.ToColumnName() + "Id";
		}
	}
}