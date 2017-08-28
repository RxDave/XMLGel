using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DaveSexton.XmlGel
{
	public sealed class DefaultedTextBox : TextBox
	{
		public static readonly DependencyProperty DefaultTextProperty = DependencyProperty.Register("DefaultText", typeof(string), typeof(DefaultedTextBox), new FrameworkPropertyMetadata("(Unset)"));
		public static readonly DependencyProperty IsDefaultedProperty = DependencyProperty.Register("IsDefaulted", typeof(bool), typeof(DefaultedTextBox), new FrameworkPropertyMetadata(false));

		public string DefaultText
		{
			get
			{
				return (string) GetValue(DefaultTextProperty);
			}
			set
			{
				SetValue(DefaultTextProperty, value);
			}
		}

		public bool IsDefaulted
		{
			get
			{
				return (bool) GetValue(IsDefaultedProperty);
			}
			set
			{
				SetValue(IsDefaultedProperty, value);
			}
		}

		private bool hasTextChanged;

		public DefaultedTextBox()
		{
			Loaded += DefaultedTextBox_Loaded;
		}

		private void DefaultedTextBox_Loaded(object sender, RoutedEventArgs e)
		{
			if (!hasTextChanged)
			{
				IsDefaulted = true;
			}
		}

		protected override void OnTextChanged(TextChangedEventArgs e)
		{
			hasTextChanged = true;

			if (!IsKeyboardFocused)
			{
				if (IsDefaulted)
				{
					if (Text.Length > 0 && !string.Equals(Text, DefaultText, StringComparison.Ordinal))
					{
						IsDefaulted = false;
					}
				}
				else if (Text.Length == 0)
				{
					IsDefaulted = true;
				}
			}

			base.OnTextChanged(e);
		}

		protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
		{
			IsDefaulted = false;

			base.OnGotKeyboardFocus(e);
		}

		protected override void OnLostFocus(RoutedEventArgs e)
		{
			if (Text.Length == 0)
			{
				IsDefaulted = true;
			}

			base.OnLostFocus(e);
		}
	}
}