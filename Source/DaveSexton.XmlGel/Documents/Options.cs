using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace DaveSexton.XmlGel.Documents
{
	public sealed class Options : InlineUIContainer, IUIElementContainer
	{
		public object SelectedValue
		{
			get
			{
				return comboBox.SelectedValue;
			}
			set
			{
				comboBox.SelectedValue = value;
			}
		}

		public string SelectedValueAsString
		{
			get
			{
				return getTextFromValue(comboBox.SelectedValue);
			}
		}

		public bool AllowCustomOption
		{
			get
			{
				return comboBox.IsEditable;
			}
			set
			{
				comboBox.IsEditable = false;
			}
		}

		private readonly ComboBox comboBox = new ComboBox()
		{
			DisplayMemberPath = "Name",
			SelectedValuePath = "Value"
		};

		private readonly Func<string, object> getValueFromText;
		private readonly Func<object, string> getTextFromValue;

		public Options(Func<string, object> getValueFromText, Func<object, string> getTextFromValue, params KeyValuePair<object, object>[] options)
			: this(getValueFromText, getTextFromValue, (IEnumerable<KeyValuePair<object, object>>) options)
		{
		}

		public Options(Func<string, object> getValueFromText, Func<object, string> getTextFromValue, IEnumerable<KeyValuePair<object, object>> options)
		{
			this.getValueFromText = getValueFromText;
			this.getTextFromValue = getTextFromValue;

			foreach (var option in options.Select(option => new OptionItem(option.Key, option.Value)))
			{
				comboBox.Items.Add(option);
			}

			comboBox.SelectionChanged += (_, e) => OnValueChanged(e);

			Child = comboBox;
		}

		public void SelectValueFromText(string value)
		{
			comboBox.SelectedValue = getValueFromText(value);
		}

		public event RoutedEventHandler ValueChanged;

		private void OnValueChanged(RoutedEventArgs e)
		{
			var handler = ValueChanged;

			if (handler != null)
			{
				handler(this, e);
			}
		}
	}
}