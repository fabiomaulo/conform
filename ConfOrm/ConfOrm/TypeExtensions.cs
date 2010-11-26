using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ConfOrm
{
	public static class TypeExtensions
	{
		private const BindingFlags PublicPropertiesOfClassHierarchy =
			BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

		public static IEnumerable<Type> GetBaseTypes(this Type type)
		{
			foreach (var @interface in type.GetInterfaces())
			{
				yield return @interface;
			}
			var analizing = type;
			while (analizing != null && analizing != typeof(object))
			{
				analizing = analizing.BaseType;
				yield return analizing;
			}
		}

		public static IEnumerable<Type> GetHierarchyFromBase(this Type type)
		{
			var typeHierarchy = new List<Type>();
			var analizingType = type;
			while (analizingType != null && analizingType != typeof(object))
			{
				typeHierarchy.Add(analizingType);
				analizingType = analizingType.BaseType;
			}
			return typeHierarchy.AsEnumerable().Reverse();
		}

		public static Type GetPropertyOrFieldType(this MemberInfo propertyOrField)
		{
			if (propertyOrField.MemberType == MemberTypes.Property)
			{
				return ((PropertyInfo)propertyOrField).PropertyType;
			}

			if (propertyOrField.MemberType == MemberTypes.Field)
			{
				return ((FieldInfo)propertyOrField).FieldType;
			}
			throw new ArgumentOutOfRangeException("propertyOrField",
																						"Expected PropertyInfo or FieldInfo; found :" + propertyOrField.MemberType);
		}

		public static MemberInfo DecodeMemberAccessExpression<TEntity>(Expression<Func<TEntity, object>> expression)
		{
			if (expression.Body.NodeType != ExpressionType.MemberAccess)
			{
				if ((expression.Body.NodeType == ExpressionType.Convert) && (expression.Body.Type == typeof(object)))
				{
					return ((MemberExpression)((UnaryExpression)expression.Body).Operand).Member;
				}
				throw new Exception(string.Format("Invalid expression type: Expected ExpressionType.MemberAccess, Found {0}",
																					expression.Body.NodeType));
			}
			return ((MemberExpression)expression.Body).Member;
		}

		/// <summary>
		/// Decode a member access expression of a specific ReflectedType
		/// </summary>
		/// <typeparam name="TEntity">Type to reflect</typeparam>
		/// <param name="expression">The expression of the property getter</param>
		/// <returns>The <see cref="MemberInfo"/> os the ReflectedType. </returns>
		public static MemberInfo DecodeMemberAccessExpressionOf<TEntity>(Expression<Func<TEntity, object>> expression)
		{
			MemberInfo memberOfDeclaringType;
			if (expression.Body.NodeType != ExpressionType.MemberAccess)
			{
				if ((expression.Body.NodeType == ExpressionType.Convert) && (expression.Body.Type == typeof (object)))
				{
					memberOfDeclaringType = ((MemberExpression) ((UnaryExpression) expression.Body).Operand).Member;
				}
				else
				{
					throw new Exception(string.Format("Invalid expression type: Expected ExpressionType.MemberAccess, Found {0}",
					                                  expression.Body.NodeType));
				}
			}
			else
			{
				memberOfDeclaringType = ((MemberExpression) expression.Body).Member;
			}
			PropertyInfo memberOfReflectType;
			if (typeof(TEntity).IsInterface)
			{
				// Type.GetProperty(string name,Type returnType) does not work properly with interfaces
				return memberOfDeclaringType;
			}
			else
			{
				memberOfReflectType = typeof(TEntity).GetProperty(memberOfDeclaringType.Name, memberOfDeclaringType.GetPropertyOrFieldType());
			}
			return memberOfReflectType;
		}

		public static MemberInfo DecodeMemberAccessExpression<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> expression)
		{
			if (expression.Body.NodeType != ExpressionType.MemberAccess)
			{
				if ((expression.Body.NodeType == ExpressionType.Convert) && (expression.Body.Type == typeof(object)))
				{
					return ((MemberExpression)((UnaryExpression)expression.Body).Operand).Member;
				}
				throw new Exception(string.Format("Invalid expression type: Expected ExpressionType.MemberAccess, Found {0}",
																					expression.Body.NodeType));
			}
			return ((MemberExpression)expression.Body).Member;
		}

		/// <summary>
		/// Decode a member access expression of a specific ReflectedType
		/// </summary>
		/// <typeparam name="TEntity">Type to reflect</typeparam>
		/// <typeparam name="TProperty">Type of property</typeparam>
		/// <param name="expression">The expression of the property getter</param>
		/// <returns>The <see cref="MemberInfo"/> os the ReflectedType. </returns>
		public static MemberInfo DecodeMemberAccessExpressionOf<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> expression)
		{
			MemberInfo memberOfDeclaringType;
			if (expression.Body.NodeType != ExpressionType.MemberAccess)
			{
				if ((expression.Body.NodeType == ExpressionType.Convert) && (expression.Body.Type == typeof(object)))
				{
					memberOfDeclaringType = ((MemberExpression)((UnaryExpression)expression.Body).Operand).Member;
				}
				else
				{
					throw new Exception(string.Format("Invalid expression type: Expected ExpressionType.MemberAccess, Found {0}",
																						expression.Body.NodeType));
				}
			}
			else
			{
				memberOfDeclaringType = ((MemberExpression)expression.Body).Member;
			}
			PropertyInfo memberOfReflectType;
			if (typeof(TEntity).IsInterface)
			{
				// Type.GetProperty(string name,Type returnType) does not work properly with interfaces
				return memberOfDeclaringType;
			}
			else
			{
				memberOfReflectType = typeof(TEntity).GetProperty(memberOfDeclaringType.Name, memberOfDeclaringType.GetPropertyOrFieldType());				
			}
			return memberOfReflectType;
		}

		public static MemberInfo GetMemberFromDeclaringType(this MemberInfo source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			if (source.DeclaringType.Equals(source.ReflectedType))
			{
				return source;
			}
			return source.DeclaringType.GetProperty(source.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
		}

		public static IEnumerable<MemberInfo> GetPropertyFromInterfaces(this MemberInfo source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			var propertyInfo = source as PropertyInfo;
			if(propertyInfo == null)
			{
				yield break;
			}
			if (source.ReflectedType.IsInterface)
			{
				yield break;
			}
			var interfaces = source.ReflectedType.GetInterfaces();
			if(interfaces.Length == 0)
			{
				yield break;
			}
			var propertyGetter = propertyInfo.GetGetMethod();
			foreach (var @interface in interfaces)
			{
				var memberMap = source.ReflectedType.GetInterfaceMap(@interface);
				var interfaceProperties = @interface.GetProperties();
				for (int i = 0; i < memberMap.TargetMethods.Length; i++)
				{
					if (memberMap.TargetMethods[i] == propertyGetter)
					{
						yield return interfaceProperties.Single(pi => pi.GetGetMethod() == memberMap.InterfaceMethods[i]);
					}
				}
			}
		}

		public static Type DetermineCollectionElementType(this Type genericCollection)
		{
			if (genericCollection.IsGenericType)
			{
				List<Type> interfaces = genericCollection.GetInterfaces().Where(t => t.IsGenericType).ToList();
				if (genericCollection.IsInterface)
				{
					interfaces.Add(genericCollection);
				}
				var enumerableInterface = interfaces.FirstOrDefault(t => t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
				if (enumerableInterface != null)
				{
					return enumerableInterface.GetGenericArguments()[0];
				}
			}
			return null;
		}

		public static Type DetermineCollectionElementOrDictionaryValueType(this Type genericCollection)
		{
			if (genericCollection.IsGenericType)
			{
				List<Type> interfaces = genericCollection.GetInterfaces().Where(t => t.IsGenericType).ToList();
				if (genericCollection.IsInterface)
				{
					interfaces.Add(genericCollection);
				}
				var enumerableInterface = interfaces.FirstOrDefault(t => t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
				if (enumerableInterface != null)
				{
					var dictionaryInterface = interfaces.FirstOrDefault(t => t.GetGenericTypeDefinition() == typeof (IDictionary<,>));
					if (dictionaryInterface == null)
					{
						return enumerableInterface.GetGenericArguments()[0];
					}
					return dictionaryInterface.GetGenericArguments()[1];
				}
			}
			return null;
		}

		public static Type DetermineDictionaryKeyType(this Type genericDictionary)
		{
			if (genericDictionary.IsGenericType)
			{
				Type dictionaryInterface = GetDictionaryInterface(genericDictionary);
				if (dictionaryInterface != null)
				{
					return dictionaryInterface.GetGenericArguments()[0];
				}
			}
			return null;
		}

		private static Type GetDictionaryInterface(Type genericDictionary)
		{
			List<Type> interfaces = genericDictionary.GetInterfaces().Where(t => t.IsGenericType).ToList();
			if (genericDictionary.IsInterface)
			{
				interfaces.Add(genericDictionary);
			}
			return interfaces.FirstOrDefault(t => t.GetGenericTypeDefinition() == typeof (IDictionary<,>));
		}

		public static Type DetermineDictionaryValueType(this Type genericDictionary)
		{
			if (genericDictionary.IsGenericType)
			{
				Type dictionaryInterface = GetDictionaryInterface(genericDictionary);
				if (dictionaryInterface != null)
				{
					return dictionaryInterface.GetGenericArguments()[1];
				}
			}
			return null;
		}

		public static bool IsGenericCollection(this Type source)
		{
			return source.IsGenericType && typeof(IEnumerable).IsAssignableFrom(source);
		}

		public static MemberInfo GetFirstPropertyOfType(this Type propertyContainerType, Type propertyType)
		{
			return GetFirstPropertyOfType(propertyContainerType, propertyType, BindingFlags.Public | BindingFlags.Instance);
		}

		public static MemberInfo GetFirstPropertyOfType(this Type propertyContainerType, Type propertyType, BindingFlags bindingFlags)
		{
			if (propertyContainerType == null || propertyType == null)
			{
				return null;
			}
			var propertyInfos = propertyContainerType.GetProperties(bindingFlags);
			if(propertyInfos == null)
			{
				return null;
			}
			return propertyInfos.FirstOrDefault(p => p.PropertyType == propertyType);
		}

		public static IEnumerable<MemberInfo> GetInterfaceProperties(this Type type)
		{
			if (!type.IsInterface)
			{
				yield break;
			}

			var analyzedInterface = new List<Type>();
			var interfacesQueue = new Queue<Type>();
			analyzedInterface.Add(type);
			interfacesQueue.Enqueue(type);
			while (interfacesQueue.Count > 0)
			{
				var subType = interfacesQueue.Dequeue();
				foreach (var subInterface in
					subType.GetInterfaces().Where(subInterface => !analyzedInterface.Contains(subInterface)))
				{
					analyzedInterface.Add(subInterface);
					interfacesQueue.Enqueue(subInterface);
				}

				foreach (var propertyInfo in subType.GetProperties())
				{
					yield return propertyInfo;
				}
			}
		}

		public static bool IsEnumOrNullableEnum(this Type type)
		{
			if (type == null)
			{
				return false;
			}
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				var typeOfNullable = type.GetGenericArguments()[0];
				return typeOfNullable.IsEnum;
			}
			return type.IsEnum;
		}

		public static bool IsFlagEnumOrNullableFlagEnum(this Type type)
		{
			if (type == null)
			{
				return false;
			}
			Type typeofEnum = type;
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				typeofEnum = type.GetGenericArguments()[0];
			}
			return typeofEnum.IsEnum && typeofEnum.GetCustomAttributes(typeof(FlagsAttribute),false).Length > 0;
		}

		public static IEnumerable<Type> GetGenericIntercafesTypeDefinitions(this Type type)
		{
			if(type.IsGenericType && type.IsInterface)
			{
				yield return type.GetGenericTypeDefinition();
			}
			foreach (var t in type.GetInterfaces().Where(t => t.IsGenericType))
			{
				yield return t.GetGenericTypeDefinition();
			}
		}

		public static Type GetFirstImplementorOf(this Type source, Type abstractType)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (abstractType == null)
			{
				throw new ArgumentNullException("abstractType");
			}
			if(source.IsInterface)
			{
				return null;
			}
			if (source.Equals(abstractType))
			{
				return source;
			}
			return source.GetHierarchyFromBase().FirstOrDefault(t => !t.Equals(abstractType) && abstractType.IsAssignableFrom(t));
		}

		public static bool HasPublicPropertyOf(this Type source, Type typeOfProperty)
		{
			return GetFirstPropertyOfType(source, typeOfProperty, PublicPropertiesOfClassHierarchy) != null;
		}
	}
}