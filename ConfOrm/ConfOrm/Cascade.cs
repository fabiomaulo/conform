using System;

namespace ConfOrm
{
	[Flags]
	public enum Cascade
	{
		None = 0,
		Persist= 2,
		Refresh= 4,
		Merge= 8,
		Remove= 16,
		Detach= 32,
		ReAttach= 64,
		DeleteOrphans = 128,
		All = 256
	}
}