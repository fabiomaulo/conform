using System;
using System.Collections.Generic;

namespace ConfOrm.UsageExamples.StringProperty
{
	public class Entity
	{
		public virtual Guid Id { get; protected set; }
	}

	public class Customer : Entity
	{
		//this is mapped as NVARCHAR(50)
		public virtual string Name { get; set; }

		//this is mapped as NVARCHAR(MAX)		
		public virtual string Note { get; set; }

		//Collection of NVARCHAR(MAX)		
		public virtual IEnumerable<string> Notes { get; set; }
	}
}