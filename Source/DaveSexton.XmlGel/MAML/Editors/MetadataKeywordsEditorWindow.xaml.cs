using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace DaveSexton.XmlGel.Maml.Editors
{
	/// <summary>
	/// Interaction logic for MetadataKeywordsEditorWindow.xaml
	/// </summary>
	/// <remarks>
	/// TODO: Format docs.
	/// Feature: 
	/// You may enter custom index values into the Index column.  (If you type "B", for example, then "Dynamic Link (B)" will 
	/// be selected automatically since it already has the value "B".  But if you type "A", which doesn't have any corresponding 
	/// index name by default, then a new index name of "A" with a value of "A" will be created and will appear as an option 
	/// in the drop-down list so you can easily add additional keyword values for the new index.)
	/// </remarks>
	public sealed partial class MetadataKeywordsEditorWindow : Window
	{
		private static readonly List<NameValuePair> commonIndexNames = new List<NameValuePair>()
		{
			new NameValuePair("Keyword (K)", "K"), 
			new NameValuePair("Context-sensitive (F)", "F"), 
			new NameValuePair("Search Window (S)", "S"), 
			new NameValuePair("Dynamic Link (B)", "B")
		};

		private readonly IMamlMetadata metadata;
		private readonly ObservableCollection<KeywordIndexEntry> keywords;
		private readonly NameValuePairEqualityComparer indexValueComparer = new NameValuePairEqualityComparer(StringComparison.OrdinalIgnoreCase, compareNames: false);
		private ObservableCollection<NameValuePair> uniqueIndexNames;

		public MetadataKeywordsEditorWindow(IMamlMetadata metadata)
		{
			if (metadata == null)
				throw new ArgumentNullException("metadata");

			this.metadata = metadata;

			keywords = new ObservableCollection<KeywordIndexEntry>(
				metadata.KeywordIndexes.SelectMany(
					index => metadata.GetKeywords(index),
					(index, keyword) => new KeywordIndexEntry(
						new NameValuePair(GetCommonIndexName(index) ?? index, index),
						keyword,
						metadata.GetSubkeywords(index, keyword)
										.Select(s => new Subkeyword() { Value = s })
				)));

			InitializeComponent();
		}

		private static string GetCommonIndexName(string value)
		{
			foreach (var name in commonIndexNames)
			{
				if (name.Value.Equals(value, StringComparison.OrdinalIgnoreCase))
				{
					return name.Name;
				}
			}

			return null;
		}

		private void Initialize()
		{
			var indexNames = (ObjectDataProvider) FindResource("indexNames");

			indexNames.ObjectInstance = uniqueIndexNames = new ObservableCollection<NameValuePair>(
				metadata.KeywordIndexes
					.Select(index => new NameValuePair(GetCommonIndexName(index) ?? index, index))
					.Union(commonIndexNames, indexValueComparer)
					.OrderBy(n => n.Name, StringComparer.OrdinalIgnoreCase));

			keywordsDataGrid.ItemsSource = keywords;
		}

		private void SaveKeywords()
		{
			//metadata.SuspendChangeNotification(true);

			try
			{
				metadata.ClearKeywords();

				foreach (KeywordIndexEntry entry in keywords)
				{
					if (entry.Index != null
						&& !string.IsNullOrEmpty(entry.Index.Value)
						&& !string.IsNullOrEmpty(entry.Keyword))
					{
						metadata.SetKeyword(
							entry.Index.Value,
							entry.Keyword,
							entry.Subkeywords.Select(k => k.Value)
															 .Where(k => !string.IsNullOrEmpty(k))
															 .ToArray());
					}
				}
			}
			finally
			{
				//metadata.ResumeChangeNotification();
			}
		}

		private bool IsUniqueIndexValue(string userValue, ref NameValuePair selectedItem)
		{
			if (selectedItem != null && userValue.Equals(selectedItem.Name, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			else
			{
				selectedItem = GetIndexName(userValue);

				return selectedItem == null;
			}
		}

		private NameValuePair GetIndexName(string userValue)
		{
			return uniqueIndexNames.FirstOrDefault(index => userValue.Equals(index.Value, StringComparison.OrdinalIgnoreCase));
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Initialize();
		}

		private void ComboBox_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			ComboBox box = (ComboBox) sender;

			if (box.Text.Length > 0)
			{
				if (box.SelectedIndex < 0)
				{
					var name = GetIndexName(box.Text);

					if (name != null)
					// for example, user enters "F" but no index name begins with "F".  So there is no 
					// selected item but "F" just so happens to be a value already for "Context-sensitive (F)".
					// Instead of adding a new index value of "F" the existing value should be selected instead.
					{
						box.SelectedItem = name;
						return;
					}
				}
				else
				{
					var originalName = box.SelectedItem as NameValuePair;
					var name = originalName;

					if (!IsUniqueIndexValue(box.Text, ref name))
					// user entered the full name of the selected item or the specified value already 
					// belongs to a different name in the collection.
					{
						if (name != originalName)
							box.SelectedItem = name;
						return;
					}
				}

				var items = (IList<NameValuePair>) box.ItemsSource;

				items.Add(new NameValuePair(box.Text, box.Text));

				box.SelectedValue = box.Text;
			}
		}

		private void okButton_Click(object sender, RoutedEventArgs e)
		{
			SaveKeywords();

			DialogResult = true;
		}

		private void cancelButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}

		private sealed class KeywordIndexEntry
		{
			public NameValuePair Index
			{
				get;
				set;
			}

			public string Keyword
			{
				get;
				set;
			}

			public ObservableCollection<Subkeyword> Subkeywords
			{
				get
				{
					return subkeywords;
				}
			}

			private readonly ObservableCollection<Subkeyword> subkeywords;

			/// <summary>
			/// Constructs a new instance of the <see cref="KeywordIndexEntry" /> class.
			/// </summary>
			/// <remarks>
			/// The <see cref="KeywordIndexEntry()"/> constructor overload is required for data binding.
			/// <see cref="Microsoft.Windows.Controls.DataGrid"/> looks for the parameterless constructor 
			/// when instantiating a type that represents a new item.
			/// </remarks>
			public KeywordIndexEntry()
			{
				// set Keyword (K) as the default, merely as a convenience for end-users
				Index = MetadataKeywordsEditorWindow.commonIndexNames[0];

				subkeywords = new ObservableCollection<Subkeyword>();
			}

			/// <summary>
			/// Constructs a new instance of the <see cref="KeywordIndexEntry" /> class.
			/// </summary>
			public KeywordIndexEntry(NameValuePair index, string keyword, IEnumerable<Subkeyword> subkeywords)
			{
				Index = index;
				Keyword = keyword;
				this.subkeywords = new ObservableCollection<Subkeyword>(subkeywords);
			}
		}

		private sealed class Subkeyword
		{
			public string Value
			{
				get;
				set;
			}
		}
	}
}