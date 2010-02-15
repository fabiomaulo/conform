using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Properties;
using NHibernate.Type;
using NHibernate.UserTypes;

namespace ConfOrm.NH
{
	public class PropertyMapper: IPropertyMapper
	{
		private readonly MemberInfo member;
		private readonly HbmProperty propertyMapping;
		private const BindingFlags FiledBindingFlag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
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

		public PropertyMapper(MemberInfo member, HbmProperty propertyMapping)
		{
			if (propertyMapping == null)
			{
				throw new ArgumentNullException("propertyMapping");
			}
			this.member = member;
			this.propertyMapping = propertyMapping;
			if(member == null)
			{
				this.propertyMapping.access = "none";				
			}
			else if ((member as FieldInfo) != null)
			{
				this.propertyMapping.access = "field";
			}
		}

		#region Implementation of IEntityPropertyMapper

		public void Access(Accessor accessor)
		{
			switch (accessor)
			{
				case Accessor.Property:
					propertyMapping.access = "property";
					break;
				case Accessor.Field:
					string patialFieldNamingStrategyName = GetNamingFieldStrategy();
					if (patialFieldNamingStrategyName != null)
					{
						propertyMapping.access = "field." + patialFieldNamingStrategyName;
					}
					else
					{
						propertyMapping.access = "field";
					}
					break;
				case Accessor.NoSetter:
					string patialNoSetterNamingStrategyName = GetNamingFieldStrategy();
					if (patialNoSetterNamingStrategyName != null)
					{
						propertyMapping.access = "nosetter." + patialNoSetterNamingStrategyName;
					}
					else
					{
						throw new ArgumentOutOfRangeException("accessor","The property name "+ member.Name +" does not match with any supported field naming strategies.");
					}
					break;
				case Accessor.ReadOnly:
					propertyMapping.access = "readonly";
					break;
				case Accessor.None:
					propertyMapping.access = "none";
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
			if(typeof (IPropertyAccessor).IsAssignableFrom(accessorType))
			{
				propertyMapping.access = accessorType.AssemblyQualifiedName;
			}
			else
			{
				throw new ArgumentOutOfRangeException("accessorType", "The accessor should implements IPropertyAccessor.");				
			}
		}

		#endregion

		private string GetNamingFieldStrategy()
		{
			var pair =
				fieldNamningStrategies.FirstOrDefault(
				                                     	p =>
				                                     	member.DeclaringType.GetField(p.Value.GetFieldName(member.Name),
				                                     	                              FiledBindingFlag) != null);
			return pair.Key;
		}

		#region Implementation of IPropertyMapper

		public void Type(IType persistentType)
		{
			if (persistentType != null)
			{
				propertyMapping.type1 = persistentType.Name;
				propertyMapping.type = null;
			}
		}

		public void Type<TPersistentType>() where TPersistentType : IUserType
		{
			propertyMapping.type1 = typeof (TPersistentType).AssemblyQualifiedName;
			propertyMapping.type = null;
		}

		public void Type<TPersistentType>(object parameters) where TPersistentType : IUserType
		{
			if (parameters != null)
			{
				propertyMapping.type1 = null;
				var hbmType = new HbmType
				              	{
				              		name = typeof (TPersistentType).AssemblyQualifiedName,
				              		param = (from pi in parameters.GetType().GetProperties()
				              		         let pname = pi.Name
				              		         let pvalue = pi.GetValue(parameters, null)
				              		         select
				              		         	new HbmParam
				              		         		{name = pname, Text = new[] {ReferenceEquals(pvalue, null) ? "null" : pvalue.ToString()}})
				              			.ToArray()
				              	};
				propertyMapping.type = hbmType;
			}
			else
			{
				Type<TPersistentType>();
			}
		}

		#endregion
	}
}