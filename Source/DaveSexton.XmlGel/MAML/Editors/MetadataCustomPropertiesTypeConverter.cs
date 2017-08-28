using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace DaveSexton.XmlGel.Maml.Editors
{
	internal sealed class MetadataCustomPropertiesTypeConverter : TypeConverter
	{
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				var properties = value as ICollection<string>;

				if (properties != null)
				{
					return string.Format(CultureInfo.CurrentCulture, "{0} properties", properties.Count);
				}
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}