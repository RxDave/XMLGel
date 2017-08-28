using System;
using System.Windows;
using System.Windows.Media;

namespace DaveSexton.XmlGel.Extensions
{
	public static class RectExtensions
	{
		const double degreesToRadians = Math.PI / 180;

		public static Point GetCenter(this Rect box)
		{
			return new Point(box.Left + box.Width / 2, box.Top + box.Height / 2);
		}

		public static Rect Rotate(this Rect box, double degrees)
		{
			var center = box.GetCenter();

			return Rotate(box, degrees, center.X, center.Y);
		}

		public static Rect Rotate(this Rect box, double degrees, Point center)
		{
			return Rotate(box, degrees, center.X, center.Y);
		}

		public static Rect Rotate(this Rect box, double degrees, double centerX, double centerY)
		{
			if (degrees == 0)
			{
				return box;
			}

			var matrix = Matrix.Identity;

			matrix.RotateAt(degrees, centerX, centerY);

			box.Transform(matrix);

			return box;
		}
	}
}