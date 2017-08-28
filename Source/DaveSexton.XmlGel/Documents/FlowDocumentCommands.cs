using System.Windows.Input;

namespace DaveSexton.XmlGel.Documents
{
	public static class FlowDocumentCommands
	{
		public static RoutedCommand UIElementChanged = new RoutedCommand("UIElementChanged", typeof(FlowDocumentCommands));
	}
}