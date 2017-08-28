using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace DaveSexton.XmlGel.Maml.Editors
{
	internal sealed class MetadataAttributesTypeConverter : TypeConverter
	{
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				var collection = value as ICollection<string>;

				if (collection != null)
				{
					return string.Format(CultureInfo.CurrentCulture, "{0} attribute(s)", collection.Count);
				}
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}