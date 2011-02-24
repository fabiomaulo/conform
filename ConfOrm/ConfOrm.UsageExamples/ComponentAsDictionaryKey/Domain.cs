using System.Collections.Generic;

namespace ConfOrm.UsageExamples.ComponentAsDictionaryKey
{
	public class Person
	{
		public int Id { get; set; }
		public string Email { get; set; }
		public IDictionary<ToySkill, double> Skills { get; set; }
		public ICollection<ToySkill> DefaultSkills { get; set; }
	}

	public class ToySkill
	{
		public Skill Skill { get; set; }
		public int Level { get; set; }
	}

	public class Skill
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}
}