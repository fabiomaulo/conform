using System;
using System.Collections.Generic;

namespace ConfOrm.UsageExamples.TableAndColumnNaming
{
	public class PorEO
	{
		public virtual Int64 PorId { get; set; }
		public virtual string RegName { get; set; }
		public virtual IList<PorBoxEO> ListPorBox { get; set; }
		public virtual DateTime? TransmitedDt { get; set; }
		public virtual string AccessionNbr { get; set; }
		public virtual string AddUser { get; set; }
		public virtual DateTime? AddDt { get; set; }
		public virtual string LastModUser { get; set; }
		public virtual DateTime? LastModDt { get; set; }
	}
	public class PorBoxEO
	{
		public virtual Int64 PorBoxId { get; set; }
		public virtual Int64 PorId { get; set; }
		public virtual Int32 BoxNbr { get; set; }
		public virtual string AddUser { get; set; }
		public virtual DateTime? AddDt { get; set; }
		public virtual string LastModUser { get; set; }
		public virtual DateTime? LastModDt { get; set; }
	}
}