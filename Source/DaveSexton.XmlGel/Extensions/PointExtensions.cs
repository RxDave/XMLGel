using System.Windows;

namespace DaveSexton.XmlGel.Extensions
{
	public static class PointExtensions
	{
		public static Point Negate(this Point point)
		{
			return new Point(-point.X, -point.Y);
		}
	}
}