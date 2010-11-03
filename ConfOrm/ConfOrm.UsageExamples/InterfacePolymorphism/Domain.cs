using System;
using System.Collections.Generic;

namespace ConfOrm.UsageExamples.InterfacePolymorphism
{
	public class Entity
	{
		public virtual int Id { get; set; }
	}

	internal interface IHasMessage
	{
		string Message { get; set; }
	}

	public class User : Entity
	{
		public virtual string Name { get; set; }
		public virtual ICollection<UserWelcomeMessage> WelcomeMessage { get; set; }
		public virtual ICollection<Tweet> Tweets { get; set; }
	}

	public class UserWelcomeMessage : IHasMessage
	{
		public virtual DateTime ForDate { get; set; }
		public virtual string Message { get; set; }
	}

	public class Tweet : IHasMessage
	{
		public virtual string Message { get; set; }
	}

	public class Email : Entity, IHasMessage
	{
		public virtual string To { get; set; }
		public virtual string Cc { get; set; }
		public virtual string Message { get; set; }
	}

	public class InstantMessage : Entity, IHasMessage
	{
		public virtual string To { get; set; }
		public virtual string Message { get; set; }
	}
}