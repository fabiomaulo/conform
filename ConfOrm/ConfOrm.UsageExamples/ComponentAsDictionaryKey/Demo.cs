using System;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Shop.CoolNaming;
using NUnit.Framework;

namespace ConfOrm.UsageExamples.ComponentAsDictionaryKey
{
	public class Demo
	{
		[Test, Explicit]
		public void GeneralCustomizationOfComponent()
		{
			// In this example you can see how configure the HighLowPoidPattern with a per hierarchy 'where' clause
			var entities = new[] { typeof(Person), typeof(Skill) };
			var orm = new ObjectRelationalMapper();

			// Instancing the Mapper using the result of Merge
			var mapper = new Mapper(orm, new CoolPatternsAppliersHolder(orm));
			mapper.Component<ToySkill>(x => x.ManyToOne(toySkill => toySkill.Skill, map => { map.Column("SkillId"); map.Fetch(FetchMode.Join); }));
			mapper.Class<Person>(cm => cm.Map(person => person.Skills, mapm => mapm.Table("PersonSkill"), mapk => { }, cer => cer.Element(em => em.Column("Lel"))));

			orm.TablePerClass(entities);

			// In the mapping you can see that the FetchMode.Join is used only where NHibernate supports it.
			var mapping = mapper.CompileMappingFor(entities);
			Console.Write(mapping.AsString());
		}
	}
}