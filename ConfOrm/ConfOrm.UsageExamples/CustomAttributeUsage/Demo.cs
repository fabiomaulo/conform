using System;
using ConfOrm.NH;
using NUnit.Framework;

namespace ConfOrm.UsageExamples.CustomAttributeUsage
{
	public class Demo
	{
		[Test, Explicit]
		public void UsageOfAttributeToSpecifyThePoID()
		{
			var orm = new ObjectRelationalMapper();
			orm.Patterns.Poids.Add(new PoidByAttributePattern());
			orm.TablePerClass<CourseList>();

			var mapper = new Mapper(orm);

			var mapping = mapper.CompileMappingFor(new[] { typeof(CourseList) });
			Console.Write(mapping.AsString());
		}
	}
}