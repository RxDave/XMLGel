using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

namespace DaveSexton.XmlGel
{
	/// <summary>
	/// A markup extension for routed events to invoke the specified command.
	/// </summary>
	public sealed class CommandExtension : MarkupExtension
	{
		private readonly ICommand command;
		private readonly object parameter;
		private readonly bool hasParameter;

		public CommandExtension(ICommand command)
		{
			this.command = command;
		}

		public CommandExtension(ICommand command, object parameter)
		{
			this.command = command;
			this.parameter = parameter;

			hasParameter = true;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			var provider = (IProvideValueTarget) serviceProvider.GetService(typeof(IProvideValueTarget));

			return new RoutedEventHandler((sender, e) => command.Execute(hasParameter ? parameter : e));
		}
	}
}