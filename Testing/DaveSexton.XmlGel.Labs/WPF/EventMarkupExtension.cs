using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

namespace DaveSexton.XmlGel.Labs.WPF
{
	public sealed class EventMarkupExtension : MarkupExtension
	{
		private readonly ICommand command;

		public EventMarkupExtension(ICommand command)
		{
			this.command = command;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			var provider = (IProvideValueTarget) serviceProvider.GetService(typeof(IProvideValueTarget));

			return new RoutedEventHandler((sender, e) => command.Execute(e));
		}
	}
}