using System;
using System.Collections.Generic;
using System.Linq;

namespace ConfOrm.Shop.Inflectors
{
	public static class InflectorExtensions
	{
		private static readonly HashSet<IInflectorRuleApplier> UnaccentRules = new HashSet<IInflectorRuleApplier>();

		static InflectorExtensions()
		{
			AddUnaccent("([ÀÁÂÃÄÅÆ])", "A");
			AddUnaccent("([Ç])", "C");
			AddUnaccent("([ÈÉÊË])", "E");
			AddUnaccent("([ÌÍÎÏ])", "I");
			AddUnaccent("([Ð])", "D");
			AddUnaccent("([Ñ])", "N");
			AddUnaccent("([ÒÓÔÕÖØ])", "O");
			AddUnaccent("([ÙÚÛÜ])", "U");
			AddUnaccent("([Ý])", "Y");
			AddUnaccent("([Þ])", "T");
			AddUnaccent("([ß])", "s");
			AddUnaccent("([àáâãäåæ])", "a");
			AddUnaccent("([ç])", "c");
			AddUnaccent("([èéêë])", "e");
			AddUnaccent("([ìíîï])", "i");
			AddUnaccent("([ð])", "e");
			AddUnaccent("([ñ])", "n");
			AddUnaccent("([òóôõöø])", "o");
			AddUnaccent("([ùúûü])", "u");
			AddUnaccent("([ý])", "y");
			AddUnaccent("([þ])", "t");
			AddUnaccent("([ÿ])", "y");
		}

		public static string Unaccent(this string word)
		{
			if (word == null)
			{
				return word;
			}
			return UnaccentRules.Aggregate(word, (current, rule) => rule.Apply(current));
		}

		private static void AddUnaccent(string rule, string replacement)
		{
			UnaccentRules.Add(new CaseSensitiveRule(rule, replacement));
		}
	}
}