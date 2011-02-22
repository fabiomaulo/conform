using System;
using System.ComponentModel.DataAnnotations;

namespace ConfOrm.UsageExamples.CustomAttributeUsage
{
	public class CourseList
	{
		[Id]
		public Guid CourseId { get; set; }
		[Required]
		public string Rubric { get; set; }
		public string Number { get; set; }
		[StringLength(150)]
		public string Title { get; set; }
	}

	public class SpecialCourseList: CourseList
	{
		[Required]
		public string LongDescription { get; set; }
	}
}