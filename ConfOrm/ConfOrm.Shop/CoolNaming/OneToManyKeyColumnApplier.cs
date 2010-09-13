using System;
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
			// Note: in a double usage of a collection of children the user can choose to solve it
			// using a different 'key-column' or a specific 'where-clause'.
			// Is better to delegate this responsibility case-by-case to a customizer instead use a general pattern.
			Type propertyType = subject.LocalMember.GetPropertyOrFieldType();
			Type childType = propertyType.DetermineCollectionElementType();
			var entity = subject.GetContainerEntity(DomainInspector);
			var parentPropertyInChild = childType.GetFirstPropertyOfType(entity);
			var baseName = parentPropertyInChild == null ? subject.PreviousPath == null ? entity.Name : entity.Name + subject.PreviousPath : parentPropertyInChild.Name;
			return GetKeyColumnName(baseName);
		}

		protected virtual string GetKeyColumnName(string baseName)
		{
			return string.Format("{0}Id", baseName);
		}
	}
}