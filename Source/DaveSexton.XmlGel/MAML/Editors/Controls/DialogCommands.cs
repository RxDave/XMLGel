using System.Windows.Input;

namespace DaveSexton.XmlGel.Maml.Editors.Controls
{
	internal static class DialogCommands
	{
		public static readonly RoutedUICommand Ok = new RoutedUICommand("OK", "Ok", typeof(DialogCommands));
		public static readonly RoutedUICommand Cancel = new RoutedUICommand("Cancel", "Cancel", typeof(DialogCommands));
	}
}