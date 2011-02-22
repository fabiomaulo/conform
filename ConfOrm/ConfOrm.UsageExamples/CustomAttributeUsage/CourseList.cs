using System;

namespace ConfOrm.UsageExamples.CustomAttributeUsage
{
	public class CourseList
	{
		[Id]
		public Guid CourseId { get; set; }
		public string Rubric { get; set; }
		public string Number { get; set; }
		public string Title { get; set; }
	}
}