using System;

namespace ConfOrm.UsageExamples.Version
{
	public class Entity
	{
		public virtual Guid Id { get; protected set; }
	}

	public class VersionedEntity: Entity
	{
		public virtual int Version { get; protected set; }
	}

	public class CurrencyDefinition: Entity
	{
		public virtual string Name { get; set; }
	}

	public class Company: VersionedEntity
	{
		public virtual string Name { get; set; }
		public Customer Customer { get; set; }
		public Provider Provider { get; set; }
	}

	public class Customer : VersionedEntity
	{
		public Company Company { get; set; }
		public virtual string PriceListCode { get; set; }
	}

	public class Provider : VersionedEntity
	{
		public Company Company { get; set; }
		public virtual string ContactName { get; set; }
	}
}