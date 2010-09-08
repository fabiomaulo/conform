using System.Collections.Generic;
using ConfOrm.Shop.Inflectors;
using NUnit.Framework;

namespace ConfOrm.ShopTests.InflectorsTests
{
	public abstract class CommonInflectorTests
	{
		public readonly Dictionary<string, string> SingularToPlural = new Dictionary<string, string>();
		public IInflector TestInflector { get; set; }

		[Test]
		public void Pluralize()
		{
			foreach (KeyValuePair<string, string> keyValuePair in SingularToPlural)
			{
				Assert.AreEqual(keyValuePair.Value, TestInflector.Pluralize(keyValuePair.Key));
			}
		}

		[Test]
		public void Singularize()
		{
			foreach (KeyValuePair<string, string> keyValuePair in SingularToPlural)
			{
				Assert.AreEqual(keyValuePair.Key, TestInflector.Singularize(keyValuePair.Value));
			}
		}
	}
}