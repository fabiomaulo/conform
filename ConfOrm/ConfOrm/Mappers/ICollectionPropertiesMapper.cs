using System;
using System.Reflection;

namespace ConfOrm.Mappers
{
	public interface ICollectionPropertiesMapper
	{
		bool Inverse { get; set; }
		bool Mutable { get; set; }
		string Where { get; set; }
		int BatchSize { get; set; }
		CollectionLazy Lazy { get; set; }
		void Key(Action<IKeyMapper> keyMapping);
		void OrderBy(MemberInfo property);
		void Sort();
		void Cascade(Cascade cascadeStyle);
	}
}