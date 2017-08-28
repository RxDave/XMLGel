using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Windows;

namespace DaveSexton.XmlGel.Maml.Editors
{
	/// <summary>
	/// Interaction logic for MetadataAttributesEditorWindow.xaml
	/// </summary>
	public sealed partial class MetadataAttributesEditorWindow : Window
	{
		public MetadataAttributesEditorWindow(IMamlMetadata metadata)
		{
			Contract.Requires(metadata != null);

			InitializeComponent();

			attributesGrid.Metadata = metadata;
		}

		public MetadataAttributesEditorWindow(IDictionary<string, List<string>> originalAttributes)
		{
			Contract.Requires(originalAttributes != null);

			InitializeComponent();

			attributesGrid.OriginalAttributes = originalAttributes;
		}

		private void okButton_Click(object sender, RoutedEventArgs e)
		{
			attributesGrid.CommitChanges();

			DialogResult = true;
		}

		private void cancelButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}
	}
}