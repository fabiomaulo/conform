using System;
using ConfOrm.NH;
using NHibernate;
using NUnit.Framework;

namespace ConfOrm.UsageExamples.InterfacePolymorphism
{
	public class Demo
	{
		[Test, Explicit]
		public void InterfacePolymorphisDemo()
		{
			// In this example you can see how you can specify the mapping using an interface that will be implemented by some classes of your domain
			// no matter if they are entities or component and over all even when the interface is not really part of the mapping (not an entity).
			// Even when you have defined a mapping using an interface you can always override the mapping for a specific implementation (see Customize<Tweet>)

			var entities = new[] { typeof(User), typeof(Email), typeof(InstantMessage) };
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass(entities);

			var mapper = new Mapper(orm);
			mapper.Customize<IHasMessage>(x => x.Property(hasMessage => hasMessage.Message, pm => { pm.Type(NHibernateUtil.StringClob); pm.Lazy(true); }));
			mapper.Customize<Tweet>(x => x.Property(tweet => tweet.Message, pm => { pm.Type(NHibernateUtil.String); pm.Length(140); pm.Lazy(false); }));

			// Show the mapping to the console
			var mapping = mapper.CompileMappingFor(entities);
			Console.Write(mapping.AsString());
		}
	}
}