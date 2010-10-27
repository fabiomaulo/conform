using System;
using System.Text.RegularExpressions;

namespace ConfOrm.UsageExamples.TableAndColumnNaming
{
	public static class MyInflector
	{
		public static string Underscore(this string pascalCasedWord)
		{
			if (string.IsNullOrEmpty(pascalCasedWord))
			{
				return pascalCasedWord;
			}

			return Regex.Replace(
							Regex.Replace(
											Regex.Replace(pascalCasedWord, @"([A-Z]+)([A-Z][a-z])", "$1_$2"), @"([a-z\d])([A-Z])",
											"$1_$2"), @"[-\s]", "_").ToLower();
		}
	}
}