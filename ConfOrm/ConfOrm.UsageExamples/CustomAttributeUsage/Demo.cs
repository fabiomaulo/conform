using System;
using System.Linq;
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
			mapper.PatternsAppliers.Merge(new RequiredPropertyPatternApplier(orm));
			mapper.PatternsAppliers.Merge(new StringLengthApplier());

			var mapping = mapper.CompileMappingFor(new[] { typeof(CourseList), typeof(SpecialCourseList) });
			Console.Write(mapping.AsString());
		}

		[Test, Explicit]
		public void UsageOfAttributeToSpecifyThePoIDUsingDelegates()
		{
			var orm = new ObjectRelationalMapper();
			orm.Patterns.Poids.Add(mi => mi.GetCustomAttributes(typeof(IdAttribute), true).Any());
			orm.TablePerClass<CourseList>();

			var mapper = new Mapper(orm);

			var mapping = mapper.CompileMappingFor(new[] { typeof(CourseList) });
			Console.Write(mapping.AsString());
		}

		[Test, Explicit]
		public void UsageOfRequiredInTablePerClassHierarchy()
		{
			var orm = new ObjectRelationalMapper();
			orm.Patterns.Poids.Add(new PoidByAttributePattern());
			orm.TablePerClassHierarchy<CourseList>();

			var mapper = new Mapper(orm);
			mapper.PatternsAppliers.Merge(new RequiredPropertyPatternApplier(orm));
			mapper.PatternsAppliers.Merge(new StringLengthApplier());

			var mapping = mapper.CompileMappingFor(new[] { typeof(CourseList), typeof(SpecialCourseList) });
			Console.Write(mapping.AsString());
		}
	}
}