using System.Linq;
using ConfOrm;
using ConfOrm.NH;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using Iesi.Collections.Generic;
using SharpTestsEx;

namespace ConfOrmTests.ObjectRelationalMapperTests
{
	public class ImplicitPolymorphismsWithGenericOneToManyTest
	{
		private class Node<T> where T : Node<T>
		{
			public int Id { get; set; }
			public T Parent { get; set; }
			public ISet<T> Subnodes { get; private set; }
		}

		private class ConcreteNode : Node<ConcreteNode>
		{
		}

		[Test]
		public void ShouldMapParentChild()
		{
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass<ConcreteNode>();

			var mapper = new Mapper(orm);
			var mappings = mapper.CompileMappingFor(new[] {typeof(ConcreteNode)});
			var hbmClass = mappings.RootClasses.Single();
			var hbmSet = hbmClass.Properties.OfType<HbmSet>().Single();
			
			hbmSet.ElementRelationship.Should().Be.OfType<HbmOneToMany>();
			hbmSet.Cascade.Should().Contain("all").And.Contain("delete-orphan");
		}
	}
}