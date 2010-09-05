using System;
using System.Collections.Generic;
using Iesi.Collections.Generic;

namespace ConfOrm.UsageExamples.ClassExclusion
{
	public class BaseEntity
	{
		public virtual int Id { get; protected set; }
	}

	public abstract class Movement : BaseEntity
	{
		public virtual DateTime Date { get; set; }
		public virtual string Observation { get; set; }
	}

	public abstract class Movement<TDetail> : Movement
	{
		private ISet<TDetail> details;

		protected Movement()
		{
			details = new HashedSet<TDetail>();
		}

		public virtual IEnumerable<TDetail> Details
		{
			get { return details; }
		}

		public virtual TDetail AddDetail()
		{
			TDetail result = CreateNewDetail();
			details.Add(result);
			return result;
		}

		protected abstract TDetail CreateNewDetail();

		public virtual void RemoveDetail(TDetail detail)
		{
			details.Remove(detail);
			ResetParentMovement(detail);
		}

		protected abstract void ResetParentMovement(TDetail detail);
	}

}