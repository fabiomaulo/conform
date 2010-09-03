using NUnit.Framework;

namespace ConfOrm.UsageExamples.CreateXmlMappingsInBinFolder
{
	public class DemoOfXmlMappingCreation
	{
		[Test, Explicit("Create Mapping Xml Files")]
		public void CreateXmlMappings()
		{
			// you can use a similar implementation is a console application or
			// you can copy&paste this method in your persistence-test-suite
			var mappings = new ConfOrmInitializer().GetCompiledMappingsPerClass();
			mappings.WriteAllXmlMapping();
		}
	}
}