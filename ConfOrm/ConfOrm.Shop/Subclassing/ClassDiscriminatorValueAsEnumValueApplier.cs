using System;
using System.Collections.Generic;
using System.Linq;
using ConfOrm.Mappers;

namespace ConfOrm.Shop.Subclassing
{
	public class ClassDiscriminatorValueAsEnumValueApplier<TRootEntity, TEnum> : IPatternApplier<Type, IClassAttributesMapper>
		where TRootEntity: class
		where TEnum: struct
	{
		private readonly IDomainInspector domainInspector;
		private readonly string[] enumsNames;
		private readonly int indexOfUnknow;
		private static Dictionary<Type, Func<object, object>> converters;
		static ClassDiscriminatorValueAsEnumValueApplier()
		{
			converters = new Dictionary<Type, Func<object, object>>();
			converters.Add(typeof(int), x=> Convert.ToInt32(x));
		}
		public ClassDiscriminatorValueAsEnumValueApplier(IDomainInspector domainInspector)
		{
			if (domainInspector == null)
			{
				throw new ArgumentNullException("domainInspector");
			}
			if(!typeof(TEnum).IsEnum)
			{
				throw new NotSupportedException("The TEnum type parameter should be an enum.");
			}
			this.domainInspector = domainInspector;
			enumsNames = Enum.GetNames(typeof(TEnum));
			indexOfUnknow = Array.IndexOf(enumsNames.Select(x => x.ToLowerInvariant()).ToArray(), "unknown");
		}

		public bool Match(Type subject)
		{
			return domainInspector.IsTablePerClassHierarchy(subject) && typeof(TRootEntity).IsAssignableFrom(subject);
		}

		public void Apply(Type subject, IClassAttributesMapper applyTo)
		{
			var className = subject.Name;
			if (Array.IndexOf(enumsNames, className) >= 0)
			{
				applyTo.DiscriminatorValue(GetConvertedValue(className));
			}
			else if (indexOfUnknow >= 0)
			{
				applyTo.DiscriminatorValue(GetConvertedValue(enumsNames[indexOfUnknow]));
			}
			else
			{
				throw new ArgumentException("Canot find the discriminator value for the class " + subject.FullName + " using the enum " + typeof(TEnum).FullName);
			}
		}

		private object GetConvertedValue(string enumName)
		{
			return converters[Enum.GetUnderlyingType(typeof(TEnum))](Enum.Parse(typeof(TEnum), enumName));
		}
	}
}