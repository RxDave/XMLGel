using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace DaveSexton.XmlGel.Extensions
{
	internal static class DrawingContextExtensions
	{
		public static void DrawText(this DrawingContext drawingContext, string text, Typeface typeface, double fontSize, Brush brush, Point origin, double offsetX = 0, double offsetY = 0, Transform transform = null)
		{
			var formattedText = new FormattedText(
				text,
				CultureInfo.CurrentCulture,
				FlowDirection.LeftToRight,
				typeface,
				fontSize,
				brush);

			origin.Offset(offsetX, offsetY);

			if (transform != null)
			{
				drawingContext.PushTransform(transform);
			}

			try
			{
				drawingContext.DrawText(formattedText, origin);
			}
			finally
			{
				if (transform != null)
				{
					drawingContext.Pop();
				}
			}
		}
	}
}