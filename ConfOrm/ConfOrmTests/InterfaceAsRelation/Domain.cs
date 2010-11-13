using System;

namespace ConfOrmTests.InterfaceAsRelation
{
	/*
	 * Thoughts
	 * 
	 * Case 1:
	 * An interface is implemented only by a root-entity and thus by the whole hierarchy.
	 * 
	 * Case 2:
	 * An interface is implemented only by an entity (not root-entity) and thus by its own hierarchy-branch.
	 * 
	 * Case 3:
	 * An interface is implemented only by a component and thus by its own hierarchy.
	 * 
	 * Case 4:
	 * An interface is implemented by more than one root-entity or no-root-entity and thus by its own hierarchies
	 * 
	 * Case 5:
	 * An interface is implemented by a root-entity, a no-root-entity, a component and it is used in a relation (close the house and go to the church).
	 * 
	 * Case 6:
	 * An interface is implemented by more than one root-entity or no-root-entity and thus by its own hierarchies and it is used as a bidirection-one-to-many
	 * 
	 * ConfORM should discover all cases:
	 * The ObjectRelationalMapper (IDomainInspector) should provide the correct response of IsEntity, IsRootEntity, IsManyToOne, IsComponent, IsHeterogeneousAssociation.
	 * The Mapper (perhaps through the IDomainInspector) should have an applier to be able to apply the correct concrete-class in each case.
	 * 
	 * For case-1 and case-2 ConfORM may generate even the node <import> to use those interfaces in HQL.
	 * For the case-4 ConfORM should provide a way to interprete the relation as heterogeneous-association by default.
	 * For the case-5 I don't think I can do something.
	 * 
	 * NOTE: each case is already supported but only using explicit declaration/customizers
	 */

	public class MyEntity
	{
		public int Id { get; set; }
		public IRelation Relation { get; set; }
		public IDeepRelation DeepRelation { get; set; }
		public IHasSomething ShouldBeComponent { get; set; }
	}

	public interface IRelation
	{
	}
	public interface IDeepRelation
	{
	}
	public interface IRootRelation
	{
	}

	public interface IHasSomething
	{
		string Something { get; set; }
	}

	public class MyInterfacedComponent : IHasSomething
	{
		public string Something { get; set; }
	}

	public class MyRelation : IRelation, IRootRelation
	{
		public int Id { get; set; }
	}

	public class MyDeepRelation : MyRelation, IDeepRelation
	{
	}

	public class MyRelationLevel1 : MyRelation, IHasMessage
	{
		public string Message { get; set; }
	}

	public interface IHasMessage
	{
		string Message { get; set; }
	}

	public class MyOtherEntity : IHasMessage, IRootRelation
	{
		public int Id { get; set; }
		public string Message { get; set; }
	}

	public class MyComponent : IHasMessage
	{
		public string Message { get; set; }
	}

	public class MyBeast
	{
		public int Id { get; set; }
		public IHasMessage Relation { get; set; }
	}
}