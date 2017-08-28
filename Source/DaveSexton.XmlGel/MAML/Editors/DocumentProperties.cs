using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace DaveSexton.XmlGel.Maml.Editors
{
	[TypeConverter(typeof(DocumentPropertiesTypeConverter))]
	[DefaultProperty("Title")]
	public sealed class DocumentProperties : INotifyPropertyChanged
	{
		[SortedProperty(0)]
		[Category("File"), DisplayName("File Name")]
		[Description("The name of the file.")]
		public string FileName
		{
			get
			{
				return fileNameGetter == null ? string.Empty : fileNameGetter();
			}
		}

		[SortedProperty(10)]
		[Category("File"), DisplayName("ID")]
		[Description("The globally unique identifier (GUID) for the document.")]
		public Guid Id
		{
			get
			{
				return document == null ? Guid.Empty : document.Id;
			}
		}

		[SortedProperty(20)]
		[Category("File"), DisplayName("Revision")]
		[Description("Number of times that the document has been revised.")]
		public int RevisionNumber
		{
			get
			{
				return document == null ? 0 : document.RevisionNumber;
			}
			set
			{
				if (document != null && document.RevisionNumber != value)
				{
					document.RevisionNumber = value;

					RaisePropertyChanged("RevisionNumber");
				}
			}
		}

		[SortedProperty(30)]
		[Category("File"), DisplayName("Author")]
		[Description("Name of the document's author.  This may refer to the original author or the last author to modify the document.")]
		public string Author
		{
			get
			{
				var metadata = Metadata;

				return metadata == null ? string.Empty : metadata.Author;
			}
			set
			{
				var metadata = Metadata;

				if (metadata != null && !string.Equals(metadata.Author, value, StringComparison.Ordinal))
				{
					metadata.Author = value;

					RaisePropertyChanged("Author");
				}
			}
		}

		[SortedProperty(40)]
		[Category("File"), DisplayName("Last modified")]
		[Description("Date and time of the last revision.")]
		public DateTime LastModified
		{
			get
			{
				var metadata = Metadata;

				return metadata == null ? DateTime.MinValue : metadata.LastModified.LocalDateTime;
			}
			set
			{
				var metadata = Metadata;

				if (metadata != null && metadata.LastModified != value)
				{
					metadata.LastModified = value;

					RaisePropertyChanged("LastModified");
				}
			}
		}

		[SortedProperty(10)]
		[Category("Document"), DisplayName("Title"), DefaultValue(null), ParenthesizePropertyName()]
		[Description("Text that commonly appears in an automatic header when the topic is transformed into the target help style.")]
		public string Title
		{
			get
			{
				var metadata = Metadata;

				return metadata == null ? string.Empty : metadata.Title;
			}
			set
			{
				var metadata = Metadata;

				if (metadata != null && !string.Equals(metadata.Title, value, StringComparison.Ordinal))
				{
					metadata.Title = value;

					RaisePropertyChanged("Title");
				}
			}
		}

		[SortedProperty(20)]
		[Category("Document"), DisplayName("TOC Title"), DefaultValue(null)]
		[Description("Text that represents the topic in the table of contents (TOC).")]
		public string TableOfContentsTitle
		{
			get
			{
				var metadata = Metadata;

				return metadata == null ? string.Empty : metadata.TableOfContentsTitle;
			}
			set
			{
				var metadata = Metadata;

				if (metadata != null && !string.Equals(metadata.TableOfContentsTitle, value, StringComparison.Ordinal))
				{
					metadata.TableOfContentsTitle = value;

					RaisePropertyChanged("TableOfContentsTitle");
				}
			}
		}

		[SortedProperty(30)]
		[Category("Document"), DisplayName("Keywords")]
		[Description("Words that identify the topic in various indexes.  The 'K' index is the most commonly used index as it applies to a wider range of help formats.")]
		[Editor(typeof(MetadataKeywordsEditor), typeof(System.Drawing.Design.UITypeEditor))]
		[TypeConverter(typeof(MetadataKeywordsTypeConverter))]
		public ICollection<string> Keywords
		{
			get
			{
				var metadata = Metadata;

				return metadata == null
					? new List<string>(0).AsReadOnly()
					: (from i in metadata.KeywordIndexes
						 from k in metadata.GetKeywords(i)
						 select k)
							.ToList()
							.AsReadOnly();
			}
		}

		[SortedProperty(40)]
		[Category("Document"), DisplayName("Attributes")]
		[Description("Additional metadata for the topic in the form of name/value pairs.")]
		[Editor(typeof(MetadataAttributesEditor), typeof(System.Drawing.Design.UITypeEditor))]
		[TypeConverter(typeof(MetadataAttributesTypeConverter))]
		public ICollection<string> AttributeValues
		{
			get
			{
				var metadata = Metadata;

				return metadata == null
					? new List<string>(0).AsReadOnly()
					: (from n in metadata.AttributeNames
						 from v in metadata.GetAttributeValues(n)
						 select v)
							.ToList()
							.AsReadOnly();
			}
		}

		[SortedProperty(10)]
		[Category(CustomPropertiesCategory), DisplayName("Custom")]
		[Description("Custom properties are stored as XML elements in the document's companion file.")]
		[Editor(typeof(MetadataCustomPropertiesEditor), typeof(System.Drawing.Design.UITypeEditor))]
		[TypeConverter(typeof(MetadataCustomPropertiesTypeConverter))]
		[RefreshProperties(RefreshProperties.All)]
		public ICollection<string> CustomProperties
		{
			get
			{
				var metadata = Metadata;

				return metadata == null
					? new List<string>(0).AsReadOnly()
					: metadata.CustomProperties;
			}
		}

		[Browsable(false)]
		[RefreshProperties(RefreshProperties.All)]
		public MamlDocument Document
		{
			get
			{
				return document;
			}
			set
			{
				if (document != value)
				{
					document = value;

					AddAttributes();

					RaisePropertyChanged("Document");
				}
			}
		}

		[Browsable(false)]
		internal IMamlMetadata Metadata
		{
			get
			{
				var document = Document;

				return document == null ? null : document.Metadata;
			}
		}

		internal const string CustomPropertiesCategory = "Properties";

		private readonly IDictionary<string, List<string>> addAttributes;
		private readonly Func<string> fileNameGetter;
		private MamlDocument document;

		public DocumentProperties(Func<string> fileNameGetter, IDictionary<string, List<string>> addAttributes)
			: this(null, fileNameGetter, addAttributes)
		{
		}

		public DocumentProperties(MamlDocument document, Func<string> fileNameGetter, IDictionary<string, List<string>> addAttributes)
		{
			this.document = document;
			this.fileNameGetter = fileNameGetter;
			this.addAttributes = addAttributes;

			AddAttributes();
		}

		private void AddAttributes()
		{
			if (addAttributes != null && addAttributes.Count > 0)
			{
				var metadata = Metadata;

				if (metadata != null)
				{
					foreach (var pair in addAttributes)
					{
						if (pair.Value != null && pair.Value.Count > 0)
						{
							metadata.SetAttributeValues(pair.Key, pair.Value.ToArray());
						}
					}
				}
			}
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