using System;
using System.ComponentModel;
using System.Reactive;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DaveSexton.XmlGel.Labs.WPF
{
	[DisplayName("")]
	[Description("")]
	public partial class EventMarkupExtensionLab : BaseLab
	{
		public EventMarkupExtensionLab()
		{
			InitializeComponent();

			ApplicationCommands.Save.Subscribe(
				this,
				(EventPattern<ExecutedRoutedEventArgs> e) =>
					TraceLine("# items added to selection: " + ((SelectionChangedEventArgs) e.EventArgs.Parameter).AddedItems.Count));

			Loaded += EventMarkupExtensionLab_Loaded;
		}

		private void EventMarkupExtensionLab_Loaded(object sender, RoutedEventArgs e)
		{
			TraceLine(".NET Framework Version: {0}", Environment.Version);
		}
	}
}