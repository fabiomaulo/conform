using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ConfOrm.Shop.Inflectors
{
	public static class InflectorExtensions
	{
		private static readonly HashSet<IInflectorRuleApplier> UnaccentRules = new HashSet<IInflectorRuleApplier>();
		public const string UppercaseAccentedCharacters = "������������������������������";
		public const string LowercaseAccentedCharacters = "��������������������������������";

		private static readonly Regex WordsSpliter =
			new Regex(string.Format(@"([A-Z{0}]+[a-z{1}\d]*)|[_\s]", UppercaseAccentedCharacters, LowercaseAccentedCharacters),
								RegexOptions.Compiled);

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

		private static void AddUnaccent(string rule, string replacement)
		{
			UnaccentRules.Add(new CaseSensitiveRule(rule, replacement));
		}

		public static string Unaccent(this string word)
		{
			if (word == null)
			{
				return word;
			}
			return UnaccentRules.Aggregate(word, (current, rule) => rule.Apply(current));
		}

		public static IEnumerable<string> SplitWords(this string composedPascalCaseWords)
		{
			return from Match regex in WordsSpliter.Matches(composedPascalCaseWords) select regex.Value;
		}
	}
}