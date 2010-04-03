using System;
using ConfOrm.Mappers;
using ConfOrm.NH;

namespace ConfOrm.Shop.Appliers
{
	public class ManyToManyColumnApplier : ManyToManyPattern, IPatternApplier<PropertyPath, IManyToManyMapper>
	{
		public ManyToManyColumnApplier(IDomainInspector domainInspector) : base(domainInspector) {}

		#region Implementation of IPattern<PropertyPath>

		public bool Match(PropertyPath subject)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}
			return Match(subject.LocalMember);
		}

		#endregion

		#region Implementation of IPatternApplier<PropertyPath,IManyToManyMapper>

		public void Apply(PropertyPath subject, IManyToManyMapper applyTo)
		{
			applyTo.Column(GetColumnNameForRelation(subject));
		}

		private string GetColumnNameForRelation(PropertyPath subject)
		{
			var relation = GetRelation(subject.LocalMember);
			return relation.To.Name + "Id";
		}

		#endregion
	}
}