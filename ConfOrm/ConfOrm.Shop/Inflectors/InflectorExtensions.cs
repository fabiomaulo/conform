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
			AddUnaccent("([�������])", "A");
			AddUnaccent("([�])", "C");
			AddUnaccent("([����])", "E");
			AddUnaccent("([����])", "I");
			AddUnaccent("([�])", "D");
			AddUnaccent("([�])", "N");
			AddUnaccent("([������])", "O");
			AddUnaccent("([����])", "U");
			AddUnaccent("([�])", "Y");
			AddUnaccent("([�])", "T");
			AddUnaccent("([�])", "s");
			AddUnaccent("([�������])", "a");
			AddUnaccent("([�])", "c");
			AddUnaccent("([����])", "e");
			AddUnaccent("([����])", "i");
			AddUnaccent("([�])", "e");
			AddUnaccent("([�])", "n");
			AddUnaccent("([������])", "o");
			AddUnaccent("([����])", "u");
			AddUnaccent("([�])", "y");
			AddUnaccent("([�])", "t");
			AddUnaccent("([�])", "y");
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