using System.Collections.Generic;
using Iesi.Collections.Generic;

namespace ConfOrm.UsageExamples.UseSetForICollection
{
	public class Contact
	{
		private readonly ICollection<string> aliases;
		private readonly ICollection<string> phones;

		public Contact()
		{
			phones = new HashedSet<string>();
			aliases = new HashedSet<string>();
		}

		public virtual int Id { get; private set; }

		public ICollection<string> Phones
		{
			get { return phones; }
		}

		public ICollection<string> Aliases
		{
			get { return aliases; }
		}
	}

	public class ContactRecommended
	{
		// Note: The public properties does no changes; what is changed is only private fields
		private readonly ICollection<string> aliases;
		private readonly ISet<string> phones;

		public ContactRecommended()
		{
			phones = new HashedSet<string>();
			aliases = new List<string>();
		}

		public virtual int Id { get; private set; }

		public ICollection<string> Phones
		{
			get { return phones; }
		}

		public ICollection<string> Aliases
		{
			get { return aliases; }
		}
	}

}