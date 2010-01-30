namespace ConfOrm.Patterns
{
	public class PropertyToFieldCamelCasePattern: AbstractPropertyToFieldPattern
	{
		#region Implementation of IPattern<PropertyInfo>

		#endregion

		protected override string GetFieldName(string propertyName)
		{
			return propertyName.Substring(0, 1).ToLowerInvariant() + propertyName.Substring(1);
		}
	}
}