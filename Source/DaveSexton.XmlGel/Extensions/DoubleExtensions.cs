namespace DaveSexton.XmlGel.Extensions
{
	internal static class DoubleExtensions
	{
		public static double ZeroIfNaN(this double value)
		{
			return double.IsNaN(value) ? 0 : value;
		}
	}
}