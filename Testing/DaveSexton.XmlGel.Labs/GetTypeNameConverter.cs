using System;
using System.Windows.Data;

namespace DaveSexton.XmlGel.Labs
{
	public sealed class GetTypeNameConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value == null ? "null" : value.GetType().FullName;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}