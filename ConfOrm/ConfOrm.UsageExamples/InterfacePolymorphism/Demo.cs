using System;
using ConfOrm.NH;
using NUnit.Framework;

namespace ConfOrm.UsageExamples.InterfacePolymorphism
{
	public class Demo
	{
		[Test, Explicit]
		public void UseBagForLists()
		{
			var entities = new[] { typeof(User), typeof(Email), typeof(InstantMessage) };
			var orm = new ObjectRelationalMapper();
			orm.TablePerClass(entities);

			var mapper = new Mapper(orm);
			mapper.Customize<IHasMessage>(x => x.Property(hasMessage => hasMessage.Message, pm => { pm.Length(11000); pm.Lazy(true); }));
			mapper.Customize<Tweet>(x => x.Property(tweet => tweet.Message, pm => { pm.Length(140); pm.Lazy(false); }));

			// Show the mapping to the console
			var mapping = mapper.CompileMappingFor(entities);
			Console.Write(mapping.AsString());
		}
	}
}