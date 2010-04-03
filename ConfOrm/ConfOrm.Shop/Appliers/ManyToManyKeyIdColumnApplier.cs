using System;
using ConfOrm.Mappers;
using ConfOrm.NH;

namespace ConfOrm.Shop.Appliers
{
	public class ManyToManyKeyIdColumnApplier: ManyToManyPattern, IPatternApplier<PropertyPath, ICollectionPropertiesMapper>
	{
		public ManyToManyKeyIdColumnApplier(IDomainInspector domainInspector) : base(domainInspector) {}

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

		#region Implementation of IPatternApplier<PropertyPath,ICollectionPropertiesMapper>

		public void Apply(PropertyPath subject, ICollectionPropertiesMapper applyTo)
		{
			applyTo.Key(km => km.Column(GetColumnNameForCollectionKey(subject)));
		}

		#endregion

		protected virtual string GetColumnNameForCollectionKey(PropertyPath subject)
		{
			var entityType = GetContainerEntity(subject);
			return entityType.Name + "Id";
		}

		protected Type GetContainerEntity(PropertyPath propertyPath)
		{
			var analizing = propertyPath;
			while (analizing.PreviousPath != null && !DomainInspector.IsEntity(analizing.LocalMember.ReflectedType))
			{
				analizing = analizing.PreviousPath;
			}
			return analizing.LocalMember.ReflectedType;
		}
	}
}