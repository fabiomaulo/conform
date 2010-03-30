using System.Reflection;
using ConfOrm.Mappers;
using ConfOrm.NH;

namespace ConfOrm.Patterns
{
	public class BidirectionalManyToManyColumnApplier: BidirectionalManyToManyPattern, IPatternApplier<MemberInfo, IManyToManyMapper>
	{
		#region Implementation of IPatternApplier<MemberInfo,IManyToManyMapper>

		public void Apply(MemberInfo subject, IManyToManyMapper applyTo)
		{
			var relation = GetRelation(subject);
			if (relation.From == relation.To)
			{
				// circular many-to-many
				applyTo.Column(subject.Name);
			}
			else
			{
				applyTo.Column(GetColumnName(relation));
			}
		}

		#endregion

		protected virtual string GetColumnName(Relation relation)
		{
			return KeyMapper.DefaultColumnName(relation.To);
		}
	}
}