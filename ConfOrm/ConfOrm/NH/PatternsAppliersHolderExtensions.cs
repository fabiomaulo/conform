using System;
using System.Collections.Generic;
using System.Linq;
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
		/// <param name="applier">The instance of the applier to add.</param>
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
			else
			{
				throw new ArgumentOutOfRangeException("applier",
				                                      string.Format(
				                                      	"The IPatternsAppliersHolder does not support appliers of {0}, {1}",
				                                      	typeof (TSubject).FullName, typeof (TApplyTo).FullName));
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

		/// <summary>
		/// Add or Replace the <paramref name="applier"/> to the correspondig collection inside the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="TSubject">The subject of the pattern.</typeparam>
		/// <typeparam name="TApplyTo">The target of the applier.</typeparam>
		/// <param name="source">An instance of <see cref="IPatternsAppliersHolder"/>>.</param>
		/// <param name="applier">The instance of the applier to add.</param>
		/// <remarks>
		/// The Replace action is performed removing the all appliers with the same type-name, where exists, and then adding the new applier.
		/// This method is usefull when you want override a behaviour of an existing applier in an existing <see cref="IPatternsAppliersHolder"/>.
		/// </remarks>
		public static void UnionWith<TSubject, TApplyTo>(this IPatternsAppliersHolder source, IPatternApplier<TSubject, TApplyTo> applier)
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
				var existingAppliers = patternsAppliersCollection.Where(a=> a.GetType().Name == applier.GetType().Name).ToList();
				if(existingAppliers.Count > 0)
				{
					foreach (var existingApplier in existingAppliers)
					{
						patternsAppliersCollection.Remove(existingApplier);
					}
				}
				patternsAppliersCollection.Add(applier);
			}
			else
			{
				throw new ArgumentOutOfRangeException("applier",
																							string.Format(
																								"The IPatternsAppliersHolder does not support appliers of {0}, {1}",
																								typeof(TSubject).FullName, typeof(TApplyTo).FullName));
			}
		}
	}
}