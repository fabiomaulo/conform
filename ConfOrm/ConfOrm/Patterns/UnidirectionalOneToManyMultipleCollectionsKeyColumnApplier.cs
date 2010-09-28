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
				return HasMultipleCollectionOf(subject.GetContainerEntity(DomainInspector), subject.LocalMember.GetPropertyOrFieldType().DetermineCollectionElementOrDictionaryValueType());
			}
			return false;
		}

		public void Apply(PropertyPath subject, ICollectionPropertiesMapper applyTo)
		{
			applyTo.Key(km=> km.Column(GetColumnName(subject)));
		}

		/// <summary>
		/// Check if the <paramref name="collectionOwner"/> has more than one collection of the same entity-type.
		/// </summary>
		/// <param name="collectionOwner">The class containing the collection.</param>
		/// <param name="elementType">The type of the element of the generic collection (is an entity for sure).</param>
		/// <returns>True when the <paramref name="collectionOwner"/> contains more than one collection of the same <paramref name="elementType"/>.</returns>
		/// <remarks>
		/// Override this method if you want speed-up the pattern avoiding the usage of reflection in certain cases where you know you have a double usage
		/// (for example using a HashSet{Type} where store well known cases)./>
		/// </remarks>
		protected virtual bool HasMultipleCollectionOf(Type collectionOwner, Type elementType)
		{
			int collectionCount = 0;
			return HasMultipleCollectionOf(collectionOwner, elementType, ref collectionCount);
		}

		protected bool HasMultipleCollectionOf(Type collectionOwner, Type elementType, ref int collectionCount)
		{
			foreach (var propertyType in collectionOwner.GetProperties(PublicPropertiesOfClassHierarchy).Select(p => p.PropertyType))
			{
				if (DomainInspector.IsComponent(propertyType))
				{
					if (HasMultipleCollectionOf(propertyType, elementType, ref collectionCount))
					{
						return true;
					}
				}
				else
				{
					var propertyElementType = propertyType.DetermineCollectionElementOrDictionaryValueType();
					if(elementType.Equals(propertyElementType))
					{
						collectionCount++;
					}
				}
				if(collectionCount > 1)
				{
					return true;
				}
			}
			return false;
		}

		protected virtual string GetColumnName(PropertyPath subject)
		{
			return GetBaseColumnName(subject) + "_key";
		}

		protected virtual string GetBaseColumnName(PropertyPath subject)
		{
			return subject.GetContainerEntity(DomainInspector).Name + subject.ToColumnName();
		}
	}
}