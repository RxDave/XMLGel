using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows;

namespace DaveSexton.XmlGel.Maml.Editors
{
	/// <summary>
	/// Interaction logic for MetadataCustomPropertiesEditorWindow.xaml
	/// </summary>
	public sealed partial class MetadataCustomPropertiesEditorWindow : Window
	{
		private static readonly IList<string> commonCustomProperties = new List<string>()
		{
			// TODO: Add common custom properties
		}
		.AsReadOnly();

		private readonly IMamlMetadata metadata;
		private readonly ObservableCollection<NameValuePair> properties;

		public MetadataCustomPropertiesEditorWindow(IMamlMetadata metadata)
		{
			Contract.Requires(metadata != null);

			this.metadata = metadata;

			properties = new ObservableCollection<NameValuePair>(
				metadata.CustomProperties.Select(property =>
					new NameValuePair(property, metadata.GetCustomPropertyValue(property))));

			InitializeComponent();
		}

		private void Initialize()
		{
			propertiesDataGrid.ItemsSource = properties;
		}

		private void SaveProperties()
		{
			//metadata.SuspendChangeNotification(true);

			try
			{
				metadata.ClearCustomProperties();

				foreach (var property in properties)
				{
					if (!string.IsNullOrEmpty(property.Name))
					{
						metadata.SetCustomPropertyValue(property.Name, property.Value);
					}
				}
			}
			finally
			{
				//metadata.ResumeChangeNotification();
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Initialize();
		}

		private void okButton_Click(object sender, RoutedEventArgs e)
		{
			SaveProperties();

			DialogResult = true;
		}

		private void cancelButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}
	}
}