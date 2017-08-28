using System;
using System.Windows;
using System.Windows.Markup;
using System.Xaml;

namespace DaveSexton.XmlGel
{
	/// <summary>
	/// A markup extension for routed events to invoke a method.
	/// </summary>
	public sealed class InvokeMethodExtension : MarkupExtension
	{
		public string MethodName
		{
			get;
			set;
		}

		public string ElementName
		{
			get;
			set;
		}

		public bool Handled
		{
			get;
			set;
		}

		public object[] Parameters
		{
			get;
			set;
		}

		private InvokeMethodExtension()
		{
			Handled = true;
		}

		public InvokeMethodExtension(string methodName)
			: this()
		{
			MethodName = methodName;
		}

		public InvokeMethodExtension(string elementName, string methodName)
			: this()
		{
			ElementName = elementName;
			MethodName = methodName;
		}

		private bool InvokeMethod(UIElement element)
		{
			var method = element.GetType().GetMethod(MethodName);

			if (method != null)
			{
				method.Invoke(element, Parameters);
				return true;
			}

			return false;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return new RoutedEventHandler((sender, e) =>
				{
					UIElement element;

					if (ElementName != null)
					{
						var service = (IXamlNameResolver) serviceProvider.GetService(typeof(IXamlNameResolver));

						if (service == null)
						{
							return;
						}

						element = service.Resolve(ElementName) as UIElement;
					}
					else
					{
						element = sender as UIElement;
					}

					if (element != null && InvokeMethod(element) && Handled)
					{
						e.Handled = true;
					}
				});
		}
	}
}