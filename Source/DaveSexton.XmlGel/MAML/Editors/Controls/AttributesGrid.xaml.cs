using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;

namespace DaveSexton.XmlGel.Maml.Editors.Controls
{
	/// <summary>
	/// Interaction logic for MetadataAttributesEditorWindow.xaml
	/// </summary>
	public sealed partial class AttributesGrid : UserControl
	{
		public ObservableCollection<NameValuePair> Attributes
		{
			get
			{
				return attributes;
			}
		}

		public ObservableCollection<string> AttributeNames
		{
			get
			{
				return attributeNames;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return attributesDataGrid.IsReadOnly;
			}
			set
			{
				attributesDataGrid.IsReadOnly = value;
			}
		}

		public IMamlMetadata Metadata
		{
			get
			{
				return metadata;
			}
			set
			{
				metadata = value;
				originalAttributes = null;

				var attributes = Attributes;

				attributes.Clear();

				if (metadata != null)
				{
					foreach (var attribute in from name in metadata.AttributeNames
																		from val in metadata.GetAttributeValues(name)
																		select new NameValuePair(name, val))
					{
						attributes.Add(attribute);
					}
				}

				Initialize();
			}
		}

		public IDictionary<string, List<string>> OriginalAttributes
		{
			get
			{
				return originalAttributes;
			}
			set
			{
				originalAttributes = value;
				metadata = null;

				var attributes = Attributes;

				attributes.Clear();

				if (originalAttributes != null)
				{
					foreach (var attribute in from name in originalAttributes.Keys
																		from val in originalAttributes[name]
																		select new NameValuePair(name, val))
					{
						attributes.Add(attribute);
					}
				}

				Initialize();
			}
		}

		private static readonly StringComparer comparer = StringComparer.OrdinalIgnoreCase;

		private readonly ObservableCollection<NameValuePair> attributes = new ObservableCollection<NameValuePair>();
		private readonly ObservableCollection<string> attributeNames = new ObservableCollection<string>();
		private IDictionary<string, List<string>> originalAttributes;
		private IMamlMetadata metadata;

		public AttributesGrid()
		{
			InitializeComponent();
		}

		private void Initialize()
		{
			if (DesignerProperties.GetIsInDesignMode(this))
			{
				return;
			}

			ICollection<string> uniqueNames;

			if (metadata == null)
			{
				if (originalAttributes == null)
				{
					return;
				}

				uniqueNames = originalAttributes.Keys;
			}
			else
			{
				uniqueNames = metadata.AttributeNames;
			}

			attributeNames.Clear();

			foreach (var name in uniqueNames
				.Union(GlobalOptions.CommonAttributeNames, comparer)
				.OrderBy(s => s, comparer))
			{
				attributeNames.Add(name);
			}
		}

		public void CommitChanges()
		{
			if (metadata == null)
			{
				if (originalAttributes == null)
				{
					return;
				}

				UpdateOriginalAttributes();
			}
			else
			{
				metadata.SetAttributes(Attributes.Select(pair => new KeyValuePair<string, string>(pair.Name, pair.Value)));
			}
		}

		private void UpdateOriginalAttributes()
		{
			originalAttributes.Clear();

			foreach (var attribute in Attributes)
			{
				if (!string.IsNullOrEmpty(attribute.Name))
				{
					if (originalAttributes.ContainsKey(attribute.Name))
					{
						List<string> values = originalAttributes[attribute.Name];

						if (values == null)
						{
							values = new List<string>()
								{
									attribute.Value
								};

							originalAttributes[attribute.Name] = values;
						}
						else if (!values.Contains(attribute.Value))
						{
							values.Add(attribute.Value);
						}
					}
					else
					{
						var values = new List<string>()
							{
								attribute.Value
							};

						originalAttributes.Add(attribute.Name, values);
					}
				}
			}
		}

		private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
		{
			var box = e.EditingElement as ComboBox;

			if (e.EditAction == DataGridEditAction.Commit && e.Column == nameColumn && box != null)
			{
				if (box.SelectedIndex < 0 && box.Text.Length > 0)
				{
					var items = (IList<string>) box.ItemsSource;

					if (!items.Contains(box.Text, comparer))
					{
						items.Add(box.Text);
					}

					box.SelectedValue = box.Text;
				}
			}
		}
	}
}