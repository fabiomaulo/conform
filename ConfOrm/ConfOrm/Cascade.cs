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
		All = 256,

		/// <summary>
		/// It include each single <see cref="Cascade"/> value.
		/// </summary>
		/// <remarks>
		/// It can be useful as a shortcut when a specific exclusion is needed as for example:
		/// <example>
		/// to exclude only the <see cref="Refresh"/>
		/// <code>
		/// ConfOrm.Cascade.Every.Exclude(ConfOrm.Cascade.Refresh)
		/// </code>
		/// </example>
		/// </remarks>
		Every = Persist | Refresh | Merge | Remove | Detach | ReAttach | DeleteOrphans
	}
}