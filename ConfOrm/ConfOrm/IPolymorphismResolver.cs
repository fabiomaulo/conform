using System;
using System.Collections.Generic;

namespace ConfOrm
{
	public interface IPolymorphismResolver
	{
		IEnumerable<Type> GetBaseImplementors(Type ancestor);
		void Add(Type type);
		void Exclude(Type type);
	}
}