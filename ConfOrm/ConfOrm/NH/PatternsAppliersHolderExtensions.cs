using System;
using System.Collections.Generic;
using System.Reflection;

namespace ConfOrm.NH
{
	public static class PatternsAppliersHolderExtensions
	{
		/// <summary>
		/// Add the <paramref name="applier"/> to the correspondig collection inside the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="TSubject">The subject of the pattern.</typeparam>
		/// <typeparam name="TApplyTo">The target of the applier.</typeparam>
		/// <param name="source">An instance of <see cref="IPatternsAppliersHolder"/>>.</param>
		/// <param name="applier">The imstance of the applier to add.</param>
		public static void Merge<TSubject, TApplyTo>(this IPatternsAppliersHolder source,IPatternApplier<TSubject, TApplyTo> applier)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (applier == null)
			{
				return;
			}
			var patternsAppliersCollection = GetCollectionPropertyOf<TSubject, TApplyTo>(source);
			if (patternsAppliersCollection != null)
			{
				patternsAppliersCollection.Add(applier);
			}
		}

		private static ICollection<IPatternApplier<TSubject, TApplyTo>> GetCollectionPropertyOf<TSubject, TApplyTo>(IPatternsAppliersHolder source)
		{
			var property = typeof(IPatternsAppliersHolder).GetFirstPropertyOfType(typeof(ICollection<IPatternApplier<TSubject, TApplyTo>>));
			if (property != null)
			{
				return (ICollection<IPatternApplier<TSubject, TApplyTo>>)(((PropertyInfo)property).GetValue(source, null));
			}
			return null;
		}
	}
}