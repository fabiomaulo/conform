using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConfOrm.Mappers;
using NHibernate.Properties;

namespace ConfOrm.NH
{
	public class EntityPropertyMapper : IEntityPropertyMapper
	{
		public string PropertyName { get; set; }

		private const BindingFlags FieldBindingFlag =
			BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

		private readonly Dictionary<string, IFieldNamingStrategy> fieldNamningStrategies =
			new Dictionary<string, IFieldNamingStrategy>
				{
					{"camelcase", new CamelCaseStrategy()},
					{"camelcase-underscore", new CamelCaseUnderscoreStrategy()},
					{"lowercase", new LowerCaseStrategy()},
					{"lowercase-underscore", new LowerCaseUnderscoreStrategy()},
					{"pascalcase-underscore", new PascalCaseUnderscoreStrategy()},
					{"pascalcase-m-underscore", new PascalCaseMUnderscoreStrategy()},
				};

		private readonly Type declaringType;
		private readonly string propertyName;
		private readonly Action<string> setAccessor;
		private readonly bool canChangeAccessor = true;

		public EntityPropertyMapper(Type declaringType, string propertyName, Action<string> accesorValueSetter)
		{
			PropertyName = propertyName;
			if (declaringType == null)
			{
				throw new ArgumentNullException("declaringType");
			}
			MemberInfo member= null;
			if (propertyName != null)
			{
				member = declaringType.GetMember(propertyName, FieldBindingFlag).FirstOrDefault();
			}
			if (member == null)
			{
				accesorValueSetter("none");
				canChangeAccessor = false;
			}
			else if ((member as FieldInfo) != null)
			{
				accesorValueSetter("field");
				canChangeAccessor = false;
			}
			this.declaringType = declaringType;
			this.propertyName = propertyName;
			setAccessor = accesorValueSetter;
		}

		#region Implementation of IEntityPropertyMapper

		public void Access(Accessor accessor)
		{
			if(!canChangeAccessor)
			{
				return;
			}
			switch (accessor)
			{
				case Accessor.Property:
					setAccessor("property");
					break;
				case Accessor.Field:
					string patialFieldNamingStrategyName = GetNamingFieldStrategy();
					if (patialFieldNamingStrategyName != null)
					{
						setAccessor("field." + patialFieldNamingStrategyName);
					}
					else
					{
						setAccessor("field");
					}
					break;
				case Accessor.NoSetter:
					string patialNoSetterNamingStrategyName = GetNamingFieldStrategy();
					if (patialNoSetterNamingStrategyName != null)
					{
						setAccessor("nosetter." + patialNoSetterNamingStrategyName);
					}
					else
					{
						throw new ArgumentOutOfRangeException("accessor",
						                                      "The property name " + propertyName
						                                      + " does not match with any supported field naming strategies.");
					}
					break;
				case Accessor.ReadOnly:
					setAccessor("readonly");
					break;
				case Accessor.None:
					setAccessor("none");
					break;
				default:
					throw new ArgumentOutOfRangeException("accessor");
			}
		}

		public void Access(Type accessorType)
		{
			if (!canChangeAccessor)
			{
				return;
			}
			if (accessorType == null)
			{
				throw new ArgumentNullException("accessorType");
			}
			if (typeof (IPropertyAccessor).IsAssignableFrom(accessorType))
			{
				setAccessor(accessorType.AssemblyQualifiedName);
			}
			else
			{
				throw new ArgumentOutOfRangeException("accessorType", "The accessor should implements IPropertyAccessor.");
			}
		}

		#endregion

		private string GetNamingFieldStrategy()
		{
			KeyValuePair<string, IFieldNamingStrategy> pair =
				fieldNamningStrategies.FirstOrDefault(
				                                     	p =>
				                                     	declaringType.GetField(p.Value.GetFieldName(propertyName),
				                                     	                              FieldBindingFlag) != null);
			return pair.Key;
		}
	}
}