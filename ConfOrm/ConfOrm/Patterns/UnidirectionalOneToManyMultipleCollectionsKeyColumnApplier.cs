using System;
using System.Linq;
using System.Reflection;
using ConfOrm.Mappers;
using ConfOrm.NH;

namespace ConfOrm.Patterns
{
	/// <summary>
	/// Match with multiple collection of the same type and uses the property name to disambiguates the collection key-column.
	/// </summary>
	/// <remarks>
	/// This applier is reflection-expensive. Perhaps would be better the usage of the property-path always.
	/// Because reflection-expensive, and because you can use a specific where clause to disambiguates collection elements, 
	/// this applier is not part of <see cref="DefaultPatternsAppliersHolder"/> (you have to use it explicitly).
	/// </remarks>
	public class UnidirectionalOneToManyMultipleCollectionsKeyColumnApplier : UnidirectionalOneToManyMemberPattern, IPatternApplier<PropertyPath, ICollectionPropertiesMapper>
	{
		private const BindingFlags PublicPropertiesOfClassHierarchy = BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

		public UnidirectionalOneToManyMultipleCollectionsKeyColumnApplier(IDomainInspector domainInspector) : base(domainInspector)
		{
		}

		public bool Match(PropertyPath subject)
		{
			if (subject == null)
			{
				return false;
			}
			if (base.Match(subject.LocalMember))
			{
				return HasMultipleCollectionOf(subject.LocalMember.DeclaringType, subject.LocalMember.GetPropertyOrFieldType().DetermineCollectionElementOrDictionaryValueType());
			}
			return false;
		}

		public void Apply(PropertyPath subject, ICollectionPropertiesMapper applyTo)
		{
			applyTo.Key(km=> km.Column(GetColumnName(subject)));
		}

		protected bool HasMultipleCollectionOf(Type collectionOwner, Type elementType)
		{
			var collectionCount = 0;
			return collectionOwner.GetProperties(PublicPropertiesOfClassHierarchy)
				.Select(p => p.PropertyType)
				.Select(propertyType => propertyType.DetermineCollectionElementOrDictionaryValueType())
				.Where(propertyElementType => elementType.Equals(propertyElementType))
				.Any(x => ++collectionCount > 1);
		}

		protected virtual string GetColumnName(PropertyPath subject)
		{
			return subject.GetContainerEntity(DomainInspector).Name + subject.ToColumnName() + "_key";
		}
	}
}