using System;
using System.Collections.Generic;

namespace ConfOrm.UsageExamples.UseBagForIList
{
	public class Contact
	{
		public virtual Guid Id { get; private set; }
		public IList<string> Phones { get; set; }
		public IList<string> Aliases { get; set; }
	}
}