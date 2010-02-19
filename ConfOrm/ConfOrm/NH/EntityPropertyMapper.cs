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
		private const BindingFlags FiledBindingFlag =
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

		private readonly MemberInfo member;
		private readonly Action<string> setAccessor;

		public EntityPropertyMapper(MemberInfo member, Action<string> accesorValueSetter)
		{
			this.member = member;
			setAccessor = accesorValueSetter;
		}

		#region Implementation of IEntityPropertyMapper

		public void Access(Accessor accessor)
		{
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
						                                      "The property name " + member.Name
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
				                                     	member.DeclaringType.GetField(p.Value.GetFieldName(member.Name),
				                                     	                              FiledBindingFlag) != null);
			return pair.Key;
		}
	}
}