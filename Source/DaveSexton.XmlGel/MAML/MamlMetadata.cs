using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace DaveSexton.XmlGel.Maml
{
	public sealed class MamlMetadata : IMamlMetadata
	{
		public const string DefaultFileExtension = ".cmp";

		public Guid Id
		{
			get
			{
				return id;
			}
		}

		public string TableOfContentsTitle
		{
			get
			{
				return tableOfContentsTitle ?? string.Empty;
			}
			set
			{
				if (!string.Equals(tableOfContentsTitle, value, StringComparison.Ordinal))
				{
					tableOfContentsTitle = value;

					RaisePropertyChanged("TableOfContentsTitle");
				}
			}
		}

		public string Title
		{
			get
			{
				return title ?? string.Empty;
			}
			set
			{
				if (!string.Equals(title, value, StringComparison.Ordinal))
				{
					title = value;

					RaisePropertyChanged("Title");
				}
			}
		}

		public string Author
		{
			get
			{
				return author ?? string.Empty;
			}
			set
			{
				if (!string.Equals(author, value, StringComparison.Ordinal))
				{
					author = value;

					RaisePropertyChanged("Author");
				}
			}
		}

		public DateTimeOffset LastModified
		{
			get
			{
				return lastModified;
			}
			set
			{
				if (lastModified != value)
				{
					lastModified = value;

					RaisePropertyChanged("LastModified");
				}
			}
		}

		public ICollection<string> KeywordIndexes
		{
			get
			{
				return keywords.Keys;
			}
		}

		public ICollection<string> AttributeNames
		{
			get
			{
				return attributes.Keys;
			}
		}

		public ICollection<string> CustomProperties
		{
			get
			{
				return customProperties.Keys;
			}
		}

		private static readonly ICollection<string> emptyReadOnlyCollection = new List<string>(0).AsReadOnly();

		// The outer list contains the keywords for the specified index.
		// The inner list contains the actual keyword (at index 0) and its sub keywords.
		private readonly Dictionary<string, List<List<string>>> keywords = new Dictionary<string, List<List<string>>>(StringComparer.OrdinalIgnoreCase);
		private readonly Dictionary<string, List<string>> attributes = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
		private readonly Dictionary<string, string> customProperties = new Dictionary<string, string>(0, StringComparer.Ordinal);
		private readonly Guid id;
		private string tableOfContentsTitle, title, author;
		private DateTimeOffset lastModified;

		public MamlMetadata(Guid id)
		{
			this.id = id;
		}

		public static MamlMetadata ForDocument(string documentFile, Guid id)
		{
			var metadata = new MamlMetadata(id);

			string companionFile = string.IsNullOrWhiteSpace(documentFile)
				? null
				: GetCompanionFileForDocument(documentFile);

			if (File.Exists(companionFile))
			{
				try
				{
					var companion = XDocument.Load(companionFile, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);

					var root = companion.Root;

					if (root.Name == MamlCompanion.Root)
					{
						var assetTypeId = (string) root.Attribute(MamlCompanion.AssetTypeId);
						var fileAssetGuid = (string) root.Attribute(MamlCompanion.FileAssetGuid);

						if (string.Equals(assetTypeId, MamlCompanion.AssetTypeIdValue, StringComparison.OrdinalIgnoreCase)
							&& (string.Equals(fileAssetGuid, "*", StringComparison.Ordinal) || GuidsAreEqual(fileAssetGuid, id)))
						{
							metadata.MergeTopics(root, id);
						}
					}
				}
				catch (DirectoryNotFoundException)
				{
				}
				catch (FileNotFoundException)
				{
				}
			}

			return metadata;
		}

		private static string GetCompanionFileForDocument(string documentFile)
		{
			return Path.ChangeExtension(documentFile, DefaultFileExtension);
		}

		private static bool GuidsAreEqual(string guid1, Guid guid2)
		{
			if (string.IsNullOrWhiteSpace(guid1))
			{
				return guid2 == Guid.Empty;
			}
			else
			{
				Guid guid1Value;
				return Guid.TryParse(guid1, out guid1Value) && guid1Value == guid2;
			}
		}

		public ICollection<string> GetKeywords(string index)
		{
			if (keywords.ContainsKey(index))
			{
				return keywords[index].ConvertAll<string>(list => list[0]).AsReadOnly();
			}
			else
			{
				return emptyReadOnlyCollection;
			}
		}

		public ICollection<string> GetSubkeywords(string index, string keyword)
		{
			bool created;
			List<string> list = GetKeywordList(index, keyword, false, out created);

			if (list == null)
			{
				return emptyReadOnlyCollection;
			}
			else
			{
				return list.Skip(1).ToList().AsReadOnly();
			}
		}

		public void SetKeyword(string index, string keyword, params string[] subkeywords)
		{
			bool created;
			List<string> list = GetKeywordList(index, keyword, true, out created);

			if (subkeywords != null && subkeywords.Length > 0)
			{
				if (!created)
				{
					list.Clear();
					list.Add(keyword);
				}

				// Add the subkeywords in the same list after the main keyword
				list.AddRange(subkeywords);
			}

			RaisePropertyChanged("KeywordIndexes");
		}

		private void SetKeyword(string index, IList<string> keywordAndSubkeywords)
		{
			if (keywordAndSubkeywords.Count == 0)
			{
				throw new ArgumentOutOfRangeException("keywordAndSubkeywords");
			}

			SetKeyword(index, keywordAndSubkeywords[0], keywordAndSubkeywords.Skip(1).ToArray());
		}

		private List<string> GetKeywordList(string index, string keyword, bool canCreateList, out bool created)
		{
			created = false;

			if (keywords.ContainsKey(index))
			{
				List<string> list = (from innerList in keywords[index]
														 where innerList.Count > 0 && string.Equals(innerList[0], keyword, StringComparison.OrdinalIgnoreCase)
														 select innerList)
														 .FirstOrDefault();

				if (list != null || !canCreateList)
				{
					return list;
				}
			}
			else if (canCreateList)
			{
				keywords.Add(index, new List<List<string>>(8));
			}
			else
			{
				return null;
			}

			// create list

			List<string> keywordList = new List<string>() { keyword };

			keywords[index].Add(keywordList);

			created = true;

			return keywordList;
		}

		public ICollection<string> GetAttributeValues(string name)
		{
			if (attributes.ContainsKey(name))
			{
				return attributes[name].AsReadOnly();
			}
			else
			{
				return emptyReadOnlyCollection;
			}
		}

		public void AddAttributeValue(string name, string value)
		{
			AddAttributeValueWithoutChangeEvent(name, value);

			RaisePropertyChanged("AttributeNames");
		}

		private void AddAttributeValueWithoutChangeEvent(string name, string value)
		{
			GetOrCreateAttributeList(name).Add(value);
		}

		public void SetAttributeValues(string name, params string[] values)
		{
			if (values == null || values.Length == 0)
			{
				attributes.Remove(name);
			}
			else
			{
				List<string> list = GetOrCreateAttributeList(name);

				list.Clear();
				list.AddRange(values);
			}

			RaisePropertyChanged("AttributeNames");
		}

		public void SetAttributes(IEnumerable<KeyValuePair<string, string>> allAttributes)
		{
			attributes.Clear();

			foreach (var attribute in allAttributes)
			{
				if (!string.IsNullOrEmpty(attribute.Key))
				{
					AddAttributeValueWithoutChangeEvent(attribute.Key, attribute.Value);
				}
			}

			RaisePropertyChanged("AttributeNames");
		}

		private List<string> GetOrCreateAttributeList(string name)
		{
			if (attributes.ContainsKey(name))
			{
				return attributes[name];
			}
			else
			{
				var list = new List<string>(8);

				attributes.Add(name, list);

				return list;
			}
		}

		public string GetCustomPropertyValue(string name)
		{
			return customProperties[name];
		}

		public void SetCustomPropertyValue(string name, string value)
		{
			customProperties[name] = value;

			RaisePropertyChanged("CustomProperties");
		}

		public void ClearAttributes()
		{
			attributes.Clear();

			RaisePropertyChanged("AttributeNames");
		}

		public void ClearKeywords()
		{
			keywords.Clear();

			RaisePropertyChanged("KeywordIndexes");
		}

		public void ClearCustomProperties()
		{
			customProperties.Clear();

			RaisePropertyChanged("CustomProperties");
		}

		private void MergeTopics(XElement root, Guid id)
		{
			foreach (var topic in from element in root.Elements(Maml.TopicRoot)
														let topicId = element.Attribute(Maml.TopicId)
														where (string) topicId == "*" || GuidsAreEqual((string) topicId, id)
														select element)
			{
				LoadTopic(topic);
			}
		}

		private void LoadTopic(XElement topic)
		{
			var title = topic.Element(MamlCompanion.Title);

			if (title != null)
			{
				Title = (string) title;
			}

			var tableOfContentsTitle = topic.Element(MamlCompanion.TableOfContentsTitle);

			if (tableOfContentsTitle != null)
			{
				TableOfContentsTitle = (string) tableOfContentsTitle;
			}

			var author = topic.Element(MamlCompanion.Author);

			if (author != null)
			{
				Author = (string) author;
			}

			var lastModifiedElement = topic.Element(MamlCompanion.LastModified);

			if (lastModifiedElement != null)
			{
				DateTimeOffset value;
				if (DateTimeOffset.TryParse(
					(string) lastModifiedElement,
					System.Globalization.CultureInfo.CurrentCulture,
					System.Globalization.DateTimeStyles.AllowLeadingWhite | System.Globalization.DateTimeStyles.AllowTrailingWhite,
					out value))
				{
					LastModified = value;
				}
			}

			var attributeData = from element in topic.Elements(MamlCompanion.Attribute)
													let name = (string) element.Attribute(MamlCompanion.AttributeName)
													where !string.IsNullOrEmpty(name)
													let value = (string) element
													where !string.IsNullOrEmpty(value)
													select new { name, value };

			foreach (var data in attributeData)
			{
				AddAttributeValue(data.name, data.value);
			}

			var indexData = from element in topic.Elements(MamlCompanion.Keyword)
											let index = (string) element.Attribute(MamlCompanion.KeywordIndex)
											where !string.IsNullOrEmpty(index)
											let list = MamlCompanion.ParseKeywords(element, index)
											where list.Count > 0
											select new { index, list };

			foreach (var data in indexData)
			{
				SetKeyword(data.index, data.list);
			}

			foreach (XElement element in from element in topic.Elements()
																	 where !MamlCompanion.IsKnownMetadata(element)
																	 select element)
			{
				customProperties[element.Name.LocalName] = (string) element;
			}
		}

		public void Save(string documentFile, bool setLastModifiedDateTime)
		{
			if (setLastModifiedDateTime)
			{
				LastModified = DateTimeOffset.Now;
			}

			var document = MamlCompanion.CreateDocument(this);

			string companionFile = GetCompanionFileForDocument(documentFile);

			document.Save(companionFile);
		}

		private void RaisePropertyChanged(string propertyName)
		{
			OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			var handler = PropertyChanged;

			if (handler != null)
			{
				handler(this, e);
			}
		}
	}
}