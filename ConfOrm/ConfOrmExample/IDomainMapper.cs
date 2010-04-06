using System;
using System.Collections.Generic;
using ConfOrm;
using ConfOrm.NH;

namespace ConfOrmExample
{
	public interface IDomainMapper
	{
		ObjectRelationalMapper DomainDefinition { get; }
		Mapper Mapper { get; }
	}
}