using System;

namespace ConfOrm.UsageExamples.CustomAttributeUsage
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field,
			 Inherited = true, AllowMultiple = false)]
	public class IdAttribute : Attribute
	{

	}
}