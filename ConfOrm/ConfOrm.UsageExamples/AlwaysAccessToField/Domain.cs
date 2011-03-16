using System;

namespace ConfOrm.UsageExamples.AlwaysAccessToField
{
	public class Person
	{
		private Guid id;
		public Guid Id
		{
			get { return id; }
		}

		private string firstName;
		public string FirstName
		{
			get { return firstName; }
			set { firstName = value; }
		}

		private string lastName;
		public string LastName
		{
			get { return lastName; }
			set { lastName = value; }
		}

		public Address Address { get; set; }
	}

	public class Address
	{
		public string Street { get; set; }
		public string City { get; set; }
	}
}