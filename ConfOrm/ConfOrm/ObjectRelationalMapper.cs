using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ConfOrm
{
	public class ObjectRelationalMapper : IObjectRelationalMapper, IDomainInspector
	{
		private readonly IExplicitDeclarationsHolder explicitDeclarations;

		public ObjectRelationalMapper()
		{
			explicitDeclarations = new ExplicitDeclarationsHolder();
			Patterns = new DefaultNHibernatePatternsHolder(this, explicitDeclarations);
		}

		public ObjectRelationalMapper(IPatternsHolder patterns): this(patterns, new ExplicitDeclarationsHolder())
		{
		}

		public ObjectRelationalMapper(IPatternsHolder patterns, IExplicitDeclarationsHolder explicitDeclarations)
		{
			if (patterns == null)
			{
				throw new ArgumentNullException("patterns");
			}
			if (explicitDeclarations == null)
			{
				throw new ArgumentNullException("explicitDeclarations");
			}
			Patterns = patterns;
			this.explicitDeclarations = explicitDeclarations;
		}

		public IPatternsHolder Patterns { get; protected set; }

		#region Implementation of IObjectRelationalMapper

		public virtual void TablePerClass(IEnumerable<Type> baseEntities)
		{
			foreach (var baseEntity in baseEntities)
			{
				RegisterTablePerClass(baseEntity);
			}
		}

		public virtual void TablePerClassHierarchy(IEnumerable<Type> baseEntities)
		{
			foreach (var baseEntity in baseEntities)
			{
				RegisterTablePerClassHierarchy(baseEntity);
			}
		}

		public virtual void TablePerConcreteClass(IEnumerable<Type> baseEntities)
		{
			foreach (var baseEntity in baseEntities)
			{
				RegisterTablePerConcreteClass(baseEntity);
			}
		}

		public virtual void TablePerClassHierarchy<TBaseEntity>() where TBaseEntity : class
		{
			var type = typeof(TBaseEntity);
			RegisterTablePerClassHierarchy(type);
		}

		protected void RegisterTablePerClassHierarchy(Type type)
		{
			PreventComponentAsEntity(type);
			explicitDeclarations.RootEntities.Add(type);
			explicitDeclarations.TablePerClassHierarchyEntities.Add(type);
		}

		private void PreventComponentAsEntity(Type type)
		{
			if (explicitDeclarations.Components.Contains(type))
			{
				throw new MappingException("Ambiguos type registered as component and as entity; type:" + type);
			}
		}

		public virtual void TablePerClass<TBaseEntity>() where TBaseEntity : class
		{
			var type = typeof (TBaseEntity);
			RegisterTablePerClass(type);
		}

		protected void RegisterTablePerClass(Type type)
		{
			PreventComponentAsEntity(type);
			explicitDeclarations.RootEntities.Add(type);
			explicitDeclarations.TablePerClassEntities.Add(type);
		}

		public virtual void TablePerConcreteClass<TBaseEntity>() where TBaseEntity : class
		{
			var type = typeof(TBaseEntity);
			RegisterTablePerConcreteClass(type);
		}

		protected void RegisterTablePerConcreteClass(Type type)
		{
			PreventComponentAsEntity(type);
			explicitDeclarations.RootEntities.Add(type);
			explicitDeclarations.TablePerConcreteClassEntities.Add(type);
		}

		public virtual void Component<TComponent>()
		{
			var type = typeof(TComponent);
			if(IsEntity(type))
			{
				throw new MappingException("Ambiguos type registered as component and as entity; type:" + type);
			}
			explicitDeclarations.Components.Add(type);
		}

		public virtual void Complex<TComplex>()
		{
			var type = typeof(TComplex);
			explicitDeclarations.ComplexTypes.Add(type);
		}

		public void Exclude<TClass>()
		{
			explicitDeclarations.ClassExclusions.Add(typeof (TClass));
		}

		public virtual void Poid<TEntity>(Expression<Func<TEntity, object>> propertyGetter)
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(propertyGetter);
			explicitDeclarations.Poids.Add(member);
		}

		public virtual void NaturalId<TBaseEntity>(params Expression<Func<TBaseEntity, object>>[] propertiesGetters) where TBaseEntity : class
		{
			if (!IsRootEntity(typeof(TBaseEntity)))
			{
				throw new ArgumentOutOfRangeException("TBaseEntity", "The entity class should be a root-entity; register the table-strategy first.");
			}
			foreach (var propertyGetter in propertiesGetters.Where(pg=> pg!=null))
			{
				var member = TypeExtensions.DecodeMemberAccessExpression(propertyGetter);
				explicitDeclarations.NaturalIds.Add(member);
			}
		}

		public virtual void ManyToMany<TLeftEntity, TRigthEntity>()
		{
			explicitDeclarations.ManyToManyRelations.Add(new Relation(typeof(TLeftEntity), typeof(TRigthEntity)));
			explicitDeclarations.ManyToManyRelations.Add(new Relation(typeof(TRigthEntity), typeof(TLeftEntity), Declared.Implicit));
		}

		public virtual void ManyToOne<TLeftEntity, TRigthEntity>()
		{
			explicitDeclarations.ManyToOneRelations.Add(new Relation(typeof(TLeftEntity), typeof(TRigthEntity)));
			explicitDeclarations.OneToManyRelations.Add(new Relation(typeof(TRigthEntity), typeof(TLeftEntity)));
		}

		public virtual void OneToOne<TLeftEntity, TRigthEntity>()
		{
			explicitDeclarations.OneToOneRelations.Add(new Relation(typeof(TLeftEntity), typeof(TRigthEntity)));
			explicitDeclarations.OneToOneRelations.Add(new Relation(typeof(TRigthEntity), typeof(TLeftEntity), Declared.Implicit));
		}

		public virtual void Set<TEntity>(Expression<Func<TEntity, object>> propertyGetter)
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(propertyGetter);
			explicitDeclarations.Sets.Add(member);
			var memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(propertyGetter);
			explicitDeclarations.Sets.Add(memberOf);
		}

		public virtual void Bag<TEntity>(Expression<Func<TEntity, object>> propertyGetter)
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(propertyGetter);
			explicitDeclarations.Bags.Add(member);
			var memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(propertyGetter);
			explicitDeclarations.Bags.Add(memberOf);
		}

		public virtual void List<TEntity>(Expression<Func<TEntity, object>> propertyGetter)
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(propertyGetter);
			explicitDeclarations.Lists.Add(member);
			var memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(propertyGetter);
			explicitDeclarations.Lists.Add(memberOf);
		}

		public virtual void Array<TEntity>(Expression<Func<TEntity, object>> propertyGetter)
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(propertyGetter);
			explicitDeclarations.Arrays.Add(member);
			var memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(propertyGetter);
			explicitDeclarations.Arrays.Add(memberOf);
		}

		public virtual void Dictionary<TEntity>(Expression<Func<TEntity, object>> propertyGetter)
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(propertyGetter);
			explicitDeclarations.Dictionaries.Add(member);
			var memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(propertyGetter);
			explicitDeclarations.Dictionaries.Add(memberOf);
		}

		public virtual void Complex<TEntity>(Expression<Func<TEntity, object>> propertyGetter)
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(propertyGetter);
			explicitDeclarations.ComplexTypeMembers.Add(member);
			var memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(propertyGetter);
			explicitDeclarations.ComplexTypeMembers.Add(memberOf);
		}

		public virtual void HeterogeneousAssociation<TEntity>(Expression<Func<TEntity, object>> propertyGetter)
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(propertyGetter);
			explicitDeclarations.HeterogeneousAssociations.Add(member);
			var memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(propertyGetter);
			explicitDeclarations.HeterogeneousAssociations.Add(memberOf);
		}

		public virtual void Cascade<TFromEntity, TToEntity>(Cascade cascadeOptions)
		{
			explicitDeclarations.Cascades.Add(new Relation(typeof(TFromEntity), typeof(TToEntity)), cascadeOptions);
		}

		public virtual void PersistentProperty<TEntity>(Expression<Func<TEntity, object>> propertyGetter)
		{
			var member = TypeExtensions.DecodeMemberAccessExpressionOf(propertyGetter);
			var memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(propertyGetter);
			explicitDeclarations.PersistentProperties.Add(member);
			explicitDeclarations.PersistentProperties.Add(memberOf);
		}

		public void ExcludeProperty<TEntity>(Expression<Func<TEntity, object>> propertyGetter)
		{
			var member = TypeExtensions.DecodeMemberAccessExpressionOf(propertyGetter);
			var memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(propertyGetter);
			explicitDeclarations.ExclusionProperties.Add(member);
			explicitDeclarations.ExclusionProperties.Add(memberOf);
		}

		public virtual void VersionProperty<TEntity>(Expression<Func<TEntity, object>> propertyGetter)
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(propertyGetter);
			explicitDeclarations.VersionProperties.Add(member);
			var memberOf = TypeExtensions.DecodeMemberAccessExpression(propertyGetter);
			explicitDeclarations.VersionProperties.Add(memberOf);
		}

		#endregion

		#region Implementation of IDomainInspector

		public virtual bool IsRootEntity(Type type)
		{
			return explicitDeclarations.RootEntities.Contains(type);
		}

		public virtual bool IsComponent(Type type)
		{
			return (explicitDeclarations.Components.Contains(type) || (Patterns.Componets.Match(type) && !IsEntity(type)))
			       && !explicitDeclarations.ComplexTypes.Contains(type);
		}

		public virtual bool IsComplex(MemberInfo member)
		{
			return explicitDeclarations.ComplexTypes.Contains(member.GetPropertyOrFieldType()) || explicitDeclarations.ComplexTypeMembers.ContainsMember(member);
		}

		public bool IsVersion(MemberInfo member)
		{
			return explicitDeclarations.VersionProperties.ContainsMember(member) || Patterns.Versions.Match(member);
		}

		public virtual bool IsEntity(Type type)
		{
			return !explicitDeclarations.ClassExclusions.Contains(type) && (explicitDeclarations.RootEntities.Contains(type)
			       || type.GetBaseTypes().Any(t => explicitDeclarations.RootEntities.Contains(t)));
		}

		public virtual bool IsTablePerClass(Type type)
		{
			return IsMappedFor(explicitDeclarations.TablePerClassEntities, type);
		}

		public virtual bool IsTablePerClassHierarchy(Type type)
		{
			return IsMappedFor(explicitDeclarations.TablePerClassHierarchyEntities, type);
		}

		protected bool IsMappedFor(ICollection<Type> explicitMappedEntities, Type type)
		{
			bool isExplicitMapped = explicitMappedEntities.Contains(type);
			bool isDerived = false;

			if (!isExplicitMapped)
			{
				isDerived = type.GetBaseTypes().Any(explicitMappedEntities.Contains);
				if (isDerived)
				{
					explicitMappedEntities.Add(type);
				}
			}
			return isExplicitMapped || isDerived;
		}

		public virtual bool IsTablePerConcreteClass(Type type)
		{
			return IsMappedFor(explicitDeclarations.TablePerConcreteClassEntities, type);
		}

		public virtual bool IsOneToOne(Type from, Type to)
		{
			var relation = new Relation(from, to);
			return IsEntity(from) && IsEntity(to) && explicitDeclarations.OneToOneRelations.Contains(relation) && !Patterns.ManyToOneRelations.Match(relation);
		}

		public bool IsMasterOneToOne(Type from, Type to)
		{
			Relation relation = explicitDeclarations.OneToOneRelations.SingleOrDefault(oto => oto.Equals(new Relation(from, to)));
			return relation != null && relation.DeclaredAs == Declared.Explicit;
		}

		public virtual bool IsManyToOne(Type from, Type to)
		{
			bool areEntities = IsEntity(from) && IsEntity(to);
			bool isFromComponentToEntity = IsComponent(from) && IsEntity(to);
			var relation = new Relation(from, to);
			return (areEntities && explicitDeclarations.ManyToOneRelations.Contains(relation))
						 || (areEntities && Patterns.ManyToOneRelations.Match(relation)) ||
			       (areEntities && !IsOneToOne(from, to)
			        && !explicitDeclarations.ManyToManyRelations.Contains(relation)) || isFromComponentToEntity;
		}

		public virtual bool IsManyToMany(Type role1, Type role2)
		{
			return IsEntity(role1) && IsEntity(role2)
			       && explicitDeclarations.ManyToManyRelations.Contains(new Relation(role1, role2));
		}

		public virtual bool IsMasterManyToMany(Type from, Type to)
		{
			return IsManyToMany(from, to)
			       &&
			       explicitDeclarations.ManyToManyRelations.Single(mtm => mtm.Equals(new Relation(from, to))).DeclaredAs
			       == Declared.Explicit;
		}

		public virtual bool IsOneToMany(Type from, Type to)
		{
			if (IsEntity(to) && explicitDeclarations.OneToManyRelations.Contains(new Relation(from, to)))
			{
				return true;
			}
			bool areEntities = IsEntity(from) && IsEntity(to);
			bool isFromComponentToEntity = IsComponent(from) && IsEntity(to);
			return (areEntities || isFromComponentToEntity) && !IsOneToOne(from, to)
			       && !explicitDeclarations.ManyToManyRelations.Contains(new Relation(from, to));
		}

		public virtual bool IsHeterogeneousAssociation(MemberInfo member)
		{
			return explicitDeclarations.HeterogeneousAssociations.ContainsMember(member);
		}

		public virtual Cascade? ApplyCascade(Type from, MemberInfo on, Type to)
		{
			var relationOn = new RelationOn(from, on, to);

			Cascade resultByClasses;
			var relation = new Relation(from, to);
			if (explicitDeclarations.Cascades.TryGetValue(relation, out resultByClasses))
			{
				// Has Explicit Cascade By Classes
				return resultByClasses;
			}

			return Patterns.Cascades.GetValueOfFirstMatch(relationOn);
		}

		public virtual bool IsPersistentId(MemberInfo member)
		{
			return explicitDeclarations.Poids.ContainsMember(member) || Patterns.Poids.Match(member);
		}

		public virtual IPersistentIdStrategy GetPersistentIdStrategy(MemberInfo member)
		{
			return Patterns.PoidStrategies.GetValueOfFirstMatch(member);
		}

		public virtual bool IsMemberOfNaturalId(MemberInfo member)
		{
			return explicitDeclarations.NaturalIds.ContainsMember(member);
		}

		public virtual bool IsPersistentProperty(MemberInfo role)
		{
			return (!explicitDeclarations.ExclusionProperties.Contains(role) && !Patterns.PersistentPropertiesExclusions.Match(role))
						 || explicitDeclarations.PersistentProperties.ContainsMember(role);
		}

		public virtual bool IsSet(MemberInfo role)
		{
			return explicitDeclarations.Sets.ContainsMember(role) || Patterns.Sets.Match(role);
		}

		public virtual bool IsBag(MemberInfo role)
		{
			return explicitDeclarations.Bags.ContainsMember(role)
			       ||
						 (!explicitDeclarations.Sets.ContainsMember(role) && !explicitDeclarations.Lists.ContainsMember(role)
							&& !explicitDeclarations.Arrays.ContainsMember(role) && Patterns.Bags.Match(role));
		}

		public virtual bool IsList(MemberInfo role)
		{
			return explicitDeclarations.Lists.ContainsMember(role)
			       ||
						 (!explicitDeclarations.Arrays.ContainsMember(role) && !explicitDeclarations.Bags.ContainsMember(role)
			        && Patterns.Lists.Match(role));
		}

		public virtual bool IsArray(MemberInfo role)
		{
			return explicitDeclarations.Arrays.ContainsMember(role)
			       ||
						 (!explicitDeclarations.Lists.ContainsMember(role) && !explicitDeclarations.Bags.ContainsMember(role)
			        && Patterns.Arrays.Match(role));
		}

		public virtual bool IsDictionary(MemberInfo role)
		{
			return explicitDeclarations.Dictionaries.ContainsMember(role) || Patterns.Dictionaries.Match(role);
		}

		#endregion
	}
}