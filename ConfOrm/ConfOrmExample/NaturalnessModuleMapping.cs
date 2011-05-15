using System;
using System.Collections.Generic;
using System.Linq;
using ConfOrm;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrmExample.Domain;

namespace ConfOrmExample
{
	public class NaturalnessModuleMapping: IModuleMapping
	{
		#region Implementation of IModuleMapping

		public void DomainDefinition(ObjectRelationalMapper orm)
		{
			orm.TablePerClass<Animal>();
			orm.TablePerClass<User>();
			orm.TablePerClass<StateProvince>();
			orm.TablePerClassHierarchy<Zoo>();

			orm.ManyToMany<Human, Human>();
			orm.OneToOne<User, Human>();
		}

		public void RegisterPatterns(Mapper mapper, IDomainInspector domainInspector)
		{
		}

		public void Customize(Mapper mapper)
		{
			CustomizeRelations(mapper);
			CustomizeTables(mapper);
			CustomizeColumns(mapper);
		}

		public IEnumerable<Type> GetEntities()
		{
			return typeof (Animal).Assembly.GetTypes().Where(t => t.Namespace == typeof (Animal).Namespace);
		}

		#endregion

		private void CustomizeRelations(Mapper mapper)
		{
			/* TODO: add IDomainInspector.IsOptionalOneToMany to avoid auto OnDelete.Cascade and soft-Cascade actions.
				IsOptionalOneToMany may come in place using Declared.Explicit in the ORM */
			mapper.Class<User>(cm =>
			{
				cm.Id(u => u.Id, im => im.Generator(Generators.Foreign<User>(u => u.Human)));
				cm.OneToOne(u => u.Human, otom => otom.Constrained(true));
			});
			mapper.Class<Human>(cm => cm.Bag(human => human.Pets, bagm =>
			{
				bagm.Cascade(CascadeOn.None);
				bagm.Key(km => km.OnDelete(OnDeleteAction.NoAction));
			}, cer => { }));
			mapper.Class<Zoo>(cm =>
			{
				cm.Map(zoo => zoo.Mammals, mapm =>
				{
					mapm.Cascade(CascadeOn.None);
					mapm.Key(km => km.OnDelete(OnDeleteAction.NoAction));
					mapm.Inverse(false);
				}, x=> { }, cer => { });
				cm.Map(zoo => zoo.Animals, mapm =>
				{
					mapm.Cascade(CascadeOn.None);
					mapm.Key(km => km.OnDelete(OnDeleteAction.NoAction));
				}, x => { }, cer => { });
			});
		}

		private void CustomizeColumns(Mapper mapper)
		{
		}

		private void CustomizeTables(Mapper mapper)
		{
		}
	}
}