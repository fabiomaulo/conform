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
				memberOfDeclaringType = ((MemberExpression) expression.Body).Member;
			}
			var memberOfReflectType = typeof (TEntity).GetProperty(memberOfDeclaringType.Name,
			                                                       BindingFlags.Public | BindingFlags.Instance
			                                                       | BindingFlags.FlattenHierarchy);
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
	}
}