using System.ComponentModel;
using System.Linq;

namespace DaveSexton.XmlGel.Maml.Editors
{
	internal sealed class DocumentPropertiesTypeConverter : SortedPropertyConverter
	{
		protected override PropertyDescriptorCollection GetSortedProperties(ITypeDescriptorContext context, object value, PropertyDescriptorCollection unsortedProperties)
		{
			var obj = value as DocumentProperties;

			if (obj != null)
			{
				var metadata = obj.Metadata;

				var customProperties = metadata.CustomProperties.ToDictionary(p => p, p => metadata.GetCustomPropertyValue(p));

				var list = unsortedProperties.Cast<PropertyDescriptor>().ToList();

				foreach (var property in customProperties)
				{
					list.Add(new CustomPropertyDescriptor(
						property.Key, property.Value ?? string.Empty, customProperties,
						new SortedPropertyAttribute(20),
						new CategoryAttribute(DocumentProperties.CustomPropertiesCategory),
						new DefaultValueAttribute(property.Value ?? string.Empty),
						new DescriptionAttribute("Specifies a custom metadata XML element named, \"" + property.Key + "\".")));
				}

				unsortedProperties = new PropertyDescriptorCollection(list.ToArray());
			}

			return base.GetSortedProperties(context, value, unsortedProperties);
		}
	}
}