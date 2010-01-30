using System.Reflection;

namespace ConfOrm.Patterns
{
	public abstract class AbstractPropertyToFieldPattern : IPattern<PropertyInfo>
	{
		protected const BindingFlags DefaultBinding =
			BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

		#region IPattern<PropertyInfo> Members

		public bool Match(PropertyInfo subject)
		{
			string fieldName = GetFieldName(subject.Name);
			return subject.DeclaringType.GetField(fieldName, DefaultBinding) != null;
		}

		#endregion

		protected abstract string GetFieldName(string propertyName);
	}
}