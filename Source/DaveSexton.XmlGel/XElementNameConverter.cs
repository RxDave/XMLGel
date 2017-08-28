using System;
using System.Globalization;
using System.Windows.Data;
using System.Xml.Linq;

namespace DaveSexton.XmlGel
{
	public sealed class XElementNameConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var element = value as XElement;

			return element == null ? null : element.Name.ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}