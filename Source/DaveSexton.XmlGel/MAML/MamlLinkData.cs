using System;
using System.ComponentModel;
using System.Xml.Linq;

namespace DaveSexton.XmlGel.Maml
{
	public sealed class MamlLinkData
	{
		#region Public Properties
		public MamlLinkKind LinkKind
		{
			get;
			set;
		}

		public string EntityId
		{
			get
			{
				return entityId ?? string.Empty;
			}
			set
			{
				entityId = value;
			}
		}

		public string DocumentId
		{
			get
			{
				return documentId ?? string.Empty;
			}
			set
			{
				documentId = value;
			}
		}

		public string Uri
		{
			get
			{
				return uri ?? string.Empty;
			}
			set
			{
				uri = value;
			}
		}

		public string Text
		{
			get
			{
				return text ?? string.Empty;
			}
			set
			{
				text = value;
			}
		}

		public string Value
		{
			get
			{
				switch (LinkKind)
				{
					case MamlLinkKind.XLink:
						return string.IsNullOrEmpty(text) ? documentId : text;
					case MamlLinkKind.CodeEntityReference:
						return string.IsNullOrEmpty(text) ? entityId : text;
					case MamlLinkKind.ExternalLink:
						return string.IsNullOrEmpty(text) ? uri : text;
					default:
						throw new InvalidOperationException();
				}
			}
		}

		public string AlternateText
		{
			get
			{
				return alternateText ?? string.Empty;
			}
			set
			{
				alternateText = value;
			}
		}

		public bool? QualifyHint
		{
			get
			{
				return qualifyHint;
			}
			set
			{
				qualifyHint = value;
			}
		}

		public bool? AutoUpgrade
		{
			get
			{
				return autoUpgrade;
			}
			set
			{
				autoUpgrade = value;
			}
		}
		#endregion

		#region Private / Protected
		private string text, alternateText, uri, documentId, entityId;
		private bool? qualifyHint, autoUpgrade;
		#endregion

		#region Constructors
		public MamlLinkData()
		{
		}

		public MamlLinkData(MamlLinkKind linkType)
		{
			LinkKind = linkType;
		}

		public MamlLinkData(MamlLinkData link)
		{
			SetFrom(link);
		}
		#endregion

		#region Methods
		public static MamlLinkKind GetLinkType(XElement element)
		{
			var name = element.Name;

			if (name == Maml.Link)
			{
				return MamlLinkKind.XLink;
			}
			else if (name == Maml.CodeEntityReference)
			{
				return MamlLinkKind.CodeEntityReference;
			}
			else if (name == Maml.ExternalLink)
			{
				return MamlLinkKind.ExternalLink;
			}

			throw new ArgumentOutOfRangeException();
		}

		public static XName GetLinkElementName(MamlLinkKind linkType)
		{
			switch (linkType)
			{
				case MamlLinkKind.XLink:
					return Maml.Link;
				case MamlLinkKind.CodeEntityReference:
					return Maml.CodeEntityReference;
				case MamlLinkKind.ExternalLink:
					return Maml.ExternalLink;
				default:
					throw new InvalidEnumArgumentException("linkType", (int) linkType, typeof(MamlLinkKind));
			}
		}

		public static MamlLinkData Parse(XElement element)
		{
			var type = GetLinkType(element);

			var data = new MamlLinkData(type);

			switch (type)
			{
				case MamlLinkKind.XLink:
					data.Text = element.Value;
					data.DocumentId = element.Attribute(Maml.XLinkHref).Value;
					break;
				case MamlLinkKind.CodeEntityReference:
					data.EntityId = element.Value;
					break;
				case MamlLinkKind.ExternalLink:
					XElement e = element.Element(Maml.LinkText);

					if (e != null)
					{
						data.Text = e.Value;
					}

					e = element.Element(Maml.LinkUri);

					if (e != null)
					{
						data.Uri = e.Value;
					}

					e = element.Element(Maml.LinkAlternateText);

					if (e != null)
					{
						data.AlternateText = e.Value;
					}

					break;
			}

			return data;
		}

		public void SetFrom(MamlLinkData link)
		{
			entityId = link.entityId;
			documentId = link.documentId;
			uri = link.uri;
			text = link.text;
			alternateText = link.alternateText;
			qualifyHint = link.qualifyHint;
			autoUpgrade = link.autoUpgrade;
			LinkKind = link.LinkKind;
		}

		public XElement ToElement()
		{
			switch (LinkKind)
			{
				case MamlLinkKind.XLink:
					return new XElement(Maml.Link,
						new XAttribute(Maml.XLinkPrefix, Maml.XLinkNamespaceUri),
						new XAttribute(Maml.XLinkHref, DocumentId),
						Text);
				case MamlLinkKind.CodeEntityReference:
					var entityLink = new XElement(Maml.CodeEntityReference, EntityId);

					if (qualifyHint.HasValue)
					{
						entityLink.SetAttributeValue(Maml.QualifyHint, qualifyHint.Value);
					}

					if (autoUpgrade.HasValue)
					{
						entityLink.SetAttributeValue(Maml.AutoUpgrade, autoUpgrade.Value);
					}

					return entityLink;
				case MamlLinkKind.ExternalLink:
					var externalLink = new XElement(
						Maml.ExternalLink,
						new XElement(Maml.LinkText, Text),
						new XElement(Maml.LinkUri, Uri));

					if (!string.IsNullOrEmpty(AlternateText))
					{
						externalLink.FirstNode.AddAfterSelf(
							new XElement(Maml.LinkAlternateText, AlternateText));
					}

					return externalLink;
				default:
					throw new InvalidOperationException();
			}
		}

		public override string ToString()
		{
			return Value;
		}
		#endregion
	}
}