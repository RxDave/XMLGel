using System;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace DaveSexton.XmlGel.Maml
{
	public sealed class AlertKindToImageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var kind = (AlertKind) value;

			string imageResourceName;

			switch (kind)
			{
				case AlertKind.Caution:
				case AlertKind.Important:
				case AlertKind.Warning:
					imageResourceName = "Alert Caution.gif";
					break;
				case AlertKind.Security:
					imageResourceName = "Alert Security.gif";
					break;
				default:
					imageResourceName = "Alert Note.gif";
					break;
			}

			return new BitmapImage(new Uri("pack://application:,,,/DaveSexton.XmlGel;component/Maml/Documents/Images/" + imageResourceName));
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}